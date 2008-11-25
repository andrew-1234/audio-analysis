using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using TowseyLib;
using AudioTools;

namespace AudioStuff
{
    public enum SonogramType { amplitude, spectral, cepstral, acousticVectors, sobelEdge }

	public sealed class Sonogram
	{
		#region Statics
		public const int binWidth = 1000; //1 kHz bands for calculating acoustic indices 

        //constants for analysing the logEnergy array for signal segmentation
        public const double minLogEnergy = -7.0;        // typical noise value for BAC2 recordings = -4.5
        public const double maxLogEnergy = -0.60206;    // = Math.Log10(0.25) which assumes max average frame amplitude = 0.5
        //public const double maxLogEnergy = -0.444;    // = Math.Log10(0.36) which assumes max average frame amplitude = 0.6
        //public const double maxLogEnergy = -0.310;    // = Math.Log10(0.49) which assumes max average frame amplitude = 0.7
        //note that the cicada recordings reach max average frame amplitude = 0.55

        //Following const used to normalise the logEnergy values to the background noise.
        //Has the effect of setting bcakground noise level to 0 dB. Value of 10dB is in Lamel et al, 1981 
        //Lamel et al call it "Adaptive Level Equalisatsion".
        public const double noiseThreshold = 10.0; //dB

		public static SonogramType SetSonogramType(string typeName)
		{
			if (string.IsNullOrEmpty(typeName))
				return SonogramType.spectral;
			if (typeName.StartsWith("spectral"))
				return SonogramType.spectral;
			if (typeName.StartsWith("cepstral"))
				return SonogramType.cepstral;
			if (typeName.StartsWith("acousticVectors"))
				return SonogramType.acousticVectors;
			if (typeName.StartsWith("sobelEdge"))
				return SonogramType.sobelEdge;
			return SonogramType.spectral; //the default
		}
		#endregion

		#region Properties
		public SonoConfig State { get; set; } //class containing state of all application parameters
		
		public double[] FrameEnergy { get; private set; } // energy per signal frame
		public double[] Decibels { get; private set; } // normalised decibels per signal frame
		public int[] ZeroCross { get; private set; } // number of zero crossings per frame
		public int[] SigState { get; private set; } // integer coded signal state ie  0=non-vocalisation, 1=vocalisation, etc.
		public double[,] AmplitudM { get; private set; } // the original matrix of FFT amplitudes i.e. unprocessed sonogram
		public double[,] SpectralM { get; private set; } // the sonogram of log energy spectra
		public double[,] CepstralM { get; private set; } // the matrix of energy, mfccs, delta and doubleDelta coefficients ie 3x(1+12)=39
		public double[,] AcousticM { get; private set; } // the matrix of acoustic vectors ie 3x39 for frames T-2, T, T+2
		#endregion

		#region Constructors
		/// <summary>
		/// CONSTRUCTOR 1
		/// Use this constructor when initialising  a sonogram from within a template
		/// </summary>
		public Sonogram(SonoConfig state, WavReader wav)
		{
			State = state;
			Make(wav);
			if (Log.Verbosity > 0) WriteInfo();
		}

		public Sonogram(SonoConfig state, StreamedWavReader wav)
		{
			State = state;
			Make(wav);
			if (Log.Verbosity > 0) WriteInfo();
		}

		/// <summary>
		/// CONSTRUCTOR 2
		/// </summary>
		public Sonogram(string iniFName)
		{
			State.ReadConfig(iniFName);
		}

		/// <summary>
		/// CONSTRUCTOR 3
		/// </summary>
		public Sonogram(string iniFName, string wavPath)
		{
			State.WavFilePath = wavPath;
			State.ReadConfig(iniFName);
			Make(wavPath);
		}

		/// <summary>
		/// CONSTRUCTOR 4
		/// This constructor called by the Template class when it creates a new Template
		/// Creates matrix of acoustic vectors.
		/// </summary>
		public Sonogram(SonoConfig state, string wavPath)
		{
			State = state;
			state.WavFilePath = wavPath;
			Make(wavPath);
		}

		/// <summary>
		/// CONSTRUCTOR 5
		/// </summary>
		public Sonogram(string iniFName, string wavPath, byte[] wavBytes)
		{
			State.WavFilePath = wavPath;
			State.ReadConfig(iniFName);
			State.SetDateAndTime(wavPath);

			//initialise WAV class with bytes array
			WavReader wav = new WavReader(wavBytes);
			Make(wav);
			if (Log.Verbosity > 0) WriteInfo();
		}

		/// <summary>
		/// CONSTRUCTOR 6
		/// </summary>
		public Sonogram(string iniFName, string sigName, double[] rawData, int sampleRate)
		{
			State.ReadConfig(iniFName);

			//initialise WAV class with double array
			WavReader wav = new WavReader(rawData, 1, 16, sampleRate);
			Make(wav);
			if (Log.Verbosity > 0) WriteInfo();
		}
		#endregion

        /// <summary>
        /// Makes the sonogram given path wav file.
        /// Assumes that sonogram class already initialised
        /// </summary>
        public void Make(string wavPath)
        {
            State.SetDateAndTime(Path.GetFileNameWithoutExtension(wavPath));

            //read the .WAV file
            WavReader wav = new WavReader(wavPath);
            Make(wav);
        }

		void Make(StreamedWavReader wav)
		{
			Make(wav.GetNonStreamedWavReader());
		}

        void Make(WavReader wav)
        {
            if (State.SubSample > 1)
				wav.SubSample(State.SubSample);

            //store essential parameters for this sonogram
			Log.WriteIfVerbose("\nCALCULATING SONOGRAM");
			// Removed this check for performance reasons
            /*if (wav.Amplitude_AbsMax == 0.0)
				throw new ArgumentException("Wav file has zero signal. Cannot make sonogram.");
            state.WavMax        = wav.Amplitude_AbsMax;*/
            
            State.SampleRate    = wav.SampleRate;
            State.TimeDuration  = wav.Time.TotalSeconds;
            
            State.MinFreq       = 0;                     //the default minimum freq (Hz)
            State.NyquistFreq   = State.SampleRate / 2;  //Nyquist
            if (State.FreqBand_Min <= 0)
                State.FreqBand_Min  = State.MinFreq;    //default min of the freq band to be analysed  
            if (State.FreqBand_Max <= 0)
                State.FreqBand_Max  = State.NyquistFreq;    //default max of the freq band to be analysed
            if (State.FreqBand_Min > 0 || State.FreqBand_Max < State.NyquistFreq)
				State.doFreqBandAnalysis = true;

            State.FrameDuration = State.WindowSize / (double)State.SampleRate; // window duration in seconds
            State.FrameOffset   = State.FrameDuration * (1 - State.WindowOverlap);// duration in seconds
            State.FreqBinCount  = State.WindowSize / 2; // other half is phase info
            State.MelBinCount   = State.FreqBinCount; // same has number of Hz bins
            State.FBinWidth     = State.NyquistFreq / (double)State.FreqBinCount;
            State.FrameCount = (int)(State.TimeDuration / State.FrameOffset);
            State.FramesPerSecond = 1 / State.FrameOffset;

            double[] signal = wav.Samples;

            //SIGNAL PRE-EMPHASIS helps with speech signals
            bool doPreemphasis = false;
            if (doPreemphasis)
            {
                double coeff = 0.96;
                signal = DSP.PreEmphasis(signal, coeff);
            }

            //FRAME WINDOWING
            int step = (int)(State.WindowSize * (1 - State.WindowOverlap));
            double[,] frames = DSP.Frames(signal, State.WindowSize, step);
            State.FrameCount = frames.GetLength(0);

            //ENERGY PER FRAME
            FrameEnergy = DSP.SignalLogEnergy(frames, Sonogram.minLogEnergy, Sonogram.maxLogEnergy);
            //Console.WriteLine("FrameNoiseDecibels=" + State.FrameNoiseLogEnergy + "  FrameMaxDecibels=" + State.FrameMaxLogEnergy);
            
            //FRAME NOISE SUBTRACTION: subtract background noise to produce decibels array in which zero dB = average noise
            double minEnergyRatio = Sonogram.minLogEnergy - Sonogram.maxLogEnergy;
            double Q;
            double min_dB;
            double max_dB;
            Decibels = DSP.NoiseSubtract(FrameEnergy, out min_dB, out max_dB, minEnergyRatio, Sonogram.noiseThreshold, out Q);
            State.NoiseSubtracted = Q;
            State.FrameNoise_dB = min_dB; //min decibels of all frames 
            State.FrameMax_dB = max_dB;
            State.Frame_SNR = max_dB - min_dB;
            State.MinDecibelReference = min_dB - Q;
            State.MaxDecibelReference = (Sonogram.maxLogEnergy * 10) - Q;

            // ZERO CROSSINGS
            //this.zeroCross = DSP.ZeroCrossings(frames);

            //DETERMINE ENDPOINTS OF VOCALISATIONS
            double k1 = State.MinDecibelReference + State.SegmentationThreshold_k1;
            double k2 = State.MinDecibelReference + State.SegmentationThreshold_k2;
            int k1_k2delay = (int)(State.k1_k2Latency / State.FrameOffset); //=5  frames delay between signal reaching k1 and k2 thresholds
            int syllableDelay = (int)(State.vocalDelay / State.FrameOffset); //=10 frames delay required to separate vocalisations 
            int minPulse = (int)(State.minPulseDuration / State.FrameOffset); //=2 frames is min vocal length
            //Console.WriteLine("k1_k2delay=" + k1_k2delay + "  syllableDelay=" + syllableDelay + "  minPulse=" + minPulse);
            SigState = Speech.VocalizationDetection(Decibels, k1, k2, k1_k2delay, syllableDelay, minPulse, null);
            State.FractionOfHighEnergyFrames = Speech.FractionHighEnergyFrames(Decibels, k2);
            if ((State.FractionOfHighEnergyFrames > 0.8)&&(State.DoNoiseReduction))
            {
                Console.WriteLine("\n\t################### Sonogram.Make(WavReader wav): WARNING ##########################################");
                Console.WriteLine("\t################### This is a high energy recording. The fraction of high energy frames = "
                                                                + State.FractionOfHighEnergyFrames.ToString("F2") + " > 80%");
                Console.WriteLine("\t################### Noise reduction algorithm may not work well in this instance!\n");
            }

            //generate the spectra of FFT AMPLITUDES
            //calculate a minimum amplitude to prevent taking log of small number. This would increase the range when normalising
            double epsilon = Math.Pow(0.5, wav.BitsPerSample - 1);
            AmplitudM = MakeAmplitudeSpectra(frames, State.WindowFnc, epsilon);
			Log.WriteIfVerbose("\tDim of amplitude spectrum =" + AmplitudM.GetLength(1));

            //EXTRACT REQUIRED FREQUENCY BAND
            if (State.doFreqBandAnalysis)
            {
                int c1 = (int)(State.freqBand_Min / State.FBinWidth);
                int c2 = (int)(State.freqBand_Max / State.FBinWidth);
                AmplitudM = DataTools.Submatrix(AmplitudM, 0, c1, AmplitudM.GetLength(0)-1, c2);
                Log.WriteIfVerbose("\tDim of required sub-band  =" + AmplitudM.GetLength(1));
                //DETERMINE ENERGY IN FFT FREQ BAND
                Decibels = FreqBandEnergy(AmplitudM);
                //DETERMINE ENDPOINTS OF VOCALISATIONS
                SigState = Speech.VocalizationDetection(Decibels, k1, k2, k1_k2delay, syllableDelay, minPulse, null);
            }
            
            //POST-PROCESS to final SPECTROGRAM
			switch (State.SonogramType)
			{
				case SonogramType.spectral:
					SpectralM = MakeSpectrogram(AmplitudM);
					break;
				case SonogramType.cepstral:
					CepstralM = MakeCepstrogram(AmplitudM, Decibels, State.IncludeDelta, State.IncludeDoubleDelta);
					break;
				case SonogramType.acousticVectors:
					AcousticM = MakeAcousticVectors(AmplitudM, Decibels, State.IncludeDelta, State.IncludeDoubleDelta, State.DeltaT);
					break;
				case SonogramType.sobelEdge:
					SpectralM = SobelEdgegram(AmplitudM);
					break;
			}
        } //end Make(WavReader wav)

        public double[,] MakeAmplitudeSpectra(double[,] frames, TowseyLib.FFT.WindowFunc w, double epsilon)
        {
            int frameCount = frames.GetLength(0);
            int N = frames.GetLength(1);  //= the FFT windowSize 
            int binCount = (N / 2) + 1;  // = fft.WindowSize/2 +1 for the DC value;

            var fft = new TowseyLib.FFT(N, w); // init class which calculates the FFT

            //calculate a minimum amplitude to prevent taking log of small number. This would increase the range when normalising
            int smoothingWindow = 3; //to smooth the spectrum 

            double[,] sonogram = new double[frameCount, binCount];

            for (int i = 0; i < frameCount; i++)//foreach time step
            {
                double[] data = DataTools.GetRow(frames, i);
                double[] f1 = fft.Invoke(data);
                f1 = DataTools.filterMovingAverage(f1, smoothingWindow); //to smooth the spectrum - reduce variance
                for (int j = 0; j < binCount; j++) //foreach freq bin
                {
                    double amplitude = f1[j];
                    if (amplitude < epsilon) amplitude = epsilon; //to prevent possible log of a very small number
                    sonogram[i, j] = amplitude;
                }
            } //end of all frames
            return sonogram;
        }

        public double[] FreqBandEnergy(double[,] fftAmplitudes) 
        {
            //Console.WriteLine("minDefinedLogEnergy=" + Sonogram.minLogEnergy.ToString("F2") + "  maxLogEnergy=" + Sonogram.maxLogEnergy);
            double[] logEnergy = DSP.SignalLogEnergy(fftAmplitudes, Sonogram.minLogEnergy, Sonogram.maxLogEnergy);

            //NOTE: FreqBand LogEnergy levels are higher than Frame levels but SNR remains same.
            //double min; double max;
            //DataTools.MinMax(logEnergy, out min, out max);
            //Console.WriteLine("FrameNoise_dB   =" + State.FrameNoise_dB    + "  FrameMax_dB   =" + State.FrameMax_dB    + "  SNR=" + State.Frame_SNR);
            //Console.WriteLine("FreqBandNoise_dB=" + State.FreqBandNoise_dB + "  FreqBandMax_dB=" + State.FreqBandMax_dB + "  SNR=" + State.FreqBand_SNR);
            //Console.WriteLine("FreqBandNoise_dB=" + (min*10) + "  FreqBandMax_dB=" + (max*10) + "  SNR=" + State.FreqBand_SNR);

            //noise reduce the energy array to produce decibels array
            double minFraction = Sonogram.minLogEnergy - Sonogram.maxLogEnergy;
            double Q;
            double min_dB;
            double max_dB;
            double[] decibels = DSP.NoiseSubtract(logEnergy, out min_dB, out max_dB, minFraction, Sonogram.noiseThreshold, out Q);
            State.NoiseSubtracted = Q;
            State.FreqBandNoise_dB = min_dB; //min decibels of all frames 
            State.FreqBandMax_dB = max_dB;
            State.FreqBand_SNR = max_dB - min_dB;
            State.MinDecibelReference = min_dB - State.NoiseSubtracted;
            State.MaxDecibelReference = State.MinDecibelReference + State.FreqBand_SNR;
            //State.MaxDecibelReference = (Sonogram.maxLogEnergy * 10) - State.NoiseSubtracted;
            //Console.WriteLine("Q=" + State.NoiseSubtracted + "  MinDBReference=" + State.MinDecibelReference + "  MaxDecibelReference=" + State.MaxDecibelReference);
            return decibels;
        }

        /// <summary>
        /// trims the values of the passed spectrogram using the Min and Max percentile values in the ini file.
        /// First calculate the value cut-offs for the given percentiles.
        /// Second, calculate the min, avg and max values of the spectrogram.
        /// </summary>
        public double[,] Trim(double[,] SPEC, out double min, out double avg, out double max)
        {
            int frameCount = SPEC.GetLength(0);
            int binCount   = SPEC.GetLength(1);

            //normalise and compress/bound the values
            double minCut;
            double maxCut;
            DataTools.PercentileCutoffs(SPEC, State.MinPercentile, State.MaxPercentile, out minCut, out maxCut);
            State.MinCut = minCut;
            State.MaxCut = maxCut;
            AmplitudM = DataTools.boundMatrix(AmplitudM, minCut, maxCut);

            min = Double.MaxValue;
            max = Double.MinValue;
            double sum = 0.0;

            for (int i = 0; i < frameCount; i++)//foreach time step
            {
                for (int j = 0; j < binCount; j++) //foreach freq bin
                {
                    double value = SPEC[i, j];
                    if (value < min) min = value;
                    else
                        if (value > max) max = value;
                    sum += value;
                }
            } //end of all frames
            avg = sum / (frameCount * binCount);
            return SPEC;
        }

        /// <summary>
        /// Converts amplitude spectra to power spectra
        /// Does NOT apply filter bank i.e. returns full bandwidth spectrogram
        /// </summary>
        public double[,] MakeSpectrogram(double[,] matrix)
        {
            Log.WriteIfVerbose(" MakeSpectrogram(double[,] matrix)");
            double[,] m = Speech.DecibelSpectra(matrix);

            if (State.DoNoiseReduction)
            {
                Log.WriteIfVerbose("\t... doing noise reduction.");
                m = ImageTools.NoiseReduction(m); //Mel scale conversion should be done before noise reduction
            }

            //string bmpfPath = @"C:\SensorNetworks\Sonograms\spectralM_create.bmp";
            //ImageTools.DrawMatrix(m, bmpfPath);
            return m;
        }

        public double[,] ApplyFilterBank(double[,] matrix)
        {
            Log.WriteIfVerbose(" ApplyFilterBank(double[,] matrix)");
            //error check that filterBankCount < FFTbins
            int FFTbins = State.FreqBinCount;  //number of Hz bands = 2^N +1. Subtract DC bin
            if (State.FilterbankCount > FFTbins)
            {
                throw new Exception("####### FATAL ERROR:- Sonogram.ApplyFilterBank():- Cannot calculate cepstral coefficients. FilterbankCount > FFTbins. (" + State.FilterbankCount + " > " + FFTbins+")\n\n");
                //Console.WriteLine("Change size of filter bank from " + State.FilterbankCount + " to " + FFTbins);
                //State.FilterbankCount = FFTbins;
            }

            double Nyquist = State.NyquistFreq;
            State.MaxMel = Speech.Mel(Nyquist);
            //this is the filter count for full bandwidth 0-Nyquist. This number is trimmed proportionately to fit the required bandwidth. 
            int bandCount = State.FilterbankCount;
            double[,] m = matrix;
            Log.WriteIfVerbose("\tDim prior to filter bank  =" + m.GetLength(1));

            if (State.DoMelScale)
            {
                //Console.WriteLine(" Mel Nyquist= " + State.MaxMel.ToString("F1"));
                //Console.WriteLine(" Mel Band Count = " + State.MelBinCount + " FilterbankCount= " + State.FilterbankCount);
                if (State.SonogramType == SonogramType.spectral) bandCount = State.MelBinCount;

                m = Speech.MelFilterBank(m, bandCount, Nyquist, State.freqBand_Min, State.freqBand_Max);  //using the Greg integral
                //m = Speech.MelFilterBank(m, bandCount, Nyquist);  //using the Greg integral
                //m = Speech.MelFilterbank(m, bandCount, Nyquist);  //using the Matlab algorithm
            }
            else
            {
                m = Speech.LinearFilterBank(m, bandCount, Nyquist, State.freqBand_Min, State.freqBand_Max);
            }
            Log.WriteIfVerbose("\tDim after use of filter bank=" + m.GetLength(1) + " (Max filter bank=" + bandCount + ")");

            return m;
        } //end ApplyFilterBank(double[,] matrix)

        public double[,] MakeCepstrogram(double[,] matrix, double[] decibels, bool includeDelta, bool includeDoubleDelta)
        {
            Log.WriteIfVerbose(" MakeCepstrogram(matrix, decibels, includeDelta=" + includeDelta + ", includeDoubleDelta=" + includeDoubleDelta + ")");

            double[,] m = ApplyFilterBank(matrix);
            m = Speech.DecibelSpectra(m);
            //string bmpfPath = @"C:\SensorNetworks\Sonograms\spectralM_create.bmp";
            //ImageTools.DrawMatrix(m, bmpfPath);

            if (State.DoNoiseReduction)
            {
                Log.WriteIfVerbose("\t... doing noise reduction.");
                m = ImageTools.NoiseReduction(m); //Mel scale conversion should be done before noise reduction
            }

            SpectralM = m; //stores the reduced bandwidth, filtered, noise reduced spectra as new spectrogram

            //calculate cepstral coefficients and normalise
            m = Speech.Cepstra(m, State.ccCount);
            m = DataTools.normalise(m);

            //calculate the full range of MFCC coefficients ie including energy and deltas, etc
            //normalise energy between 0.0 decibels and max decibels.
            double[] E = Speech.NormaliseEnergyArray(decibels, State.MinDecibelReference, State.MaxDecibelReference);
            m = Speech.AcousticVectors(m, E, includeDelta, includeDoubleDelta);
            return m;
        }

        public double[,] MakeAcousticVectors(double[,] matrix, double[] decibels, bool includeDelta, bool includeDoubleDelta, int deltaT)
        {
            Log.WriteIfVerbose(" MakeAcousticVectors(matrix, decibels, includeDelta=" + includeDelta + ", includeDoubleDelta=" + includeDoubleDelta + ", deltaT=" + deltaT + ")");
            
            double[,] m = MakeCepstrogram(matrix, decibels, includeDelta, includeDoubleDelta);
            CepstralM = m;

            //initialise feature vector for template - will contain three acoustic vectors - for T-dT, T and T+dT
            int frameCount = m.GetLength(0);
            int cepstralL  = m.GetLength(1);  // length of cepstral vector 
            int featurevL  = 3 * cepstralL;   // to accomodate cepstra for T-2, T and T+2

            double[] featureVector = new double[featurevL];
            double[,] acousticM = new double[frameCount, featurevL]; //init the matrix of acoustic vectors
            for (int i = deltaT; i < frameCount-deltaT; i++)
            {
                double[] rowTm2 = DataTools.GetRow(m, i - deltaT);
                double[] rowT   = DataTools.GetRow(m, i);
                double[] rowTp2 = DataTools.GetRow(m, i + deltaT);

                for (int j = 0; j < cepstralL; j++) acousticM[i, j] = rowTm2[j];
                for (int j = 0; j < cepstralL; j++) acousticM[i, cepstralL + j] = rowT[j];
                for (int j = 0; j < cepstralL; j++) acousticM[i, cepstralL + cepstralL + j] = rowTp2[j];
            }

            //return m;
            return acousticM;
        }

        public double[,] SobelEdgegram(double[,] matrix)
        {
            double[,] m = Speech.DecibelSpectra(matrix);
            if (State.DoNoiseReduction) m = ImageTools.NoiseReduction(m);
            m = ImageTools.SobelEdgeDetection(m);
            return m;
        }

        public double[,] Gradient()
        {
            double gradThreshold = 2.0;
            int fWindow = 11;
            int tWindow = 9;
            double[,] blurM = ImageTools.Blur(AmplitudM, fWindow, tWindow);
            int height = blurM.GetLength(0);
            int width  = blurM.GetLength(1);
            double[,] outData = new double[height, width];

            double min = Double.MaxValue;
            double max = -Double.MaxValue;

            for (int x = 0; x < width; x++) outData[0, x] = 0.5; //patch in first  time step with zero gradient
            for (int x = 0; x < width; x++) outData[1, x] = 0.5; //patch in second time step with zero gradient
           // for (int x = 0; x < width; x++) this.gradM[2, x] = 0.5; //patch in second time step with zero gradient

            for (int y = 2; y < height - 1; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double grad1 = blurM[y, x] - blurM[y - 1, x];//calculate one step gradient
                    double grad2 = blurM[y, x] - blurM[y - 2, x];//calculate two step gradient

                    //get min and max gradient
                    if (grad1 < min) min = grad1;
                    else
                    if (grad1 > max) max = grad1;

                    // quantize the gradients
                    if (grad1 < -gradThreshold) outData[y, x] = 0.0;
                    else
                        if (grad1 > gradThreshold) outData[y, x] = 1.0;
                        else
                            if (grad2 < -gradThreshold) outData[y, x] = 0.0;
                            else
                                if (grad2 > gradThreshold) outData[y, x] = 1.0;
                                else outData[y, x] = 0.5;
                }
            }

            //for (int x = 0; x < width; x++) this.gradM[height - 1, x] = 0.5; //patch in last time step with medium gradient
            return outData;
        }

        public double[] CalculatePowerHisto()
        {
            int bandCount = State.NyquistFreq / Sonogram.binWidth;
            State.kHzBandCount = bandCount;
            int tracksPerBand = State.FreqBinCount / bandCount;
            int height = AmplitudM.GetLength(0); //time dimension
            int width = AmplitudM.GetLength(1);
            double[] power = new double[bandCount];


            for (int f = 0; f < bandCount; f++) // over all 11 bands
            {
                int minTrack = f * tracksPerBand;
                int maxTrack = ((f + 1) * tracksPerBand) - 1;
                for (int y = 0; y < height; y++) //full duration of recording
                {
                    for (int x = minTrack; x < maxTrack; x++) //full width of freq band
                        power[f] += AmplitudM[y, x]; //sum the power
                }
            }

            double[] histo = new double[bandCount];
            for (int f = 0; f < bandCount; f++)
            {
                histo[f] = power[f] / (double)tracksPerBand / State.FrameCount;
            }
            return histo;
        }

        public double[] CalculateEventHisto(double[,] gradM)
        {
            int bandCount = State.NyquistFreq / Sonogram.binWidth;
            State.kHzBandCount = bandCount;
            int tracksPerBand = State.FreqBinCount / bandCount;
            int height = AmplitudM.GetLength(0); //time dimension
            int width = AmplitudM.GetLength(1);
            int[] counts = new int[bandCount];

            for (int f = 0; f < bandCount; f++) // over all 11 bands
            {
                int minTrack = f * tracksPerBand;
                int maxTrack = ((f + 1) * tracksPerBand) - 1;
                for (int y = 1; y < height; y++) //full duration of recording
                    for (int x = minTrack; x < maxTrack; x++) //full width of freq band
                        if (gradM[y, x] != gradM[y-1, x])
							counts[f]++; //count any gradient change
            }
            double[] histo = new double[bandCount];
            for (int f = 0; f < bandCount; f++)
                histo[f] = counts[f] / (double)tracksPerBand / State.TimeDuration;
            return histo;
        }

        public double[] CalculateEvent2Histo(double[,] gradM)
        {
            int bandCount = State.NyquistFreq / Sonogram.binWidth;
            State.kHzBandCount = bandCount;
            int tracksPerBand = State.FreqBinCount / bandCount;
            int height = AmplitudM.GetLength(0); //time dimension
            int width  = AmplitudM.GetLength(1);
            double[] positiveGrad = new double[bandCount];
            double[] negitiveGrad = new double[bandCount];


            for (int f = 0; f < bandCount; f++) // over all 11 bands
            {
                int minTrack = f * tracksPerBand;
                int maxTrack = ((f + 1) * tracksPerBand) - 1;
                for (int y = 0; y < height; y++) //full duration of recording
                {
                    for (int x = minTrack; x < maxTrack; x++) //full width of freq band
                    {
                        double d = gradM[y,x];
                        if (d == 0)
							negitiveGrad[f]++;
                        else if (d == 1)
							positiveGrad[f]++;
                    }
                }
            }
            double[] histo = new double[bandCount];
            for (int f = 0; f < bandCount; f++)
            {
                if (positiveGrad[f] > negitiveGrad[f])
					histo[f] = positiveGrad[f] / (double)tracksPerBand / State.TimeDuration;
                else
					histo[f] = negitiveGrad[f] / (double)tracksPerBand / State.TimeDuration;
            }
            return histo;
        }

        public double[] CalculateActivityHisto(double[,] gradM)
        {
            int bandCount = State.NyquistFreq / Sonogram.binWidth;
            State.kHzBandCount = bandCount;
            int tracksPerBand = State.FreqBinCount / bandCount;
            int height = AmplitudM.GetLength(0); //time dimension
            int width = AmplitudM.GetLength(1);
            double[] activity = new double[bandCount];

            for (int f = 0; f < bandCount; f++) // over all 11 bands
            {
                int minTrack = f * tracksPerBand;
                int maxTrack = ((f + 1) * tracksPerBand) - 1;
                for (int y = 0; y < height; y++) //full duration of recording
                    for (int x = minTrack; x < maxTrack; x++) //full width of freq band
                        activity[f] += (gradM[y, x] * gradM[y, x]); //add square of gradient
            }

            double[] histo = new double[bandCount];
            for (int f = 0; f < bandCount; f++)
                histo[f] = activity[f] / (double)tracksPerBand / State.TimeDuration;
            return histo;
        }

        public void WriteInfo()
        {
			Log.WriteLine("\nSONOGRAM INFO");
			Log.WriteLine(" Wav Sample Rate = " + State.SampleRate + "\tNyquist Freq = " + State.NyquistFreq + " Hz");
            if (State.SubSample > 1)
				Log.WriteLine(" Sub sampling interval = " + State.SubSample);
			Log.WriteLine(/*" SampleCount     = " + State.SampleCount + */"\tDuration=" + State.TimeDuration.ToString("F3") + "s");
			Log.WriteLine(" Frame Size      = " + State.WindowSize + "\t\tFrame Overlap = " + (int)(State.WindowOverlap * 100) + "%");
			Log.WriteLine(" Frame duration  = " + (State.FrameDuration * 1000).ToString("F1") + " ms. \tFrame Offset = " + (State.FrameOffset * 1000).ToString("F1") + " ms");
			Log.WriteLine(" Frame count     = " + State.FrameCount + "\t\tFrames/sec = " + State.FramesPerSecond.ToString("F1"));
			Log.WriteLine(" Min freq        = " + State.FreqBand_Min + " Hz. \tMax freq = " + State.FreqBand_Max + " Hz");
			Log.WriteLine(" Freq Bin Width  = " + (State.NyquistFreq / (double)State.FreqBinCount).ToString("F1") + " Hz");
			Log.Write(" Min Frame Noise = " + State.FrameNoise_dB.ToString("F2") + " dB");
			Log.WriteLine("\tS/N Ratio = " + State.Frame_SNR.ToString("F2") + " dB (maxFrameLogEn-minFrameLogEn)");
			Log.WriteLine(" Fraction of high energy frames (above k2 threshold) = " + State.FractionOfHighEnergyFrames.ToString("F2"));
            if (State.doFreqBandAnalysis)
            {
				Log.Write(" Min FBand Noise = " + State.FreqBandNoise_dB.ToString("F2") + " dB");
				Log.WriteLine("\tS/N Ratio = " + State.FreqBand_SNR.ToString("F2") + " dB (maxFreqBand-minFreqBand)");
            }
            Log.WriteLine(" Modal Noise(dB) = " + State.NoiseSubtracted.ToString("F2")+ "  (This dB level subtracted for normalisation)");
			Log.WriteLine(" Min reference dB= " + State.MinDecibelReference.ToString("F2") + "\tMax reference dB=" + State.MaxDecibelReference.ToString("F2"));
			//Log.WriteLine(" Min power=" + State.PowerMin.ToString("F3") + " Avg power=" + State.PowerAvg.ToString("F3") + " Max power=" + State.PowerMax.ToString("F3"));
			//Log.WriteLine(" Min percentile=" + State.MinPercentile.ToString("F2") + "  Max percentile=" + State.MaxPercentile.ToString("F2"));
			//Log.WriteLine(" Min cutoff=" + State.MinCut.ToString("F3") + "  Max cutoff=" + State.MaxCut.ToString("F3"));

			Log.WriteLine(" Sonogram Type   = " + State.SonogramType);
			Log.WriteLine(" Filterbank count= " + State.FilterbankCount);

            if (State.DoMelScale)
            {
				Log.WriteLine(" Mel Conversion  = true");
				Log.WriteLine(" Mel Nyquist     = " + State.MaxMel.ToString("F1"));
				Log.WriteLine(" Mel Band Count  = " + State.MelBinCount);// + " FilterbankCount= " + State.FilterbankCount);
            }
            else
				Log.WriteLine(" Linear Filterbank.");

            int coeffcount = State.ccCount + 1; //number of MFCCs + 1 for energy
            string str = "("+ coeffcount;
            if (State.IncludeDelta) str += (" + " + coeffcount);
            if (State.IncludeDoubleDelta) str += (" + " + coeffcount);
            str += " - including addition of delta coefficients)";
			//Log.WriteLine(" mfccCount=" + mfccCount + " coeffcount=" + coeffcount + " dim=" + dim);

            Console.WriteLine(" Cepstral coeff count = " + State.ccCount);
            if (AmplitudM == null)
				Log.WriteLine(" Matrix of spectral amplitudes does not exist!");
            else
				Log.WriteLine(" Dimension of ampltude vector = " + AmplitudM.GetLength(1) + "  (Before application of filter bank)");

            if (SpectralM == null)
				Log.WriteLine(" Matrix of spectral power does not exist!");
            else
				Log.WriteLine(" Dimension of spectral vector = " + SpectralM.GetLength(1) + "  (After  application of filter bank)");

            if (CepstralM == null)
				Log.WriteLine(" Matrix of cepstral coefficients does not exist!");
            else
				Log.WriteLine(" Dimension of cepstral vector = " + CepstralM.GetLength(1) + "  " + str);

            if (AcousticM == null)
				Log.WriteLine(" Matrix of acoustic vectors does not exist!");
            else
				Log.WriteLine(" Dimension of acoustic vector = " + AcousticM.GetLength(1) + "  (After inclusion of +&- T frames)");
        }

        public void WriteStatistics()
        {
			Log.WriteLine("\nSONOGRAM STATISTICS");
			Log.WriteLine(" Max power=" + State.PowerMax.ToString("F3") + " dB");
			Log.WriteLine(" Avg power=" + State.PowerAvg.ToString("F3") + " dB");
            //results.WritePowerHisto();
            //results.WritePowerEntropy();
            //results.WriteEventHisto();
            //results.WriteEventEntropy();
        }

		#region Image Saving Methods
		public double[,] GetMatrix(SonogramType type)
		{
			switch (type)
			{
				case SonogramType.amplitude:
					return AmplitudM;
				case SonogramType.cepstral:
					return CepstralM;
				case SonogramType.acousticVectors:
					return AcousticM;
				default:
					return SpectralM;
			}
		}

		public Image GetImage(SonogramType type, List<Track> tracks)
		{
			double[,] m = GetMatrix(type);
			return GetImage(m, tracks);
		}

		public void SaveImage(string path, SonogramType sType, List<Track> tracks)
		{
			//create and save image to file
			double[,] m = GetMatrix(sType);
			SaveImage(path, m, tracks);
		}

		public Image GetImage(double[,] m, List<Track> tracks)
		{
			if (m == null)
				throw new InvalidOperationException("Cannot save the sonogram as image");

			SonoImage image = new SonoImage(this);
			image.SetTracks(tracks);
			return image.CreateBitmap(m);
		}

		public void SaveImage(string path, double[,] m, List<Track> tracks)
		{
			var image = GetImage(m, tracks);
			image.Save(path);
		}

		public void SaveImage(string path, SonogramType sType, TrackType tType, int[] data)
		{
			if ((data == null) || (data.Length == 0))
			{
				Console.WriteLine("WARNING!!!!  Sonogram.SaveImage(): sonogram data ==null CANNOT SAVE THE SONOGRAM AS IMAGE!");
				return;
			}

			double[,] m = GetMatrix(sType);
			if (tType == TrackType.none)
				SaveImage(path, m, null);
			else
			{
				//add syllable ID track
				var tracks = new List<Track>();
				Track track1 = new Track(tType, data);
				track1.GarbageID = State.FeatureVectorCount + 2 - 1;
				tracks.Add(track1);

				//create and save image to file
				SaveImage(path, m, tracks);
			}
		}

		public void SaveImage(string path, double[,] matrix, TrackType tType, double[] data)
		{
			var tracks = new List<Track>();
			if (tType == TrackType.energy)
			{
				Track track = new Track(TrackType.energy, data);
				track.MinDecibelReference = State.MinDecibelReference;
				track.MaxDecibelReference = State.MaxDecibelReference;
				track.SegmentationThreshold_k1 = State.SegmentationThreshold_k1;
				track.SegmentationThreshold_k2 = State.SegmentationThreshold_k2;
				track.SetIntArray(this.SigState);
				tracks.Add(track);
			}
			else if (tType == TrackType.scoreMatrix)
			{
				Track track = new Track(TrackType.scoreMatrix, data);
				track.ScoreThreshold = State.ZScoreThreshold;
				tracks.Add(track);
			}

			SaveImage(path, matrix, tracks);
		}

		//public void SaveImage(double[,] sono, int[] hits, double[,] scores)
		//{
		//    if(hits==null) SaveImage(sono, scores);
		//    if (sono == null)
		//    {
		//        //throw new Exception("WARNING!!!!  matrix==null CANNOT SAVE THE SONOGRAM AS IMAGE!");
		//        Console.WriteLine("WARNING!!!!  Sonogram.SaveImage(): sonogram data ==null CANNOT SAVE THE SONOGRAM AS IMAGE!");
		//        return;
		//    }

		//    SonoImage image = new SonoImage(this);

		//    Track track1 = new Track(TrackType.syllables, hits);
		//    track1.GarbageID = state.FeatureVectorCount + 2 - 1;
		//    image.AddTrack(track1);

		//    //add score tracks one at a time
		//    int L = scores.GetLength(1);
		//    for (int i = 0; i < scores.GetLength(0); i++)
		//    {
		//        //transfer scores to an array
		//        double[] scoreArray = new double[L];
		//        for (int j = 0; j < L; j++) scoreArray[j] = scores[i, j];

		//        Track track2 = new Track(TrackType.scoreArray, scoreArray);
		//        track2.ScoreThreshold = state.ZScoreThreshold;
		//        image.AddTrack(track2);
		//    }

		//    //create and save image to file
		//    Bitmap bmp = image.CreateBitmap(sono);
		//    string fName        = State.SonogramDir + State.WavFName + State.BmpFileExt;
		//    State.BmpFName = fName;
		//    if (State.Verbosity != 0) Console.WriteLine("\n Image in file  = " + fName);
		//    bmp.Save(fName);
		//}

		public void SaveImage(string path, double[,] matrix, ArrayList shapes, Color col)
		{
			State.SonogramType = SonogramType.spectral; //image is linear scale not mel scale

			SonoImage image = new SonoImage(this);
			Bitmap bmp = image.CreateBitmap(matrix);
			if (shapes != null)
				bmp = image.AddShapeBoundaries(bmp, shapes, col);

			bmp.Save(path);
		}

		public void SaveImageOfSolids(string path, double[,] matrix, ArrayList shapes, Color col)
		{
			State.SonogramType = SonogramType.spectral; //image is linear scale not mel scale

			SonoImage image = new SonoImage(this);
			Bitmap bmp = image.CreateBitmap(matrix);
			if (shapes != null)
				bmp = image.AddShapeSolids(bmp, shapes, col);

			bmp.Save(path);
		}

		public void SaveImageOfCentroids(string path, double[,] matrix, ArrayList shapes, Color col)
		{
			State.SonogramType = SonogramType.spectral; //image is linear scale not mel scale

			SonoImage image = new SonoImage(this);
			Bitmap bmp = image.CreateBitmap(matrix);
			if (shapes != null)
				bmp = image.AddCentroidBoundaries(bmp, shapes, col);

			bmp.Save(path);
		}
		#endregion

        /// <summary>
        /// WARNING!! This method must be consistent with the ANALYSIS HEADER line declared in Results.AnalysisHeader()
        /// </summary>
        public string OneLineResult(int id, int[] syllableDistribution, int[] categoryDistribution, int categoryCount)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(id + Results.spacer); //CALLID
            //sb.Append(DateTime.Now.ToString("u") + spacer); //DATE
            sb.Append(Path.GetFileNameWithoutExtension(State.WavFilePath) + Results.spacer); //sonogram FNAME
            sb.Append(State.Time.ToString() + Results.spacer); //sonogram date
            sb.Append(State.DeploymentName + Results.spacer); //Deployment name
            sb.Append(State.TimeDuration.ToString("F2") + Results.spacer); //length of recording
			if (State.Time != null)
			{
				sb.Append(State.Time.Value.TimeOfDay.Hours + Results.spacer); //hour when recording made
				sb.Append(State.Time.Value.TimeOfDay.Minutes + Results.spacer); //hour when recording made
			}
			else
				sb.Append(Results.spacer + Results.spacer);
            sb.Append(State.TimeSlot + Results.spacer); //half hour when recording made

			// Removed WavMax calculation for performance reasons
			sb.Append(Results.spacer);//sb.Append(State.WavMax.ToString("F4") + Results.spacer);
            sb.Append(State.FrameNoise_dB.ToString("F4") + Results.spacer);
            sb.Append(State.Frame_SNR.ToString("F4") + Results.spacer);
            sb.Append(State.PowerMax.ToString("F3") + Results.spacer);
            sb.Append(State.PowerAvg.ToString("F3") + Results.spacer);

            //syllable distribution
            if ((categoryCount == 0) || (syllableDistribution==null))
                for (int f = 0; f < Results.analysisBandCount; f++) sb.Append("0  " + Results.spacer);
            else
                for (int f = 0; f < Results.analysisBandCount; f++) sb.Append(syllableDistribution[f].ToString() + Results.spacer);
            sb.Append(DataTools.Sum(syllableDistribution) + Results.spacer);

            //category distribution
            if ((categoryCount == 0) || (syllableDistribution == null))
                for (int f = 0; f < Results.analysisBandCount; f++) sb.Append("0  " + Results.spacer);
            else
                for (int f = 0; f < Results.analysisBandCount; f++) sb.Append(categoryDistribution[f].ToString() + Results.spacer);
            sb.Append(categoryCount + Results.spacer);

            //monotony index
            double sum = 0.0;
            double monotony = 0.0;
            if ((categoryCount == 0) || (syllableDistribution == null))
            {
                for (int f = 0; f < Results.analysisBandCount; f++) sb.Append("0.0000" + Results.spacer);
                sb.Append("0.0000" + Results.spacer);
            }
            else
            {
                for (int f = 0; f < Results.analysisBandCount; f++)
                {
                    if (categoryDistribution[f] == 0) monotony = 0.0;
                    else                              monotony = syllableDistribution[f] / (double)categoryDistribution[f];
                    sb.Append(monotony.ToString("F4") + Results.spacer);
                    sum += monotony;
                }
                double av = sum / (double)Results.analysisBandCount;
                sb.Append(av.ToString("F4") + Results.spacer);
            }
            sb.Append(Path.GetFileNameWithoutExtension(State.WavFilePath) + Results.spacer);
            
            return sb.ToString();
        }
    } //end class Sonogram
}
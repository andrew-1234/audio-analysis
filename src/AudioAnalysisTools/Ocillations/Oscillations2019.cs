// <copyright file="Oscillations2019.cs" company="QutEcoacoustics">
// All code in this file and all associated files are the copyright and property of the QUT Ecoacoustics Research Group (formerly MQUTeR, and formerly QUT Bioacoustics Research Group).
// </copyright>

namespace AudioAnalysisTools
{
    using System;
    using System.Collections.Generic;
    using AnalysisPrograms.Recognizers.Base;
    using AudioAnalysisTools.DSP;
    using AudioAnalysisTools.Events;
    using AudioAnalysisTools.StandardSpectrograms;
    using TowseyLibrary;

    /// <summary>
    /// NOTE: 26th October 2019.
    ///
    /// This class contains methods to detect oscillations in a the spectrogram of an audio signal.
    /// The method Execute() returns all info about oscillations in the passed spectrogram.
    /// </summary>
    public static class Oscillations2019
    {
        public static (List<EventCommon> OscEvents, List<Plot> Plots) GetComponentsWithOscillations(
            SpectrogramStandard spectrogram,
            OscillationParameters op,
            double? decibelThreshold,
            TimeSpan segmentStartOffset,
            string profileName)
        {
            double minDuration = op.MinDuration.Value;
            double maxDuration = op.MaxDuration.Value;
            int minHz = op.MinHertz.Value;
            int maxHz = op.MaxHertz.Value;
            double minOscFreq = op.MinOscillationFrequency.Value;
            double maxOscFreq = op.MaxOscillationFrequency.Value;
            double dctDuration = op.DctDuration;
            double dctThreshold = op.DctThreshold;
            double scoreThreshold = op.EventThreshold;
            int smoothingWindow = 5;

            // smooth the spectra in all time-frames.
            spectrogram.Data = MatrixTools.SmoothRows(spectrogram.Data, 3);

            // extract array of decibel values, frame averaged over required frequency band
            var decibelArray = SNR.CalculateFreqBandAvIntensity(spectrogram.Data, minHz, maxHz, spectrogram.NyquistFrequency);

            //DETECT OSCILLATIONS in the search band.
            var framesPerSecond = spectrogram.FramesPerSecond;
            DetectOscillations(
                decibelArray,
                framesPerSecond,
                decibelThreshold.Value,
                dctDuration,
                minOscFreq,
                maxOscFreq,
                dctThreshold,
                out var dctScores,
                out var oscFreq);

            // smooth the scores - window=11 has been the DEFAULT. Now letting user set this.
            dctScores = DataTools.filterMovingAverage(dctScores, smoothingWindow);

            var oscillationEvents = OscillationEvent.ConvertOscillationScores2Events(
                spectrogram,
                minDuration,
                maxDuration,
                minHz,
                maxHz,
                minOscFreq,
                maxOscFreq,
                dctScores,
                scoreThreshold,
                segmentStartOffset);

            var oscEvents = new List<EventCommon>();
            oscEvents.AddRange(oscillationEvents);

            // prepare plot of resultant decibel and score arrays.
            var plots = new List<Plot>();
            var plot1 = Plot.PreparePlot(decibelArray, $"{profileName} (Oscillations:{decibelThreshold:F0}db)", decibelThreshold.Value);
            plots.Add(plot1);
            var plot2 = Plot.PreparePlot(dctScores, $"{profileName} (Oscillation Event Score:{op.EventThreshold:F1})", op.EventThreshold);
            plots.Add(plot2);
            var plot3 = Plot.PreparePlot(oscFreq, $"{profileName} (Oscillation Rate:{30:F1}/s)", 30);
            plots.Add(plot3);

            return (oscEvents, plots);
        }

        /// <summary>
        /// Currently this method is called by only one species recognizer - LitoriaCaerulea.
        /// </summary>
        /// <param name="ipArray">an array of decibel values.</param>
        /// <param name="framesPerSecond">the frame rate.</param>
        /// <param name="decibelThreshold">Ignore frames below this threshold.</param>
        /// <param name="dctDuration">Duration in seconds of the required DCT.</param>
        /// <param name="minOscFreq">minimum oscillation frequency.</param>
        /// <param name="maxOscFreq">maximum oscillation frequency.</param>
        /// <param name="dctThreshold">Threshold for the maximum DCT coefficient.</param>
        /// <param name="dctScores">an array of dct scores.</param>
        /// <param name="oscFreq">an array of oscillation frequencies.</param>
        public static void DetectOscillations(
            double[] ipArray,
            double framesPerSecond,
            double decibelThreshold,
            double dctDuration,
            double minOscFreq,
            double maxOscFreq,
            double dctThreshold,
            out double[] dctScores,
            out double[] oscFreq)
        {
            int dctLength = (int)Math.Round(framesPerSecond * dctDuration);
            int minIndex = (int)(minOscFreq * dctDuration * 2); //multiply by 2 because index = Pi and not 2Pi
            int maxIndex = (int)(maxOscFreq * dctDuration * 2); //multiply by 2 because index = Pi and not 2Pi
            if (maxIndex > dctLength)
            {
                LoggedConsole.WriteWarnLine("MaxIndex > DCT length. Therefore set maxIndex = DCT length.");
                maxIndex = dctLength;
            }

            int length = ipArray.Length;
            dctScores = new double[length];
            oscFreq = new double[length];

            //set up the cosine coefficients
            double[,] cosines = DctMethods.Cosines(dctLength, dctLength);

            for (int r = 1; r < length - dctLength; r++)
            {
                // only stop if current location is a peak
                if (ipArray[r] < ipArray[r - 1] || ipArray[r] < ipArray[r + 1])
                {
                    continue;
                }

                // ... AND if current peak is above the decibel threhsold.
                if (ipArray[r] < decibelThreshold)
                {
                    continue;
                }

                // extract array and ready for DCT
                var dctArray = DataTools.Subarray(ipArray, r, dctLength);

                dctArray = DataTools.SubtractMean(dctArray);
                double[] dctCoefficient = MFCCStuff.CalculateCeptrum(dctArray, cosines);

                // convert to absolute values because not interested in negative values due to phase.
                for (int i = 0; i < dctLength; i++)
                {
                    dctCoefficient[i] = Math.Abs(dctCoefficient[i]);
                }

                // remove low freq oscillations from consideration
                int thresholdIndex = minIndex / 4;
                for (int i = 0; i < thresholdIndex; i++)
                {
                    dctCoefficient[i] = 0.0;
                }

                dctCoefficient = DataTools.normalise2UnitLength(dctCoefficient);

                int indexOfMaxValue = DataTools.GetMaxIndex(dctCoefficient);

                //mark DCT location with oscillation freq, only if oscillation freq is in correct range and amplitude
                if (indexOfMaxValue >= minIndex && indexOfMaxValue <= maxIndex && dctCoefficient[indexOfMaxValue] > dctThreshold)
                {
                    for (int i = 0; i < dctLength; i++)
                    {
                        if (dctScores[r + i] < dctCoefficient[indexOfMaxValue])
                        {
                            dctScores[r + i] = dctCoefficient[indexOfMaxValue];
                            oscFreq[r + i] = indexOfMaxValue / dctDuration / 2;
                        }
                    }
                }
            }
        }
    }
}
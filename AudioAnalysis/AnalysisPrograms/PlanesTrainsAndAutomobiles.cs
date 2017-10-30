﻿// <copyright file="PlanesTrainsAndAutomobiles.cs" company="QutEcoacoustics">
// All code in this file and all associated files are the copyright and property of the QUT Ecoacoustics Research Group (formerly MQUTeR, and formerly QUT Bioacoustics Research Group).
// </copyright>

namespace AnalysisPrograms
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Acoustics.Shared;
    using Acoustics.Shared.Contracts;
    using Acoustics.Shared.Extensions;
    using Acoustics.Tools;
    using Acoustics.Tools.Audio;
    using AnalysisBase;
    using AnalysisBase.Extensions;
    using AudioAnalysisTools;
    using AudioAnalysisTools.DSP;
    using AudioAnalysisTools.StandardSpectrograms;
    using AudioAnalysisTools.WavTools;
    using Production;
    using TowseyLibrary;

    [Obsolete("This recognizer is non functional. It's core should be ported to the new recognizer base immediately")]
    public class PlanesTrainsAndAutomobiles
    {
        public class Arguments : AnalyserArguments
        {
        }

        // CONSTANTS
        public const string AnalysisName = "Machine";
        public const int ResampleRate = 17640;
        //public const int RESAMPLE_RATE = 22050;
        //public const string imageViewer = @"C:\Program Files\Windows Photo Viewer\ImagingDevices.exe";
        public const string ImageViewer = @"C:\Windows\system32\mspaint.exe";

        public string DisplayName
        {
            get { return "Planes Trains And Automobiles"; }
        }

        public string Identifier
        {
            get { return "Towsey." + AnalysisName; }
        }

        [Obsolete("See https://github.com/QutBioacoustics/audio-analysis/issues/134")]
        public static void Dev(Arguments arguments)
        {
            var executeDev = arguments == null;
            if (executeDev)
            {
                //string recordingPath = @"C:\SensorNetworks\WavFiles\Human\DM420036_min465Speech.wav";
                //string recordingPath = @"C:\SensorNetworks\WavFiles\Machines\DM420036_min173Airplane.wav";
                //string recordingPath = @"C:\SensorNetworks\WavFiles\Machines\DM420036_min449Airplane.wav";
                //string recordingPath = @"C:\SensorNetworks\WavFiles\Machines\DM420036_min700Airplane.wav";
                string recordingPath = @"C:\SensorNetworks\WavFiles\Machines\KAPITI2-20100219-202900_Airplane.mp3";

                string configPath = @"C:\SensorNetworks\Output\Machines\Machine.cfg";
                string outputDir = @"C:\SensorNetworks\Output\Machines\";

                string title = "# FOR DETECTION OF PLANES, TRAINS AND AUTOMOBILES using CROSS-CORRELATION & FFT";
                string date = "# DATE AND TIME: " + DateTime.Now;
                LoggedConsole.WriteLine(title);
                LoggedConsole.WriteLine(date);
                LoggedConsole.WriteLine("# Output folder:  " + outputDir);
                LoggedConsole.WriteLine("# Recording file: " + Path.GetFileName(recordingPath));
                var diOutputDir = new DirectoryInfo(outputDir);

                Log.Verbosity = 1;
                int startMinute = 0;
                int durationSeconds = 0; //set zero to get entire recording
                var tsStart = new TimeSpan(0, startMinute, 0); //hours, minutes, seconds
                var tsDuration = new TimeSpan(0, 0, durationSeconds); //hours, minutes, seconds
                var segmentFileStem = Path.GetFileNameWithoutExtension(recordingPath);
                var segmentFName = string.Format("{0}_{1}min.wav", segmentFileStem, startMinute);
                var sonogramFname = string.Format("{0}_{1}min.png", segmentFileStem, startMinute);
                var eventsFname = string.Format("{0}_{1}min.{2}.Events.csv", segmentFileStem, startMinute, "Towsey." + AnalysisName);
                var indicesFname = string.Format("{0}_{1}min.{2}.Indices.csv", segmentFileStem, startMinute, "Towsey." + AnalysisName);

                arguments = new Arguments
                {
                    Source = recordingPath.ToFileInfo(),
                    Config = configPath.ToFileInfo(),
                    Output = outputDir.ToDirectoryInfo(),
                    Start = tsStart.TotalSeconds,
                    Duration = tsDuration.TotalSeconds,
                };
            }

            Execute(arguments);
        }

        /// <summary>
        /// A WRAPPER AROUND THE analyser.Analyze(analysisSettings) METHOD
        /// To be called as an executable with command line arguments.
        /// </summary>
        public static void Execute(Arguments arguments)
        {
            Contract.Requires(arguments != null);

            TimeSpan tsStart = TimeSpan.FromSeconds(arguments.Start ?? 0);
            TimeSpan tsDuration = TimeSpan.FromSeconds(arguments.Duration ?? 0);

            //EXTRACT THE REQUIRED RECORDING SEGMENT
            FileInfo tempF = TempFileHelper.NewTempFile(arguments.Output);
            AudioUtilityModifiedInfo preparedFile;
            if (tsDuration == TimeSpan.Zero)   //Process entire file
            {
                preparedFile = AudioFilePreparer.PrepareFile(arguments.Source, tempF, new AudioUtilityRequest { TargetSampleRate = ResampleRate }, arguments.Output);
                //var fiSegment = AudioFilePreparer.PrepareFile(diOutputDir, fiSourceFile, , Human2.RESAMPLE_RATE);
            }
            else
            {
                preparedFile = AudioFilePreparer.PrepareFile(arguments.Source, tempF, new AudioUtilityRequest { TargetSampleRate = ResampleRate, OffsetStart = tsStart, OffsetEnd = tsStart.Add(tsDuration) }, arguments.Output);
                //var fiSegmentOfSourceFile = AudioFilePreparer.PrepareFile(diOutputDir, new FileInfo(recordingPath), MediaTypes.MediaTypeWav, TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(3), RESAMPLE_RATE);
            }

            var (analysisSettings, segmentSettings) = arguments.ToAnalysisSettings(
                sourceSegment: preparedFile.SourceInfo.ToSegment(),
                preparedSegment: preparedFile.TargetInfo.ToSegment());

            //DO THE ANALYSIS
            //#############################################################################################################################################
            IAnalyser2 analyser = null; /* BROKEN ! new PlanesTrainsAndAutomobiles();*/
            AnalysisResult2 result = analyser.Analyze(analysisSettings, segmentSettings);
            //DataTable dt = result.Data;
            //#############################################################################################################################################

            throw new NotImplementedException("intentionally broken in updates");
            //ADD IN ADDITIONAL INFO TO RESULTS TABLE
            /*if (dt != null)
            {
                AnalysisTemplate.AddContext2Table(dt, tsStart, result.AudioDuration);
                CsvTools.DataTable2CSV(dt, analysisSettings.SegmentSettings.SegmentEventsFile.FullName);
                //DataTableTools.WriteTable(dt);
            }*/
        }

        public AnalysisResult Analyse<T>(AnalysisSettings analysisSettings, SegmentSettings<T> segmentSettings)
        {
            var configuration = new ConfigDictionary(analysisSettings.ConfigFile.FullName);
            Dictionary<string, string> configDict = configuration.GetTable();
            var fiAudioF = segmentSettings.SegmentAudioFile;
            var diOutputDir = segmentSettings.SegmentOutputDirectory;

            var analysisResults = new AnalysisResult();
            analysisResults.AnalysisIdentifier = this.Identifier;
            analysisResults.SettingsUsed = analysisSettings;
            analysisResults.Data = null;

            //######################################################################
            var results = Analysis(fiAudioF, configDict, segmentSettings.SegmentStartOffset);
            //######################################################################

            if (results == null) return analysisResults; //nothing to process
            var sonogram = results.Item1;
            var hits = results.Item2;
            var scores = results.Item3;
            var predictedEvents = results.Item4;
            var recordingTimeSpan = results.Item5;
            analysisResults.AudioDuration = recordingTimeSpan;

            DataTable dataTable = null;

            if ((predictedEvents != null) && (predictedEvents.Count != 0))
            {
                string analysisName = configDict[AnalysisKeys.AnalysisName];
                string fName = Path.GetFileNameWithoutExtension(fiAudioF.Name);
                foreach (AcousticEvent ev in predictedEvents)
                {
                    ev.FileName = fName;
                    ev.Name = analysisName;
                    ev.SegmentDurationSeconds = recordingTimeSpan.TotalSeconds;
                }
                //write events to a data table to return.
                dataTable = WriteEvents2DataTable(predictedEvents);
                string sortString = AnalysisKeys.EventStartSec + " ASC";
                dataTable = DataTableTools.SortTable(dataTable, sortString); //sort by start time before returning

            }

            if (analysisSettings.AnalysisDataSaveBehavior)
            {
                CsvTools.DataTable2CSV(dataTable, segmentSettings.SegmentEventsFile.FullName);
            }

            if (analysisSettings.AnalysisDataSaveBehavior)
            {
                double scoreThreshold = 0.1;
                TimeSpan unitTime = TimeSpan.FromSeconds(60); //index for each time span of i minute
                var indicesDT = this.ConvertEvents2Indices(dataTable, unitTime, recordingTimeSpan, scoreThreshold);
                CsvTools.DataTable2CSV(indicesDT, segmentSettings.SegmentSummaryIndicesFile.FullName);
            }

            //save image of sonograms
            if (analysisSettings.AnalysisImageSaveBehavior.ShouldSave(analysisResults.Data.Rows.Count))
            {
                string imagePath = segmentSettings.SegmentImageFile.FullName;
                double eventThreshold = 0.1;
                Image image = DrawSonogram(sonogram, hits, scores, predictedEvents, eventThreshold);
                image.Save(imagePath, ImageFormat.Png);
            }

            analysisResults.Data = dataTable;
            analysisResults.ImageFile = segmentSettings.SegmentImageFile;
            analysisResults.AudioDuration = recordingTimeSpan;
            //result.DisplayItems = { { 0, "example" }, { 1, "example 2" }, }
            //result.OutputFiles = { { "exmaple file key", new FileInfo("Where's that file?") } }
            return analysisResults;
        } //Analyze()

        /// <summary>
        /// ################ THE KEY ANALYSIS METHOD
        /// Returns a DataTable
        /// </summary>
        /// <param name="fiSegmentOfSourceFile"></param>
        /// <param name="configDict"></param>
        /// <param name="segmentStartOffset"></param>
        /// <param name="diOutputDir"></param>
        public static Tuple<BaseSonogram, double[,], Plot, List<AcousticEvent>, TimeSpan> Analysis(FileInfo fiSegmentOfSourceFile, Dictionary<string, string> configDict, TimeSpan segmentStartOffset)
        {
            string analysisName = configDict[AnalysisKeys.AnalysisName];
            int minFormantgap = int.Parse(configDict[AnalysisKeys.MinFormantGap]);
            int maxFormantgap = int.Parse(configDict[AnalysisKeys.MaxFormantGap]);
            int minHz = int.Parse(configDict[AnalysisKeys.MinHz]);
            double intensityThreshold = double.Parse(configDict[AnalysisKeys.IntensityThreshold]); //in 0-1
            double minDuration = double.Parse(configDict[AnalysisKeys.MinDuration]);  // seconds
            int frameLength = 2048;
            if (configDict.ContainsKey(AnalysisKeys.FrameLength))
                frameLength = int.Parse(configDict[AnalysisKeys.FrameLength]);
            double windowOverlap = 0.0;
            if (frameLength == 1024) //this is to make adjustment with other harmonic methods that use frame length = 1024
            {
                frameLength = 2048;
                windowOverlap = 0.5;
            }

            AudioRecording recording = new AudioRecording(fiSegmentOfSourceFile.FullName);
            if (recording == null)
            {
                LoggedConsole.WriteLine("AudioRecording == null. Analysis not possible.");
                return null;
            }

            //#############################################################################################################################################
            var results = DetectHarmonics(recording, intensityThreshold, minHz, minFormantgap, maxFormantgap, minDuration, frameLength, windowOverlap, segmentStartOffset); //uses XCORR and FFT
            //#############################################################################################################################################

            var sonogram = results.Item1;
            var hits = results.Item2;
            var scores = results.Item3;
            var predictedEvents = results.Item4;
            foreach (AcousticEvent ev in predictedEvents)
            {
                ev.FileName = recording.BaseName;
                ev.Name = analysisName;
            }

            TimeSpan tsRecordingtDuration = recording.Duration;

            Plot plot = new Plot(AnalysisName, scores, intensityThreshold);
            return Tuple.Create(sonogram, hits, plot, predictedEvents, tsRecordingtDuration);
        } //Analysis()

        public static Tuple<BaseSonogram, double[,], double[], List<AcousticEvent>> DetectHarmonics(
            AudioRecording recording,
            double intensityThreshold,
            int minHz,
            int minFormantgap,
            int maxFormantgap,
            double minDuration,
            int windowSize,
            double windowOverlap,
            TimeSpan segmentStartOffset)
        {
            //i: MAKE SONOGRAM
            int numberOfBins = 32;
            double binWidth = recording.SampleRate / (double)windowSize;
            int sr = recording.SampleRate;
            double frameDuration = windowSize / (double)sr;           // Duration of full frame or window in seconds
            double frameOffset = frameDuration * (1 - windowOverlap); //seconds between starts of consecutive frames
            double framesPerSecond = 1 / frameOffset;
            //double framesPerSecond = sr / (double)windowSize;
            //int frameOffset = (int)(windowSize * (1 - overlap));
            //int frameCount = (length - windowSize + frameOffset) / frameOffset;

            double epsilon = Math.Pow(0.5, recording.BitsPerSample - 1);
            var results2 = DSP_Frames.ExtractEnvelopeAndAmplSpectrogram(recording.WavReader.Samples, sr, epsilon, windowSize, windowOverlap);
            double[] avAbsolute = results2.Average; //average absolute value over the minute recording
            //double[] envelope = results2.Item2;
            double[,] matrix = results2.AmplitudeSpectrogram;  //amplitude spectrogram. Note that column zero is the DC or average energy value and can be ignored.
            double windowPower = results2.WindowPower;

            //window    sr          frameDuration   frames/sec  hz/bin  64frameDuration hz/64bins       hz/128bins
            // 1024     22050       46.4ms          21.5        21.5    2944ms          1376hz          2752hz
            // 1024     17640       58.0ms          17.2        17.2    3715ms          1100hz          2200hz
            // 2048     17640       116.1ms          8.6         8.6    7430ms           551hz          1100hz

            //the Xcorrelation-FFT technique requires number of bins to scan to be power of 2.
            //assuming sr=17640 and window=1024, then  64 bins span 1100 Hz above the min Hz level. i.e. 500 to 1600
            //assuming sr=17640 and window=1024, then 128 bins span 2200 Hz above the min Hz level. i.e. 500 to 2700
            int minBin = (int)Math.Round(minHz / binWidth);
            int maxHz = (int)Math.Round(minHz + (numberOfBins * binWidth));

            int rowCount = matrix.GetLength(0);
            int colCount = matrix.GetLength(1);
            int maxbin = minBin + numberOfBins;
            double[,] subMatrix = MatrixTools.Submatrix(matrix, 0, (minBin + 1), (rowCount - 1), maxbin);

            //ii: DETECT HARMONICS
            int zeroBinCount = 5; //to remove low freq content which dominates the spectrum
            var results = CrossCorrelation.DetectBarsInTheRowsOfaMatrix(subMatrix, intensityThreshold, zeroBinCount);
            double[] intensity = results.Item1;     //an array of periodicity scores
            double[] periodicity = results.Item2;

            //transfer periodicity info to a hits matrix.
            //intensity = DataTools.filterMovingAverage(intensity, 3);
            double[] scoreArray = new double[intensity.Length];
            var hits = new double[rowCount, colCount];
            for (int r = 0; r < rowCount; r++)
            {
                double relativePeriod = periodicity[r] / numberOfBins / 2;
                if (intensity[r] > intensityThreshold)
                {
                    for (int c = minBin; c < maxbin; c++)
                    {
                        hits[r, c] = relativePeriod;
                    }
                }
                double herzPeriod = periodicity[r] * binWidth;
                if ((herzPeriod > minFormantgap) && (herzPeriod < maxFormantgap))
                    scoreArray[r] = 2 * intensity[r] * intensity[r];    //enhance high score wrt low score.
            }
            scoreArray = DataTools.filterMovingAverage(scoreArray, 11);

            //iii: CONVERT TO ACOUSTIC EVENTS
            double maxDuration = 100000.0; //abitrary long number - do not want to restrict duration of machine noise
            List<AcousticEvent> predictedEvents = AcousticEvent.ConvertScoreArray2Events(
                scoreArray,
                minHz,
                maxHz,
                framesPerSecond,
                binWidth,
                intensityThreshold,
                minDuration,
                maxDuration,
                segmentStartOffset);
            hits = null;

            //set up the songogram to return. Use the existing amplitude sonogram
            int bitsPerSample = recording.WavReader.BitsPerSample;
            TimeSpan duration = recording.Duration;
            //NoiseReductionType nrt = SNR.Key2NoiseReductionType("NONE");
            NoiseReductionType nrt = SNR.KeyToNoiseReductionType("STANDARD");

            var sonogram = (BaseSonogram)SpectrogramStandard.GetSpectralSonogram(recording.BaseName, windowSize, windowOverlap, bitsPerSample, windowPower, sr, duration, nrt, matrix);

            sonogram.DecibelsNormalised = new double[rowCount];
            for (int i = 0; i < rowCount; i++) //foreach frame or time step
            {
                sonogram.DecibelsNormalised[i] = 2 * Math.Log10(avAbsolute[i]);
            }
            sonogram.DecibelsNormalised = DataTools.normalise(sonogram.DecibelsNormalised);

            return Tuple.Create(sonogram, hits, scoreArray, predictedEvents);
        }//end Execute_HDDetect

        static Image DrawSonogram(BaseSonogram sonogram, double[,] hits, Plot scores, List<AcousticEvent> predictedEvents, double eventThreshold)
        {
            Image_MultiTrack image = new Image_MultiTrack(sonogram.GetImage());

            //System.Drawing.Image img = sonogram.GetImage(doHighlightSubband, add1kHzLines);
            //img.Save(@"C:\SensorNetworks\temp\testimage1.png", System.Drawing.Imaging.ImageFormat.Png);

            //Image_MultiTrack image = new Image_MultiTrack(img);
            image.AddTrack(Image_Track.GetTimeTrack(sonogram.Duration, sonogram.FramesPerSecond));
            image.AddTrack(Image_Track.GetSegmentationTrack(sonogram));
            if (scores != null) image.AddTrack(Image_Track.GetNamedScoreTrack(scores.data, 0.0, 1.0, scores.threshold, scores.title));
            //if (hits != null) image.OverlayRedTransparency(hits);
            if (hits != null) image.OverlayRainbowTransparency(hits);
            if (predictedEvents.Count > 0) image.AddEvents(predictedEvents, sonogram.NyquistFrequency, sonogram.Configuration.FreqBinCount, sonogram.FramesPerSecond);
            return image.GetImage();
        } //DrawSonogram()

        public static DataTable WriteEvents2DataTable(List<AcousticEvent> predictedEvents)
        {
            if (predictedEvents == null) return null;
            string[] headers = { AnalysisKeys.EventCount,
                                 AnalysisKeys.EventStartMin,
                                 AnalysisKeys.EventStartSec,
                                 AnalysisKeys.EventStartAbs,
                                 AnalysisKeys.KeySegmentDuration,
                                 AnalysisKeys.EventDuration,
                                 AnalysisKeys.EventIntensity,
                                 AnalysisKeys.EventName,
                                 AnalysisKeys.EventScore,
                                 AnalysisKeys.EventNormscore,

                               };
            //                   1                2               3              4                5              6               7              8
            Type[] types = { typeof(int), typeof(double), typeof(double), typeof(double), typeof(double), typeof(double), typeof(double), typeof(string),
                             typeof(double), typeof(double) };

            var dataTable = DataTableTools.CreateTable(headers, types);
            if (predictedEvents.Count == 0) return dataTable;

            foreach (var ev in predictedEvents)
            {
                DataRow row = dataTable.NewRow();
                row[AnalysisKeys.EventStartSec] = (double)ev.TimeStart;  //EvStartSec
                row[AnalysisKeys.EventDuration] = (double)ev.EventDurationSeconds;   //duratio in seconds
                row[AnalysisKeys.EventIntensity] = (double)ev.kiwi_intensityScore;   //
                row[AnalysisKeys.EventName] = (string)ev.Name;   //
                row[AnalysisKeys.EventNormscore] = (double)ev.ScoreNormalised;
                row[AnalysisKeys.EventScore] = (double)ev.Score;      //Score
                dataTable.Rows.Add(row);
            }
            return dataTable;
        }

        /// <summary>
        /// Converts a DataTable of events to a datatable where one row = one minute of indices
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public DataTable ConvertEvents2Indices(DataTable dt, TimeSpan unitTime, TimeSpan sourceDuration, double scoreThreshold)
        {
            double units = sourceDuration.TotalSeconds / unitTime.TotalSeconds;
            int unitCount = (int)(units / 1);   //get whole minutes
            if (units % 1 > 0.0) unitCount += 1; //add fractional minute
            int[] eventsPerUnitTime = new int[unitCount]; //to store event counts
            int[] bigEvsPerUnitTime = new int[unitCount]; //to store counts of high scoring events

            foreach (DataRow ev in dt.Rows)
            {
                double eventStart = (double)ev[AnalysisKeys.EventStartAbs];
                double eventScore = (double)ev[AnalysisKeys.EventNormscore];
                int timeUnit = (int)(eventStart / unitTime.TotalSeconds);
                if (eventScore != 0.0) eventsPerUnitTime[timeUnit]++;
                if (eventScore > scoreThreshold) bigEvsPerUnitTime[timeUnit]++;
            }

            string[] headers = { AnalysisKeys.KeyStartMinute, AnalysisKeys.EventTotal, ("#Ev>" + scoreThreshold) };
            Type[] types = { typeof(int), typeof(int), typeof(int) };
            var newtable = DataTableTools.CreateTable(headers, types);

            for (int i = 0; i < eventsPerUnitTime.Length; i++)
            {
                int unitID = (int)(i * unitTime.TotalMinutes);
                newtable.Rows.Add(unitID, eventsPerUnitTime[i], bigEvsPerUnitTime[i]);
            }
            return newtable;
        }

        public static void AddContext2Table(DataTable dt, TimeSpan segmentStartMinute, TimeSpan recordingTimeSpan)
        {
            if (!dt.Columns.Contains(AnalysisKeys.KeySegmentDuration)) dt.Columns.Add(AnalysisKeys.KeySegmentDuration, typeof(double));
            if (!dt.Columns.Contains(AnalysisKeys.EventStartAbs)) dt.Columns.Add(AnalysisKeys.EventStartAbs, typeof(double));
            if (!dt.Columns.Contains(AnalysisKeys.EventStartMin)) dt.Columns.Add(AnalysisKeys.EventStartMin, typeof(double));
            double start = segmentStartMinute.TotalSeconds;
            foreach (DataRow row in dt.Rows)
            {
                row[AnalysisKeys.KeySegmentDuration] = recordingTimeSpan.TotalSeconds;
                row[AnalysisKeys.EventStartAbs] = start + (double)row[AnalysisKeys.EventStartSec];
                row[AnalysisKeys.EventStartMin] = start;
            }
        } //AddContext2Table()

        public Tuple<DataTable, DataTable> ProcessCsvFile(FileInfo fiCsvFile, FileInfo fiConfigFile)
        {
            DataTable dt = CsvTools.ReadCSVToTable(fiCsvFile.FullName, true); //get original data table
            if ((dt == null) || (dt.Rows.Count == 0)) return null;
            //get its column headers
            var dtHeaders = new List<string>();
            var dtTypes = new List<Type>();
            foreach (DataColumn col in dt.Columns)
            {
                dtHeaders.Add(col.ColumnName);
                dtTypes.Add(col.DataType);
            }

            List<string> displayHeaders = null;
            //check if config file contains list of display headers
            if (fiConfigFile != null)
            {
                var configuration = new ConfigDictionary(fiConfigFile.FullName);
                Dictionary<string, string> configDict = configuration.GetTable();
                if (configDict.ContainsKey(AnalysisKeys.DisplayColumns))
                    displayHeaders = configDict[AnalysisKeys.DisplayColumns].Split(',').ToList();
            }
            //if config file does not exist or does not contain display headers then use the original headers
            if (displayHeaders == null) displayHeaders = dtHeaders; //use existing headers if user supplies none.

            //now determine how to display tracks in display datatable
            Type[] displayTypes = new Type[displayHeaders.Count];
            bool[] canDisplay = new bool[displayHeaders.Count];
            for (int i = 0; i < displayTypes.Length; i++)
            {
                displayTypes[i] = typeof(double);
                canDisplay[i] = false;
                if (dtHeaders.Contains(displayHeaders[i])) canDisplay[i] = true;
            }

            DataTable table2Display = DataTableTools.CreateTable(displayHeaders.ToArray(), displayTypes);
            foreach (DataRow row in dt.Rows)
            {
                DataRow newRow = table2Display.NewRow();
                for (int i = 0; i < canDisplay.Length; i++)
                {
                    if (canDisplay[i]) newRow[displayHeaders[i]] = row[displayHeaders[i]];
                    else newRow[displayHeaders[i]] = 0.0;
                }
                table2Display.Rows.Add(newRow);
            }

            //order the table if possible
            if (dt.Columns.Contains(AnalysisKeys.EventStartAbs))
            {
                dt = DataTableTools.SortTable(dt, AnalysisKeys.EventStartAbs + " ASC");
            }
            else if (dt.Columns.Contains(AnalysisKeys.EventCount))
            {
                dt = DataTableTools.SortTable(dt, AnalysisKeys.EventCount + " ASC");
            }
            else if (dt.Columns.Contains(AnalysisKeys.KeyRankOrder))
            {
                dt = DataTableTools.SortTable(dt, AnalysisKeys.KeyRankOrder + " ASC");
            }
            else if (dt.Columns.Contains(AnalysisKeys.KeyStartMinute))
            {
                dt = DataTableTools.SortTable(dt, AnalysisKeys.KeyStartMinute + " ASC");
            }

            table2Display = NormaliseColumnsOfDataTable(table2Display);
            return Tuple.Create(dt, table2Display);
        } // ProcessCsvFile()

        /// <summary>
        /// takes a data table of indices and normalises column values to values in [0,1].
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DataTable NormaliseColumnsOfDataTable(DataTable dt)
        {
            string[] headers = DataTableTools.GetColumnNames(dt);
            string[] newHeaders = new string[headers.Length];

            List<double[]> newColumns = new List<double[]>();

            for (int i = 0; i < headers.Length; i++)
            {
                double[] values = DataTableTools.Column2ArrayOfDouble(dt, headers[i]); //get list of values
                if ((values == null) || (values.Length == 0)) continue;

                double min = 0;
                double max = 1;
                if (headers[i].Equals(AnalysisKeys.KeyAvSignalAmplitude))
                {
                    min = -50;
                    max = -5;
                    newColumns.Add(DataTools.NormaliseInZeroOne(values, min, max));
                    newHeaders[i] = headers[i] + "  (-50..-5dB)";
                }
                else //default is to NormaliseMatrixValues in [0,1]
                {
                    newColumns.Add(DataTools.normalise(values)); //NormaliseMatrixValues all values in [0,1]
                    newHeaders[i] = headers[i];
                }
            } //for loop

            //convert type int to type double due to normalisation
            Type[] types = new Type[newHeaders.Length];
            for (int i = 0; i < newHeaders.Length; i++) types[i] = typeof(double);
            var processedtable = DataTableTools.CreateTable(newHeaders, types, newColumns);
            return processedtable;
        }

        public string DefaultConfiguration
        {
            get
            {
                return string.Empty;
            }
        }

        public AnalysisSettings DefaultSettings
        {
            get
            {
                return new AnalysisSettings
                {
                    AnalysisMaxSegmentDuration = TimeSpan.FromMinutes(1),
                    AnalysisMinSegmentDuration = TimeSpan.FromSeconds(30),
                    SegmentMediaType = MediaTypes.MediaTypeWav,
                    SegmentOverlapDuration = TimeSpan.Zero,
                };
            }
        }

    } //end class PlanesTrainsAndAutomobiles
}

// <copyright file="CisticolaTests.cs" company="QutEcoacoustics">
// All code in this file and all associated files are the copyright and property of the QUT Ecoacoustics Research Group (formerly MQUTeR, and formerly QUT Bioacoustics Research Group).
// </copyright>

namespace Acoustics.Test.AnalysisPrograms.Recognizers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Acoustics.Test.TestHelpers;
    using Acoustics.Tools.Wav;
    using global::AnalysisPrograms.Recognizers;
    using global::AnalysisPrograms.SourcePreparers;
    using global::AudioAnalysisTools.Events;
    using global::AudioAnalysisTools.Events.Types;
    using global::AudioAnalysisTools.WavTools;
    using global::TowseyLibrary;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The canonical recording used for this recognizer is a 30 second recording containing five Pipit calls and a number of Cisticola and other bird calls.
    /// It was recorded in Narrabri region and forms part of the Cotton Project data set.
    /// </summary>
    [TestClass]
    public class CisticolaTests : OutputDirectoryTest
    {
        /// <summary>
        /// The canonical recording used for this recognizer is a 31 second recording .
        /// </summary>
        private static readonly FileInfo TestAsset = PathHelper.ResolveAsset("Recordings", "ms1_2559_630118_20170402_075841_30_0.wav");
        //private static readonly FileInfo TestAsset = new FileInfo(@"C:\Ecoacoustics\WavFiles\TestNoiseRecordings\Cisticola\ms1_2559_628362_20170408_074841_30_0.wav");

        private static readonly FileInfo ConfigFile = PathHelper.ResolveConfigFile("RecognizerConfigFiles", "Towsey.CisticolaExilis.yml");
        private static readonly CisticolaExilis Recognizer = new CisticolaExilis();

        [TestMethod]
        public void TestRecognizer()
        {
            var config = Recognizer.ParseConfig(ConfigFile);
            int resampleRate = config.ResampleRate.Value;
            string opDir = this.TestOutputDirectory.FullName;
            string opFileName = "tempFile.wav";
            var recording = AudioRecording.GetAudioRecording(TestAsset, resampleRate, opDir, opFileName);

            var results = Recognizer.Recognize(
                audioRecording: recording,
                config: config,
                segmentStartOffset: TimeSpan.Zero,
                getSpectralIndexes: null,
                outputDirectory: this.TestOutputDirectory,
                imageWidth: null);

            var events = results.NewEvents;
            var scoreTrack = results.ScoreTrack;
            var plots = results.Plots;
            var sonogram = results.Sonogram;

            this.SaveTestOutput(
                outputDirectory => GenericRecognizer.SaveDebugSpectrogram(results, null, outputDirectory, Recognizer.SpeciesName));

            // this test returns with two false-negatives.
            // It is possible to get all positive events but creates excess FPs on test data.
            Assert.AreEqual(17, events.Count);
            Assert.IsNull(scoreTrack);
            Assert.AreEqual(1, plots.Count);
            Assert.AreEqual(3747, sonogram.FrameCount);

            Assert.IsInstanceOfType(events[3], typeof(CompositeEvent));

            var secondEvent = (CompositeEvent)events[3];

            Assert.AreEqual(7.28, secondEvent.EventStartSeconds);
            Assert.AreEqual(7.432, secondEvent.EventEndSeconds);
            Assert.AreEqual(2542, secondEvent.LowFrequencyHertz);
            Assert.AreEqual(3100, secondEvent.HighFrequencyHertz);
            Assert.AreEqual(17.91649081319, secondEvent.Score, TestHelper.AllowedDelta);
            Assert.AreEqual(0.07765486486, secondEvent.ScoreNormalized, TestHelper.AllowedDelta);
        }
    }
}
// <copyright file="Track.cs" company="QutEcoacoustics">
// All code in this file and all associated files are the copyright and property of the QUT Ecoacoustics Research Group (formerly MQUTeR, and formerly QUT Bioacoustics Research Group).
// </copyright>

namespace AudioAnalysisTools.Events.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Acoustics.Shared;
    using AudioAnalysisTools.Events.Drawing;
    using SixLabors.ImageSharp.PixelFormats;
    using SixLabors.ImageSharp.Processing;

    public class Track : ITrack
    {
        private readonly UnitConverters converter;

        /// <summary>
        /// Initializes a new instance of the <see cref="Track"/> class.
        /// Constructor.
        /// </summary>
        public Track(UnitConverters converter)
        {
            this.converter = converter;
            this.Points = new SortedSet<ISpectralPoint>();
        }

        public int PointCount => this.Points.Count;

        public double StartTimeSeconds => this.converter.SegmentStartOffset + this.Points.Min(x => x.Seconds.Minimum);

        public double EndTimeSeconds => this.converter.SegmentStartOffset + this.Points.Max(x => x.Seconds.Maximum);

        public ISet<ISpectralPoint> Points { get; }

        public double TrackDurationSeconds => this.EndTimeSeconds - this.StartTimeSeconds;

        public double LowFreqHertz => this.Points.Min(x => x.Hertz.Minimum);

        public double HighFreqHertz => this.Points.Max(x => x.Hertz.Maximum);

        public double TrackBandWidthHertz => this.HighFreqHertz - this.LowFreqHertz;

        /// <summary>
        /// Gets the frequency of the first point in the track.
        /// Where there is more than one frequency in this first frame, returns the lowest frequency.
        /// </summary>
        public double StartFrequency
        {
            get
            {
                var startTime = this.StartTimeSeconds;
                return this.Points.Where(x => x.Seconds.Contains(startTime)).Min(y => y.Hertz.Minimum);
            }
        }

        /// <summary>
        /// Adds a new point to track given the fram, freq bin and amplitude.
        /// </summary>
        /// <param name="frame">The frame number.</param>
        /// <param name="bin">The freq bin number.</param>
        /// <param name="amplitude">The amplitude at given point.</param>
        public void SetPoint(int frame, int bin, double amplitude)
        {
            var secondsStart = this.converter.GetStartTimeInSecondsOfFrame(frame);

            var hertzLow = this.converter.GetHertzFromFreqBin(bin);

            var point = new SpectralPoint(
                (secondsStart, secondsStart + this.converter.SecondsPerFrame),
                (hertzLow, hertzLow + this.converter.HertzPerFreqBin),
                amplitude);

            this.Points.Add(point);
        }

        //public int GetStartFrame()
        //{
        //    return this.Points x => x.Seconds.Min();
        //}

        //public int GetEndFrame()
        //{
        //    return this.frameIds.Max();
        //}

        //public int GetTrackFrameCount()
        //{
        //    return this.frameIds.Max() - this.frameIds.Min() + 1;
        //}

        //public int GetBottomFreqBin()
        //{
        //    return this.freqBinIds.Min();
        //}

        //public int GetTopFreqBin()
        //{
        //    return this.freqBinIds.Max();
        //}

        //public int GetTrackFreqBinCount()
        //{
        //    return this.freqBinIds.Max() - this.freqBinIds.Min() + 1;
        //}

        /// <summary>
        /// Returns an array that has the same number of time frames as the track.
        /// Each element contains the highest frequency (Hertz) for that time frame.
        /// NOTE: For tracks that include extreme frequency modulation (e.g. clicks and vertical tracks),
        ///       this method returns the highest frequency value in each time frame.
        /// </summary>
        /// <returns>An array of Hertz values.</returns>
        public double[] GetTrackAsSequenceOfHertzValues()
        {
            //TODO
            throw new NotImplementedException("Method not implemented.");
        }

        /// <summary>
        /// Returns an array of Hertz difference values.
        /// The array has length one less than the number of dicrete time frames in the track.
        /// </summary>
        /// <returns>An array of Hertz difference values.</returns>
        public double[] GetTrackFrequencyProfile()
        {
            // get points, group by start bucket, order by grouped key (start bucket) and then provide sequence of windowed pairs
            var sorted = this
                .Points
                .GroupBy(g => g.Seconds)
                .OrderBy(x => x.Key)
                .Windowed(2)
                .Select(pointPair =>
                {
                    var firstPoints = pointPair[0];
                    var secondPoints = pointPair[1];

                    var firstMaxFrequency = firstPoints.Max(y => y.Hertz.Minimum);
                    var secondMaxFrequency = secondPoints.Max(y => y.Hertz.Minimum);

                    var delta = secondMaxFrequency - firstMaxFrequency;
                    return delta;
                })
                .ToArray();
            return sorted;
        }

        /*
        /// <summary>
        /// Returns the maximum amplitude in each time frame.
        /// </summary>
        public double[] GetAmplitudeOverTimeFrames()
        {
            var frameCount = this.GetTrackFrameCount();
            int startFrame = this.GetStartFrame();
            var amplitudeArray = new double[frameCount];
            // add in amplitude values
            for (int i = 0; i < this.amplitudeSequence.Count; i++)
            {
                int elapsedFrames = this.frameIds[i] - startFrame;
                if (amplitudeArray[elapsedFrames] < this.amplitudeSequence[i])
                {
                    amplitudeArray[elapsedFrames] = this.amplitudeSequence[i];
                }
            }
            return amplitudeArray;
        }
        */

        /// <summary>
        /// Draws the track on an image given by its processing context.
        /// </summary>
        public void Draw(IImageProcessingContext graphics, EventRenderingOptions options)
        {
            ((IPointData)this).DrawPointsAsPath(graphics, options);
        }
    }
}
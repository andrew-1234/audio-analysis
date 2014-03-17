﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics;

namespace QutBioacosutics.Xie
{
    using MathNet.Numerics.LinearAlgebra.Single;
    using TowseyLib;

    class ExtractTracks
    {
        public double[,] GetTracks(double[,] matrix, double binToreance, int frameThreshold, int duraionThreshold, double trackThreshold)
        {
            matrix = MatrixTools.MatrixRotate90Anticlockwise(matrix);

            var row = matrix.GetLength(0);
            var column = matrix.GetLength(1);

            double binToreanceStable = binToreance;
            // save local peaks to an array of list

            var pointList = new List<Peak>[column];
            
            for (int j = 0; j < column; j++)
            {
                var Points = new List<Peak>();            
                for (int i = 0; i < row; i++)
                {
                    var point = new Peak();
                    if (matrix[i, j] > 0)
                    {
                        point.Y = i;
                        point.X = j;
                        point.Amplitude = matrix[i, j];
                        Points.Add(point);
                    }                                      
                }

                pointList[j] = Points;

            }

            // use neareast distance to form tracks of the first two columns
                        
            var shortTrackList = new List<Track>();
            var longTrackList = new List<Track>();
            var closedTrackList = new List<Track>();

            var longTrackXList = new List<List<double>>();
            var longTrackYList = new List<List<double>>();
            //var longTrackAmplitudeList = new List<double>();


            //var longTrackAmplitude = new List<double>();


            var indexI = new List<int>();
            var indexJ = new List<int>();

            // save nearest peaks into longTracks
            for (int i = 0; i < pointList[0].Count; i++)
            {
                binToreance = binToreanceStable;
                for(int j = 0; j < pointList[1].Count; j++)
                {
                    if (Math.Abs(pointList[0][i].Y - pointList[1][j].Y) < binToreance)
                    {
                        indexI.Add(i);
                        indexJ.Add(j);
                        binToreance = Math.Abs(pointList[0][i].Y - pointList[1][j].Y);
                    }
                }                
            }

            for (int i = 0; i < indexI.Count; i++)
            {

                var longTrackX = new List<double>();
                var longTrackY = new List<double>();

                longTrackX.AddMany(0, 1);
                longTrackY.AddMany(pointList[0][indexI[i]].Y, pointList[1][indexJ[i]].Y);

                longTrackXList.Add(longTrackX);
                longTrackYList.Add(longTrackY);
            }







            for (int i = 0; i < indexI.Count; i++) 
            {
                var longTrack = new Track();
                longTrack.StartFrame = 0;
                longTrack.EndFrame = 1;
                longTrack.LowBin = Math.Min(pointList[0][indexI[i]].Y, pointList[1][indexJ[i]].Y);
                longTrack.HighBin = Math.Max(pointList[0][indexI[i]].Y, pointList[1][indexJ[i]].Y);
                longTrackList.Add(longTrack);

                //longTrackX[i] = pointList[0][indexI[i]].X;
                //longTrackY[i] = pointList[0][indexJ[i]].Y;

                //longTrackAmplitude.Add(matrix[indexI[i],indexJ[i]]);

                //longTrackXList.Add(longTrackX);
                //longTrackYList.Add(longTrackY);
                //longTrackAmplitudeList.Add(longTrackAmplitude);
            }
            

            // remove peaks which have already been used to produce long tracks

            
            for (int i = 0; i < indexI.Count; i++)
            {
                pointList[0][indexI[i]] = null;
                
            }

            for (int i = 0; i < pointList[0].Count; i++)
            {
                if (pointList[0][i] == null) 
                {
                    pointList[0].RemoveAt(i);
                    i--;
                }

            }
            


            for (int i = 0; i < indexJ.Count; i++)
            {
                pointList[1][indexJ[i]] = null;
            }

            for (int i = 0; i < pointList[1].Count; i++)
            { 
                if(pointList[1][i] == null)
                {
                    pointList[1].RemoveAt(i);
                    i--;
                }          
            }
                


            // save individual peaks into shortTracks 
            for (int i = 0; i < pointList[0].Count; i++)
            {
                var shortTrack = new Track();
                shortTrack.StartFrame = 0;
                shortTrack.EndFrame = 0;
                shortTrack.LowBin = pointList[0][i].Y;
                shortTrack.HighBin = pointList[0][i].Y;
                shortTrackList.Add(shortTrack);
            
            }

            for (int i = 0; i < pointList[1].Count; i++)
            {
                var shortTrack = new Track();
                shortTrack.StartFrame = 1;
                shortTrack.EndFrame = 1;
                shortTrack.LowBin = pointList[1][i].Y;
                shortTrack.HighBin = pointList[1][i].Y;
                shortTrackList.Add(shortTrack);

            }
            
            // use linear regression to extend long tracks and use neareast distance to extend short tracks
            int c = 2;
            while (c < column)
            {
                // save the long tracks to closed track lists
                for (int i = 0; i < longTrackList.Count; i++)
                {
                    if ((c - longTrackList[i].EndFrame) > frameThreshold)
                    {
                        if (longTrackList[i].Duration > duraionThreshold)
                        {
                            closedTrackList.Add(longTrackList[i]);
                                                    
                            longTrackList.RemoveAt(i);
                            longTrackXList.RemoveAt(i);
                            longTrackYList.RemoveAt(i);
                            i--;
                        }
                                            
                    }                
                }


                for (int i = 0; i < shortTrackList.Count; i++)
                {
                    if ((c - shortTrackList[i].EndFrame) > frameThreshold)
                    {
                        shortTrackList.RemoveAt(i);
                        i--;
                    }               
                }


                if (longTrackList.Count != 0)
                {
                    var numberA = new List<int>();
                    var numberB = new List<int>();

                    // use linear regression to predict the next position of long tracks
                    for (int i = 0; i < longTrackList.Count; i++)
                    {
                        var xdata = new double[longTrackXList.Count];
                        xdata = longTrackXList[i].ToArray();

                        var ydata = new double[longTrackYList.Count];
                        ydata = longTrackYList[i].ToArray();

                        var p = Fit.Line(xdata, ydata);
                        var offset = p[0];
                        var slope = p[1];

                        var position = c * slope + offset;

                        // need to be refresh according to the profile document
                        binToreance = binToreanceStable;
                        for (int j = 0; j < pointList[c].Count; j++)
                        {
                            
                            if (Math.Abs(position - pointList[c][j].Y) < binToreance)
                            {
                                numberA.Add(i);
                                numberB.Add(j);

                                binToreance = Math.Abs(position - pointList[c][j].Y);
                            }
                        }
                    }

                    for (int i = 0; i < numberA.Count; i++)
                    {
                        longTrackList[numberA[i]].EndFrame = c;
                        longTrackList[numberA[i]].LowBin = Math.Min(longTrackList[numberA[i]].LowBin, pointList[c][numberB[i]].Y);
                        longTrackList[numberA[i]].HighBin = Math.Max(longTrackList[numberA[i]].HighBin, pointList[c][numberB[i]].Y);

                        longTrackXList[numberA[i]].Add(c);
                        longTrackYList[numberA[i]].Add(pointList[c][numberB[i]].Y);

                    }



                    for (int i = 0; i < numberB.Count; i++)
                    {
                        pointList[c][numberB[i]] = null;
                    }

                    for (int i = 0; i < pointList[c].Count; i++)
                    {
                        if (pointList[c][i] == null)
                        {
                            pointList[c].RemoveAt(i);
                            i--;
                        }
                    }
                        
                    

                    // add points of current frame to short tracks
                    var numberE = new List<int>();
                    var numberF = new List<int>();

                    for (int i = 0; i < shortTrackList.Count; i++)
                    {
                        binToreance = binToreanceStable;
                        for (int j = 0; j < pointList[c].Count; j++)
                        {
                            if (Math.Abs(shortTrackList[i].HighBin - pointList[c][j].Y) < binToreance)
                            {
                                numberE.Add(i);
                                numberF.Add(j);
                                binToreance = Math.Abs(shortTrackList[i].HighBin - pointList[c][j].Y);

                            }
                        }
                    }

                    for (int i = 0; i < numberE.Count; i++)
                    {
                        var longTrack = new Track();
                        var longTrackX = new List<double>();
                        var longTrackY = new List<double>();

                        longTrack.StartFrame = shortTrackList[numberE[i]].StartFrame;
                        longTrack.EndFrame = c;
                        longTrack.LowBin = Math.Min(shortTrackList[numberE[i]].LowBin, pointList[c][numberF[i]].Y);
                        longTrack.HighBin = Math.Max(shortTrackList[numberE[i]].HighBin, pointList[c][numberF[i]].Y);
                        longTrackList.Add(longTrack);

                        longTrackX.AddMany(shortTrackList[numberE[i]].StartFrame, pointList[c][numberF[i]].X);
                        longTrackY.AddMany(shortTrackList[numberE[i]].LowBin, pointList[c][numberF[i]].Y);

                        longTrackXList.Add(longTrackX);
                        longTrackYList.Add(longTrackY);
                    }


                    for (int i = 0; i < numberE.Count; i++)
                    {
                        shortTrackList[numberE[i]] = null;
                    }

                    for (int i = 0; i < shortTrackList.Count; i++)
                    {
                        if (shortTrackList[i] == null)
                        {
                            shortTrackList.RemoveAt(i);
                            i--;
                        }
                    }

                    //..........................................//
                    for (int i = 0; i < numberF.Count; i++)
                    {
                        pointList[c][numberF[i]] = null;
                    }

                    for (int i = 0; i < pointList[c].Count; i++)
                    {
                        if (pointList[c][i] == null)
                        {
                            pointList[c].RemoveAt(i);
                            i--;
                        }
                    }
    
                    //..........................................//

                    for (int i = 0; i < pointList[c].Count; i++)
                    {
                        var shortTrack = new Track();
                        shortTrack.StartFrame = c;
                        shortTrack.EndFrame = c;
                        shortTrack.LowBin = pointList[c][i].Y;
                        shortTrack.HighBin = pointList[c][i].Y;
                        shortTrackList.Add(shortTrack);
                    }

                    
                }
                else
                {
                    c = c + 1;
                    var numberC = new List<int>();
                    var numberD = new List<int>();

                    for (int i = 0; i < pointList[c].Count; i++)
                    {
                        binToreance = binToreanceStable;
                        for (int j = 0; j < pointList[c + 1].Count; j++)
                        {
                            if (Math.Abs(pointList[c][i].Y - pointList[c + 1][j].Y) < binToreance)
                            {
                                numberC.Add(i);
                                numberD.Add(j);
                                binToreance = Math.Abs(pointList[c][i].Y - pointList[c + 1][j].Y);
                            }
                        }
                    }

                    for (int i = 0; i < numberC.Count; i++)
                    {

                        var longTrackX = new List<double>();
                        var longTrackY = new List<double>();
                        longTrackX.AddMany(c, (c+1));
                        longTrackY.AddMany(pointList[c][numberC[i]].Y, pointList[(c+1)][numberD[i]].Y);

                        longTrackXList.Add(longTrackX);
                        longTrackYList.Add(longTrackY);                  
                    }

                    for (int i = 0; i < numberC.Count; i++)
                    {
                        var longTrack = new Track();
                        longTrack.StartFrame = c;
                        longTrack.EndFrame = c + 1;
                        longTrack.LowBin = Math.Min(pointList[c][numberC[i]].Y, pointList[c + 1][numberC[i]].Y);
                        longTrack.HighBin = Math.Max(pointList[c][numberC[i]].Y, pointList[c + 1][numberC[i]].Y);

                        longTrackList.Add(longTrack);
                    }

                    for (int i = 0; i < numberC.Count; i++)
                    {
                        pointList[c][numberC[i]] = null;
                    }

                    for (int i = 0; i < pointList[c].Count; i++)
                    {
                        if (pointList[c][i] == null)
                        {
                            pointList[c].RemoveAt(i);
                            i--;
                        }
                    }

                    for (int i = 0; i < numberD.Count; i++)
                    {
                        pointList[c+1][numberD[i]] = null;
                    }

                    for (int i = 0; i < pointList[c+1].Count; i++)
                    {
                        if (pointList[c+1][i] == null)
                        {
                            pointList[c+1].RemoveAt(i);
                            i--;
                        }
                    }

                    for (int i = 0; i < pointList[c].Count; i++)
                    {
                        var shortTrack = new Track();
                        shortTrack.StartFrame = c;
                        shortTrack.EndFrame = c;
                        shortTrack.LowBin = pointList[c][i].Y;
                        shortTrack.HighBin = pointList[c][i].Y;
                        shortTrackList.Add(shortTrack);
                    }


                    for (int i = 0; i < pointList[c + 1].Count; i++)
                    {
                        var shortTrack = new Track();
                        shortTrack.StartFrame = c + 1;
                        shortTrack.EndFrame = c + 1;
                        shortTrack.LowBin = pointList[c + 1][i].Y;
                        shortTrack.HighBin = pointList[c + 1][i].Y;
                        shortTrackList.Add(shortTrack);
                    }

                }
 
                c = c + 1;
            }

            // remove tracks with few points
            for (int i = 0; i < closedTrackList.Count; i++)
            {
                if((longTrackXList[i].Count / closedTrackList[i].Duration) < trackThreshold)
                {
                    closedTrackList.RemoveAt(i);
                    longTrackXList.RemoveAt(i);
                    longTrackYList.RemoveAt(i);
                    i--;
                }
            }
                    
   
            // complement the gap among tracks 


            //var closedTrackXList = new List<List<double>>();
            //var closedTrackYList = new List<List<double>>();
            //var diffTrackXList = new List<List<double>>();

            //for (int i = 0; i < closedTrackList.Count; i++)
            //{
            //    for (int j = closedTrackList[i].StartFrame; j <= closedTrackList[i].EndFrame; j++)
            //    { 
            //        closedTrackXList[i].Add(j);
            //    }                            
            //}

            //for (int i = 0; i < closedTrackList.Count; i++)
            //{
            //    diffTrackXList[i] = closedTrackXList[i].Except(longTrackXList[i]).ToList();                                           
            //}

            //if (diffTrackXList.Count > 0)
            //{
            //    for (int i = 0; i < diffTrackXList.Count; i++)
            //    {
            //        for (int j = 0; j < diffTrackXList[i].Count; j++)
            //        {                        
            //            // save list to array
            //            var xdata = new List<double>();
            //            var ydata = new List<double>();
            //            for (int s = closedTrackList[i].StartFrame; s < (diffTrackXList[i][j] - 1); s++)
            //            {
            //                xdata.Add(s);                        
            //            }
                    
            //            for (int t = 0; t < xdata.Count; t++)
            //            {
            //                ydata.Add(longTrackYList[i][t]);                       
            //            }

            //            var xdataArray = new double[xdata.Count];
            //            var ydataArray = new double[xdata.Count];
            //            xdataArray = xdata.ToArray();
            //            ydataArray = ydata.ToArray();

            //            var p = Fit.Line(xdataArray, ydataArray);
            //            var offset = p.Item1;
            //            var slope = p.Item2;
         
            //            var position = c * slope + offset;

            //            closedTrackYList[diffTrackXList[i][j] - closedTrackList[i].StartFrame] = position;
            //        }

            //        // change closedTrackYList to array

            //        }               
            //    }            
            //}


            // convert closedTrackList to trackMatrix
            // To do: convert double to int 
            var result = new double[row, column];
            for (int i = 0; i < closedTrackList.Count; i++)
            { 
                for (int j = 0; j < closedTrackList[i].Duration; j++)
                {
                    result[(int) Math.Ceiling(longTrackXList[i][j]),(int) Math.Ceiling(longTrackYList[i][j])] = 1;
                                
                }           
            }

            return result;
        }

    }
}
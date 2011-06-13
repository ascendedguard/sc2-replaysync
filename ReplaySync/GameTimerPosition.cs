// -----------------------------------------------------------------------
// <copyright file="GameTimerPosition.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace ReplaySync
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GameTimerPosition
    {
        private static Dictionary<Size, Rect> resolutions = new Dictionary<Size,Rect>();

        static GameTimerPosition()
        {
            // Is this fine, or should it be exported to a data file?
            // All positions captured using Ascend's RegionMeasure application, observing
            // a replay that has a time over 1 hour (for the longest reasonable replay time).
            resolutions.Add(new Size(1920, 1200), new Rect(17, 863, 113, 24));
            resolutions.Add(new Size(1920, 1080), new Rect(14, 777, 102, 20));
            resolutions.Add(new Size(1776, 1000), new Rect(14, 719, 96, 20));
            resolutions.Add(new Size(1680, 1050), new Rect(15, 755, 101, 20));
            resolutions.Add(new Size(1600, 1200), new Rect(17, 863, 113, 24));
            resolutions.Add(new Size(1440, 900), new Rect(13, 648, 87, 16));
            resolutions.Add(new Size(1400, 1050), new Rect(15, 755, 100, 20));
            resolutions.Add(new Size(1360, 1024), new Rect(14, 737, 96, 20));
            resolutions.Add(new Size(1360, 768), new Rect(10, 552, 78, 15));
            resolutions.Add(new Size(1280, 1024), new Rect(13, 754, 96, 19));
            resolutions.Add(new Size(1280, 960), new Rect(13, 690, 96, 19));
            resolutions.Add(new Size(1280, 800), new Rect(11, 575, 79, 16));
            resolutions.Add(new Size(1280, 768), new Rect(10, 552, 80, 16));
            resolutions.Add(new Size(1280, 720), new Rect(10, 518, 70, 15));
            resolutions.Add(new Size(1152, 864), new Rect(12, 621, 83, 18));
            resolutions.Add(new Size(1152, 648), new Rect(10, 482, 70, 14));
            resolutions.Add(new Size(1024, 768), new Rect(10, 552, 79, 15));
            // 800x600 is not supported by SC2.
        }

        /// <summary> Gets the capture rectangle based on the current screen resolution. </summary>
        /// <returns> Returns a Rect containing the capture position. </returns>
        public static Rect GetCaptureRect()
        {
            Size resolution = new Size(SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight);

            if (resolutions.ContainsKey(resolution))
            {
                return resolutions[resolution];
            }
            else
            {
                throw new KeyNotFoundException("Resolution is not supported.");
            }
        }
    }
}

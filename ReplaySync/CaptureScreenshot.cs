// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CaptureScreenshot.cs" company="Ascend">
//   Copyright © 2011 All Rights Reserved
// </copyright>
// <summary>
//   Captures screenshots using WINAPI.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ReplaySync
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Interop;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Captures screenshot using WINAPI.
    /// </summary>
    internal static class CaptureScreenshot
    {
        /// <summary> Pointer to the screen context. Static to prevent reallocation crashes. </summary>
        private static readonly IntPtr ScreenDc;

        /// <summary> Pointer to the compatible screen context. Static to prevent reallocation crashes. </summary>
        private static readonly IntPtr MemDc;

        /// <summary> Initializes static members of the <see cref="CaptureScreenshot"/> class. </summary>
        static CaptureScreenshot()
        {
            ScreenDc = GetDC(IntPtr.Zero);
            MemDc = CreateCompatibleDC(ScreenDc);
        }

        /// <summary> Frees all static memory of the screen contexts. </summary>
        public static void Destroy()
        {
            ReleaseDC(IntPtr.Zero, ScreenDc);
            ReleaseDC(IntPtr.Zero, MemDc);
        }

        /// <summary> Capture the screenshot. </summary>
        /// <param name="area">Area of screenshot.</param>
        /// <returns>Bitmap source that can be used e.g. as background.</returns>
        public static BitmapSource Capture(Rect area)
        {
            IntPtr ptrBitmap = CreateCompatibleBitmap(ScreenDc, (int)area.Width, (int)area.Height);

            if (ptrBitmap == IntPtr.Zero)
            {
                // Bitmap allocation failed. This probably shouldn't happen.
                return null;
            }

            // Select bitmap from compatible bitmap to memDC
            SelectObject(MemDc, ptrBitmap); 

            BitBlt(MemDc, 0, 0, (int)area.Width, (int)area.Height, ScreenDc, (int)area.X, (int)area.Y, TernaryRasterOperations.SRCCOPY);
            BitmapSource bsource = Imaging.CreateBitmapSourceFromHBitmap(ptrBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            DeleteObject(ptrBitmap);
            
            bsource.Freeze();                

            return bsource;
        }

        #region WINAPI DLL Imports

        [DllImport("gdi32.dll", ExactSpelling = true, PreserveSig = true, SetLastError = true)]
        internal static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int width, int height);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr ptrObject);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr ptrWnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr ptrWnd, IntPtr handleDc);

        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

        /// <summary> Enumarion of 
        /// </summary>
        private enum TernaryRasterOperations : uint
        {
            /// <summary>dest = source</summary>
            SRCCOPY = 0x00CC0020,

            /// <summary>dest = source OR dest</summary>
            SRCPAINT = 0x00EE0086,

            /// <summary>dest = source AND dest</summary>
            SRCAND = 0x008800C6,

            /// <summary>dest = source XOR dest</summary>
            SRCINVERT = 0x00660046,

            /// <summary>dest = source AND (NOT dest)</summary>
            SRCERASE = 0x00440328,

            /// <summary>dest = (NOT source)</summary>
            NOTSRCCOPY = 0x00330008,

            /// <summary>dest = (NOT src) AND (NOT dest)</summary>
            NOTSRCERASE = 0x001100A6,

            /// <summary>dest = (source AND pattern)</summary>
            MERGECOPY = 0x00C000CA,

            /// <summary>dest = (NOT source) OR dest</summary>
            MERGEPAINT = 0x00BB0226,

            /// <summary>dest = pattern</summary>
            PATCOPY = 0x00F00021,

            /// <summary>dest = DPSnoo</summary>
            PATPAINT = 0x00FB0A09,

            /// <summary>dest = pattern XOR dest</summary>
            PATINVERT = 0x005A0049,

            /// <summary>dest = (NOT dest)</summary>
            DSTINVERT = 0x00550009,

            /// <summary>dest = BLACK</summary>
            BLACKNESS = 0x00000042,

            /// <summary>dest = WHITE</summary>
            WHITENESS = 0x00FF0062
        }

        #endregion
    }

}

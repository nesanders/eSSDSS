﻿//Class adapted from http://stackoverflow.com/questions/1858122/saving-a-screenshot-of-a-window-using-c-wpf-and-dwm

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// for DLL import
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows;

namespace eSSDSS
{
    class snapshot
    {

        public static class Utilities
        {
            public static BitmapSource CaptureScreen()
            {
                return CaptureWindow(User32.GetDesktopWindow());
            }

            public static BitmapSource CaptureWindow(IntPtr handle)
            {

                IntPtr hdcSrc = User32.GetWindowDC(handle);

                RECT windowRect = new RECT();
                User32.GetWindowRect(handle, ref windowRect);

                int width = windowRect.right - windowRect.left;
                int height = windowRect.bottom - windowRect.top;

                IntPtr hdcDest = Gdi32.CreateCompatibleDC(hdcSrc);
                IntPtr hBitmap = Gdi32.CreateCompatibleBitmap(hdcSrc, width, height);

                IntPtr hOld = Gdi32.SelectObject(hdcDest, hBitmap);
                Gdi32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, 13369376);
                Gdi32.SelectObject(hdcDest, hOld);
                Gdi32.DeleteDC(hdcDest);
                User32.ReleaseDC(handle, hdcSrc);

                //Image image = Image.FromHbitmap(hBitmap);
                BitmapSource returnImage;
                returnImage = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                Gdi32.DeleteObject(hBitmap);

                return returnImage;
            }
        }

        public class Gdi32
        {
            [DllImport("gdi32.dll")]
            public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hObjectSource, int nXSrc, int nYSrc, int dwRop);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteDC(IntPtr hDC);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteObject(IntPtr hObject);
            [DllImport("gdi32.dll")]
            public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
        }

        public static class User32
        {
            [DllImport("user32.dll")]
            public static extern IntPtr GetDesktopWindow();
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowDC(IntPtr hWnd);
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);
            [DllImport("user32.dll")]
            public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
    }

    //public static class ControlExtensions
    //{
    //    public static Image DrawToImage(this System.Windows.Controls.Control control)
    //    {
    //        return snapshot.Utilities.CaptureWindow(control.Handle);
    //    }
    //}
}

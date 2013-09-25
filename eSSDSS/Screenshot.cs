using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;

namespace eSSDSS
{
    public static class Screenshot
    {
        /// <summary>
        /// Gets a JPG "screenshot" of the current UIElement
        /// </summary>
        /// <param name="source">UIElement to screenshot</param>
        /// <param name="scale">Scale to render the screenshot</param>
        /// <param name="quality">JPG Quality</param>
        /// <returns>Byte array of JPG data</returns>
        public static byte[] GetJpgImage(this UIElement source, double scale, int quality)
        {
            double actualHeight = source.RenderSize.Height;
            double actualWidth = source.RenderSize.Width;

            double renderHeight = actualHeight * scale;
            double renderWidth = actualWidth * scale;

            RenderTargetBitmap renderTarget = new RenderTargetBitmap((int) renderWidth, (int) renderHeight, 96, 96, PixelFormats.Pbgra32);
            VisualBrush sourceBrush = new VisualBrush(source);

            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            using (drawingContext)
            {
                drawingContext.PushTransform(new ScaleTransform(scale, scale));
                drawingContext.DrawRectangle(sourceBrush, null, new Rect(new System.Windows.Point(0, 0), new System.Windows.Point(actualWidth, actualHeight)));
            }
            renderTarget.Render(drawingVisual);

            JpegBitmapEncoder jpgEncoder = new JpegBitmapEncoder();
            jpgEncoder.QualityLevel = quality;
            jpgEncoder.Frames.Add(BitmapFrame.Create(renderTarget));

            Byte[] _imageArray;

            using (MemoryStream outputStream = new MemoryStream())
            {
                jpgEncoder.Save(outputStream);
                _imageArray = outputStream.ToArray();
            }

            return _imageArray;
        }

        public static BitmapSource SaveAndBitmapSource(this UIElement source)
        {
            //Get screenshot
            byte[] screenshot = source.GetJpgImage(1.0, 95);
            // Save jpeg
            FileStream fileStream = new FileStream(@"SDSS_Screenshot.jpg", FileMode.Create, FileAccess.ReadWrite);
            BinaryWriter binaryWriter = new BinaryWriter(fileStream);
            binaryWriter.Write(screenshot);
            binaryWriter.Close();
            // Load jpeg
            String appdir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var path = new Uri(System.IO.Path.Combine(appdir, "SDSS_Screenshot.jpg"));
            return new BitmapImage(path);
        }

        public static BitmapSource GDIsnap(this FrameworkElement source)
        {
            //Based on http://stackoverflow.com/a/3891991
            //See also http://jevgenig.blogspot.com/2011/06/wpf-webbrowser-screenshot.html
            var topLeftCorner = source.PointToScreen(new System.Windows.Point(0, 0));
            var topLeftGdiPoint = new System.Drawing.Point((int)topLeftCorner.X, (int)topLeftCorner.Y);
            var size = new System.Drawing.Size((int)source.ActualWidth, (int)source.ActualHeight);

            var screenShot = new Bitmap((int)source.ActualWidth, (int)source.ActualHeight);

            using (var graphics = Graphics.FromImage(screenShot))
            {
                graphics.CopyFromScreen(topLeftGdiPoint, new System.Drawing.Point(),
                    size, CopyPixelOperation.SourceCopy);
            }

            screenShot.Save(@"SDSS_Screenshot.png", ImageFormat.Png);

            // Load screenshot
            String appdir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var path = new Uri(System.IO.Path.Combine(appdir, "SDSS_Screenshot.png"));
            return new BitmapImage(path);
        }
    }
}


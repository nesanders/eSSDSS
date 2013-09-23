using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;
using System.Drawing;

namespace eSSDSS
{

    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class SurfaceWindow1 : SurfaceWindow
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SurfaceWindow1()
        {
            InitializeComponent();

            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();
        }

        /// <summary>
        /// Occurs when the window is about to close. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Remove handlers for window availability events
            RemoveWindowAvailabilityHandlers();
        }

        /// <summary>
        /// Adds handlers for window availability events.
        /// </summary>
        private void AddWindowAvailabilityHandlers()
        {
            // Subscribe to surface window availability events
            ApplicationServices.WindowInteractive += OnWindowInteractive;
            ApplicationServices.WindowNoninteractive += OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable += OnWindowUnavailable;
        }

        /// <summary>
        /// Removes handlers for window availability events.
        /// </summary>
        private void RemoveWindowAvailabilityHandlers()
        {
            // Unsubscribe from surface window availability events
            ApplicationServices.WindowInteractive -= OnWindowInteractive;
            ApplicationServices.WindowNoninteractive -= OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable -= OnWindowUnavailable;
        }

        /// <summary>
        /// This is called when the user can interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowInteractive(object sender, EventArgs e)
        {
            //TODO: enable audio, animations here
        }

        /// <summary>
        /// This is called when the user can see but not interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowNoninteractive(object sender, EventArgs e)
        {
            //TODO: Disable audio here if it is enabled

            //TODO: optionally enable animations here
        }

        /// <summary>
        /// This is called when the application's window is not visible or interactive.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowUnavailable(object sender, EventArgs e)
        {
            //TODO: disable audio, animations here
        }

        private void content_frame_Initialized(object sender, EventArgs e)
        {
            content_frame.Source = new Uri("page_wwt.xaml",UriKind.RelativeOrAbsolute);
        }

        private void modeswitch_Click(object sender, RoutedEventArgs e)
        {
            //page_wwt.ContentProperty.GetType().GetMethod("wwt_GetCoordinates").Invoke(new page_wwt(), null);

            String p_wwt = "eSSDSS;component/page_wwt.xaml";
            String p_sdss = "eSSDSS;component/page_sdss.xaml";
            Uri u_wwt = new Uri("page_wwt.xaml", UriKind.RelativeOrAbsolute);
            Uri u_sdss = new Uri("page_sdss.xaml", UriKind.RelativeOrAbsolute);


            if (content_frame.Source.OriginalString == p_wwt)
            {
                content_frame.Source = u_sdss;
            }
            else if (content_frame.Source.OriginalString == p_sdss)
            {
                content_frame.Source = u_wwt;
            }
            else { 
                MessageBox.Show("Error: content_frame source not recognized.");
            }
        }
    }


    public class g_coords
    {
        public static Single w_RA = 10.0f;
        public static Single w_DEC = 10.0f;
        public static Single w_FOV = 10.0f;
        public static BitmapSource image;
        
    }


}
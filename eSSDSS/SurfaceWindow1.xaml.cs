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

        private void surfaceButton1_Click(object sender, RoutedEventArgs e)
        {
            wwt_GetCoordinates();
            Single w_RA = Convert.ToSingle(label1.Content);
            Single w_DEC = Convert.ToSingle(label2.Content);
            sdss_getspec(w_RA,w_DEC,this.image1);
        }

        private void webBrowser1_Initialized(object sender, EventArgs e)
        {
            //String appdir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //String myfile = System.IO.Path.Combine(appdir, "wwt_html5.htm");
            //this.wwt_web.Navigate(String.Format("file:///{0}", myfile));
            this.wwt_web.Navigate(@"http://www.worldwidetelescope.org/docs/Samples/wwtwebclientsimpleUIHtml5.html");
        }

        private void wwt_GetCoordinates()
        {
            if (wwt_web.IsLoaded)
            {
                // The only way I can find to expose the wwt coordinate functions is to redefine the functions in terms of the window namespace
                dynamic document = this.wwt_web.Document;
                dynamic head = document.GetElementsByTagName("head")[0];
                dynamic scriptEl = document.CreateElement("script");
                scriptEl.text = @"function getRA() {return(String(window.wwt.getRA()*15.))};
                                  function getDEC() {return(String(window.wwt.getDec()))};
                                  function getFOV() {return(String(window.wwt.get_fov()))};";
                head.AppendChild(scriptEl);


                float w_RA = 10.0f;
                try
                {
                    w_RA = Convert.ToSingle(wwt_web.InvokeScript("getRA"));
                }
                catch (Exception ex)
                {
                    string msg = "Could not call script to get RA:\n" + ex.Message;
                    MessageBox.Show(msg);
                }
                label1.Content = w_RA;

                Single w_DEC = 10.0f;
                try
                {
                    w_DEC = Convert.ToSingle(wwt_web.InvokeScript("getDEC"));
                }
                catch (Exception ex)
                {
                    string msg = "Could not call script to get DEC:\n" + ex.Message;
                    MessageBox.Show(msg);
                }
                label2.Content = w_DEC;

                Single w_FOV = 10.0f;
                try
                {
                    w_FOV = Convert.ToSingle(wwt_web.InvokeScript("getFOV"));
                }
                catch (Exception ex)
                {
                    string msg = "Could not call script to get FOV:\n" + ex.Message;
                    MessageBox.Show(msg);
                }
                label_fov.Content = w_FOV;
            }
            else
            {
                MessageBox.Show("Please wait - WWT not yet fully loaded.");
            }
        
        }

        private void web_getimage(string URL, Image CONT)
        {
            var image = new BitmapImage();
            int BytesToRead=100;

            WebRequest request = WebRequest.Create(new Uri(URL, UriKind.Absolute));
            request.Timeout = -1;
            WebResponse response = request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            BinaryReader reader = new BinaryReader(responseStream);
            MemoryStream memoryStream = new MemoryStream();

            byte[] bytebuffer = new byte[BytesToRead];
            int bytesRead = reader.Read(bytebuffer, 0, BytesToRead);

            while (bytesRead > 0)
            {
                memoryStream.Write(bytebuffer, 0, bytesRead);
                bytesRead = reader.Read(bytebuffer, 0, BytesToRead);
            }

            image.BeginInit();
            memoryStream.Seek(0, SeekOrigin.Begin);

            image.StreamSource = memoryStream;
            image.EndInit();

            CONT.Source=image;
        }

        private void sdss_getspec(float RA, float DEC, Image CONT)
        {
            //Put up placeholder image
            String appdir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var path = new Uri(System.IO.Path.Combine(appdir, "shutter.png"));
            CONT.Source = new BitmapImage(path);
            CONT.InvalidateVisual();

            // Get closest specobj
            String sql_query;
            sql_query = String.Format("http://skyserver.sdss3.org/dr10/en/tools/search/x_sql.aspx?format=csv&cmd=SELECT TOP 1 s.specobjID, GN.distance  FROM SpecObjAll as s JOIN dbo.fGetNearbyObjEq({0},{1}, 1.0) AS GN ON s.bestObjId = GN.objID ORDER BY distance", RA, DEC);
            MessageBox.Show(sql_query);
            WebRequest wrGETURL;
            wrGETURL = WebRequest.Create(sql_query);
            Stream GRS = wrGETURL.GetResponse().GetResponseStream();
            TextReader reader = (TextReader)new StreamReader(GRS);
            string sline = reader.ReadToEnd();

            // Parse csv
            if (sline.Split('\n').Length == 1)
            {
                MessageBox.Show("SDSS query error:\n"+sline);
                CONT.Source = null;
            }
            else
            {
                sline = sline.Split('\n')[1];
                String objid = sline.Split(',')[0];
                // Get spectrum
                String spec_query;
                spec_query = "http://skyserver.sdss3.org/dr10/en/get/specById.ashx?ID=" + objid;
                web_getimage(spec_query, CONT);
            }
            
        }
    }
}
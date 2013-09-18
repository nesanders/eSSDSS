using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace eSSDSS
{
    /// <summary>
    /// Interaction logic for page_sdss.xaml
    /// </summary>
    public partial class page_sdss : Page
    {
        public page_sdss()
        {
            InitializeComponent();
        }

        private void surfaceButton1_Click(object sender, RoutedEventArgs e)
        {
            wwt_GetCoordinates();
            Single w_RA = Convert.ToSingle(label1.Content);
            Single w_DEC = Convert.ToSingle(label2.Content);
            sdss_getspec(w_RA, w_DEC, this.image1);
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
            int BytesToRead = 100;

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

            CONT.Source = image;
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
            WebRequest wrGETURL;
            wrGETURL = WebRequest.Create(sql_query);
            Stream GRS = wrGETURL.GetResponse().GetResponseStream();
            TextReader reader = (TextReader)new StreamReader(GRS);
            string sline = reader.ReadToEnd();

            // Parse csv
            if (sline.Split('\n').Length == 1)
            {
                MessageBox.Show("SDSS query error:\n" + sline);
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

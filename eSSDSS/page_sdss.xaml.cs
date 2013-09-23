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
using System.Net;
using System.IO;
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;

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

        

        private void lab_RA_v_Loaded_1(object sender, RoutedEventArgs e)
        {
            lab_RA_v.Content = g_coords.w_RA.ToString();
        }

        private void lab_DEC_v_Loaded_1(object sender, RoutedEventArgs e)
        {
            lab_DEC_v.Content = g_coords.w_DEC.ToString();
        }

        private void lab_FOV_v_Loaded_1(object sender, RoutedEventArgs e)
        {
            lab_FOV_v.Content = g_coords.w_FOV.ToString();
        }

        private void im_main_Loaded_1(object sender, RoutedEventArgs e)
        {
            im_main.Source = g_coords.image;
            im_main.InvalidateVisual();
        }
    }
}

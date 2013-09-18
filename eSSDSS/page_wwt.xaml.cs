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
    /// Interaction logic for page_wwt.xaml
    /// </summary>
    public partial class page_wwt : Page
    {
        public page_wwt()
        {
            InitializeComponent();
        }

        private void surfaceButton1_Click(object sender, RoutedEventArgs e)
        {
            SurfaceWindow1.contentframe.source = new Uri("page_sdss.xaml",UriKind.RelativeOrAbsolute);
        }

        private void wwt_web_Initialized(object sender, EventArgs e)
        {
            // Load local / custom html5 file
            //String appdir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //String myfile = System.IO.Path.Combine(appdir, "wwt_html5.htm");
            //this.wwt_web.Navigate(String.Format("file:///{0}", myfile));

            // Load online / sample html5 file
            //this.wwt_web.Navigate(@"http://www.worldwidetelescope.org/docs/Samples/wwtwebclientsimpleUIHtml5.html");
            this.wwt_web.Navigate(@"http://rawgithub.com/nesanders/eSSDSS/master/eSSDSS/wwt_html5.htm");
        }
    }
}

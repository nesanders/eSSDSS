﻿using System;
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
            eSSDSS.SurfaceWindow1.content_frame.Source = new Uri("page_sdss.xaml", UriKind.RelativeOrAbsolute);
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

                try
                {
                    g_coords.w_RA = Convert.ToSingle(wwt_web.InvokeScript("getRA"));
                }
                catch (Exception ex)
                {
                    string msg = "Could not call script to get RA:\n" + ex.Message;
                    MessageBox.Show(msg);
                }

                try
                {
                    g_coords.w_DEC = Convert.ToSingle(wwt_web.InvokeScript("getDEC"));
                }
                catch (Exception ex)
                {
                    string msg = "Could not call script to get DEC:\n" + ex.Message;
                    MessageBox.Show(msg);
                }

                try
                {
                    g_coords.w_FOV = Convert.ToSingle(wwt_web.InvokeScript("getFOV"));
                }
                catch (Exception ex)
                {
                    string msg = "Could not call script to get FOV:\n" + ex.Message;
                    MessageBox.Show(msg);
                }
            }
            else
            {
                MessageBox.Show("Please wait - WWT not yet fully loaded.");
            }

        }
    }
}

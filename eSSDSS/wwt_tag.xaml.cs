using System;
using System.Collections.Generic;
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
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;

namespace eSSDSS
{
    /// <summary>
    /// Interaction logic for wwt_tag.xaml
    /// </summary>
    public partial class wwt_tag : TagVisualization
    {
        public wwt_tag()
        {
            InitializeComponent();
        }

        private void wwt_tag_Loaded(object sender, RoutedEventArgs e)
        {
            //TODO: customize wwt_tag's UI based on this.VisualizedTag here
        }

        private void TagVisualization_Initialized(object sender, EventArgs e)
        {
            String appdir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var path = System.IO.Path.Combine(appdir, "camera-shutter-click-01.wav");
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(path);
            player.Play();
        }
    }
}

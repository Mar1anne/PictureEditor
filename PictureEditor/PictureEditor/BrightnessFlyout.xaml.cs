using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace PictureEditor
{
    public sealed partial class BrightnessFlyout : UserControl
    {
        public int BrightnessLevel;
        public BrightnessFlyout()
        {
            this.InitializeComponent();
            sliderBrigtness.Maximum=100;
            sliderBrigtness.Minimum=0;
             sliderBrigtness.Value=0;
             sliderBrigtness.Visibility = Visibility.Visible;
            
        }

        private void progressBarBrightness_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            
            textBrightness.Text = "Brightness : " + sliderBrigtness.Value + "%";
            BrightnessLevel = Convert.ToInt32(sliderBrigtness.Value);
        }
    }
}

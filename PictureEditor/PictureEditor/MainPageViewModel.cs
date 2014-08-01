using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureEditor
{
    public class MainPageViewModel
    {
        public class GifEnitty
        {
            public Uri GifImageSource { get; set; }
        }

        private readonly Uri _imageSource = new Uri("ms-appx:///Images/Building.png");
        private readonly Uri _gifImageSource = new Uri("ms-appx:///Images/test.gif");
        //private readonly Uri _gifImageSource1 = new Uri("http://fmn.rrimg.com/fmn063/xiaozhan/20111110/1305/x_large_oyeU_06ff000026e3121a.gif", UriKind.RelativeOrAbsolute);
        private readonly Uri _gifImageSource1 = new Uri("http://fmn.rrimg.com/fmn066/xiaozhan/20121213/1335/original_iNqY_5e260000d3c2125c.gif", UriKind.RelativeOrAbsolute);
        private readonly Uri _networkImageSource = new Uri("http://fmn.rrimg.com/fmn062/xiaozhan/20121213/1355/original_I8lO_0ffa0000d485118f.jpg", UriKind.RelativeOrAbsolute);
        private List<GifEnitty> _gifs = new List<GifEnitty>();
        /// <summary>
        /// Gets or sets the path to the source image.
        /// </summary>
        /// <value>The path to the source image.</value>
        public Uri ImageSource
        {
            get { return _imageSource; }
        }

        public Uri NetworkImageSource
        {
            get { return _networkImageSource; }
        }

        public Uri GifImageSource
        {
            get { return _gifImageSource; }
        }

        public Uri GifImageSource1
        {
            get { return _gifImageSource1; }
        }

        public ICollection<GifEnitty> Gifs
        {
            get { return _gifs; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPageViewModel"/> class.
        /// </summary>
        public MainPageViewModel()
        {
            for (int i = 0; i < 100; i++)
            {
                this._gifs.Add(new GifEnitty() { GifImageSource = _gifImageSource });
            }
        }
    }
}

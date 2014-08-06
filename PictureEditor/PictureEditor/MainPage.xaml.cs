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
using ImageTools.Filtering;
//using ImageTools.Controls;
using ImageTools.Helpers;
using ImageTools.IO;
using ImageTools;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;
using Windows.UI.Popups;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using System.Windows;
using ImageTools.IO.Bmp;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PictureEditor
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        WriteableBitmap wb;
        WriteableBitmap a;
        bool isFiltered;
        public MainPage()
        {
            this.InitializeComponent();
            openButton.Click += openButton_Click;
            addFilterButton.Click += addFilterButton_Click;
            saveButton.Click += saveButton_Click;
            isFiltered = false;
          //  wb = BitmapFactory.New(1, 1);
        }

        void addFilterButton_Click(object sender, RoutedEventArgs e)
        {
            filter((Application.Current as App).editStream);
        }

        void saveButton_Click(object sender, RoutedEventArgs e)
        {
          
          
            
        }
         async void saveEdit()
        {
            if (!isFiltered)
            {
                var destinationFolder = await KnownFolders.PicturesLibrary.CreateFolderAsync("MetroEditor", CreationCollisionOption.OpenIfExists);

                string allImages = @"All";
                StorageFolder InstallationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                // CreateFolderAsync("All", CreationCollisionOption.OpenIfExists);
                StorageFolder all = await InstallationFolder.GetFolderAsync(allImages);
                //  StorageFile file= (App.Current as App).editImage.B
                //    Stream stream = new Stream("empty.bmp", FileMode.Create);
                //   BitmapEncoder encoder = new BitmapEncoder

                //encoder.Frames.Add(BitmapFrame.Create(image));
                //MessageBox.Show(myPalette.Colors.Count.ToString());
                //encoder.Save(stream);
                StorageFile file = await destinationFolder.CreateFileAsync("image");
                BmpEncoder encoder=new BmpEncoder();
               // encoder.Encode(img, stream);
               // BitmapEncoder en=await BitmapEncoder.CreateAsync(
                
                
            }
         }
        
   private async Task<StorageFile> WriteableBitmapToStorageFile(WriteableBitmap WB, string fileFormat)
 
{
 
    string FileName = "MyFile.";
 
    Guid BitmapEncoderGuid = BitmapEncoder.JpegEncoderId;
        FileName += "jpeg";
 
            BitmapEncoderGuid = BitmapEncoder.JpegEncoderId;
 
   
 
 
 
    var file = await Windows.Storage.ApplicationData.Current.TemporaryFolder.CreateFileAsync(FileName, CreationCollisionOption.GenerateUniqueName);
 
    using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
 
    {
 
        BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoderGuid, stream);

        Stream pixelStream = WB.PixelBuffer as Stream;
 
        byte[] pixels = new byte[pixelStream.Length];
 
        await pixelStream.ReadAsync(pixels, 0, pixels.Length);
 
 
 
        encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
 
                            (uint)WB.PixelWidth,
 
                            (uint)WB.PixelHeight,
 
                            96.0,
 
                            96.0,
 
                            pixels);
 
        await encoder.FlushAsync();
 
    }
 
    return file;
 
}
 
 
 


        



        void openButton_Click(object sender, RoutedEventArgs e)
        {
            pickPhoto();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        async protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.DataContext = new MainPageViewModel();
             //String a = e.Parameter as String;
            if (e.Parameter != null)
            {
                //Moze da se iskoristi i (Application.Current as App).editImage
              //  BitmapImage bmp = new BitmapImage();
                (Application.Current as App).editImage.UriSource = new Uri(e.Parameter as String);
                testImage.Source = (Application.Current as App).editImage;
            }
            else
                testImage.Source = (Application.Current as App).editImage;
            //  testImage.a

        }

        /// property is typically used to configure the page.</param>
        //async protected override void OnNavigatedTo(NavigationEventArgs e,Object a)
        //{
        //    this.DataContext = new MainPageViewModel();
        //    String ab = a as String;
        //    if(ab.Equals("Controls"))
        //    testImage.Source = (Application.Current as App).editImage;
        //    //  testImage.a

        //}

        public async void pickPhoto()
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            openPicker.FileTypeFilter.Add("*");

            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                try
                {
                    var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                    BitmapImage image = new BitmapImage();
                    image.SetSource(stream);
                    //src = image;
                    //mainPicture.Source = image;
                    //mainPicture.Stretch = Stretch.Uniform;

                    filter(stream.AsStream());
                }
                catch (Exception e)
                {
                    MessageDialog msg = new MessageDialog("Greska : " + e.Message);
                    msg.ShowAsync();
                }


            }
            else
            {
                //  OutputTextBlock.Text = "Operation cancelled.";
                //    notif.Text = "Greska";
            }

        }
        async public void filter(Stream stream)
        {
            try
            {
                IImageFilter filter = new GrayscaleRMY();

                //  Stream stream;
                ExtendedImage myImage = new ExtendedImage();
                await myImage.SetSource(stream);
                ExtendedImage extImage = ExtendedImage.ApplyFilters(myImage, filter);

                BitmapImage b = new BitmapImage();

                isFiltered = true;
                (App.Current as App).editStream = await extImage.ToStream();
                WriteableBitmap bitmap = extImage.ToBitmap();
                testImage.Source = bitmap;
            }
            catch (Exception e)
            {
                MessageDialog msg = new MessageDialog("Greska : " + e.Message);
                msg.ShowAsync();
            }

        }


        async public void openImage(WriteableBitmap wb)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            openPicker.FileTypeFilter.Add("*");

            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                try
                {
                    var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                    BitmapImage image = new BitmapImage();
                    //image.SetSource(stream);
                    //src = image;
                    //testImage.Source = image;
                    //mainPicture.Stretch = Stretch.Uniform;

                    wb.SetSource(stream);
                    // await wb.FromStream(await stream);
                    //wb.FromStream(stream);





                }
                catch (Exception e)
                {
                    MessageDialog msg = new MessageDialog("tuka Greska : " + e.Message);
                    msg.ShowAsync();
                }

            }
            else
            {
                //  OutputTextBlock.Text = "Operation cancelled.";
                //    notif.Text = "Greska";
            }

        }


        //public void crop(WriteableBitmap wb)
        //{
        //    wb = WriteableBitmapExtensions.Rotate(wb, 90);

        //    testImage.Source = wb;

        //    //else
        //    //{
        //    //    MessageDialog msg = new MessageDialog("Ne smee da e null: " );
        //    //    msg.ShowAsync();
        //    //}
        //}

    }
}

using PictureEditor.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Grouped Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234231

namespace PictureEditor
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class StartPage : PictureEditor.Common.LayoutAwarePage
    {

        public StartPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        //        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        //        {
        //            // TODO: Assign a collection of bindable groups to this.DefaultViewModel["Groups"]
        //        }
        //    }
        //}
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model for your problem domain to replace the sample data
            var sampleDataGroups = SampleDataSource.GetGroups("AllGroups");
            this.DefaultViewModel["Groups"] = sampleDataGroups;
        }

        /// <summary>
        /// Invoked when a group header is clicked.
        /// </summary>
        /// <param name="sender">The Button used as a group header for the selected group.</param>
        /// <param name="e">Event data that describes how the click was initiated.</param>
        void Header_Click(object sender, RoutedEventArgs e)
        {
            // Determine what group the Button instance represents
            var group = (sender as FrameworkElement).DataContext;
            String a = ((SampleDataGroup)group).UniqueId;

            //// Navigate to the appropriate destination page, configuring the new page
            //// by passing required information as a navigation parameter
            //this.Frame.Navigate(typeof(MainPage), ((SampleDataGroup)group).UniqueId);
            if ("Controls".Equals(a))
                this.Frame.Navigate(typeof(MainPage));

        }

        /// <summary>
        /// Invoked when an item within a group is clicked.
        /// </summary>
        /// <param name="sender">The GridView (or ListView when the application is snapped)
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var itemId = ((SampleDataItem)e.ClickedItem).UniqueId;
            var uri=((SampleDataItem)e.ClickedItem).Title;
            //this.Frame.Navigate(typeof(MainPage), itemId);  
            //  String a=((SampleDataGroup)group).UniqueId;
            if ("Controls2".Equals(itemId))
            {
                savePic();
                this.Frame.Navigate(typeof(MainPage));
            }
            else if ("Controls1".Equals(itemId))
            {
                camera();
            }
            else
            {
                (App.Current as App).uri = uri;
                this.Frame.Navigate(typeof(MainPage),uri);

            }
        }

        /*
         Koga kje se klikne Browse ,  treba prvo da se socuva slikata kako aplikaciska promenliva : Bitmap i Stream 
         * potoa treba da se zacuva vo folder , storage
         * 
         */


        public async void readFolder(List<String> titles)
        {
            StorageFolder myFolder = await KnownFolders.PicturesLibrary.GetFolderAsync("MetroPhotoEditor");
            //.Ope("MetroPhotoEditor");
            IReadOnlyList<StorageFile> fileList = await myFolder.GetFilesAsync();
            titles = new List<string>();
            foreach (var file in fileList)
            {
                titles.Add(file.Name);
            }



        }

        private async void savePic()
        {
            var destinationFolder = await KnownFolders.PicturesLibrary.CreateFolderAsync("MetroEditor", CreationCollisionOption.OpenIfExists);
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            //   StorageFolder my = await ApplicationData.Current.LocalFolder.GetFolderAsync("AllImages");
            string allImages = @"All";
            StorageFolder InstallationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            // CreateFolderAsync("All", CreationCollisionOption.OpenIfExists);
            StorageFolder all = await InstallationFolder.GetFolderAsync(allImages);
            var openpicker = new FileOpenPicker();
            openpicker.CommitButtonText = "Upload";
            openpicker.FileTypeFilter.Add(".jpg");
            openpicker.FileTypeFilter.Add(".jpeg");
            openpicker.FileTypeFilter.Add(".png");
            openpicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openpicker.ViewMode = PickerViewMode.List;

            var file = await openpicker.PickSingleFileAsync();


            if (destinationFolder != null && file != null)
            {


                try
                {
                    var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                     BitmapImage image = new BitmapImage();
                    //editImage.SetSource(stream);
                    (Application.Current as App).editStream = stream.AsStream();
                    (Application.Current as App).editImage.SetSource(stream);
                    await file.CopyAsync(destinationFolder);
                    //await file.CopyAsync(localFolder);
                    await file.CopyAsync(all);
                }
                catch (Exception e)
                {

                    MessageDialog msg = new MessageDialog("Greska So Object : " + e.Message);
                    msg.ShowAsync();
                }


            }
        }

        public async void camera()
        {
            CameraCaptureUI camera = new CameraCaptureUI();
            camera.PhotoSettings.CroppedAspectRatio = new Size(16, 9);
            StorageFile photo = await camera.CaptureFileAsync(CameraCaptureUIMode.Photo);

            //By default the photo will be stored at location
            //%localappdata%\Packages\APP_PACKAGE_ID\TempState

            if (photo != null)
            {
                //await photo.MoveAsync(KnownFolders.PicturesLibrary);
                //OR
                await photo.MoveAsync(KnownFolders.PicturesLibrary, "DesiredPhotoName" + photo.FileType, NameCollisionOption.GenerateUniqueName);
            }

        }
    }
}


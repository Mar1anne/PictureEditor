﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace ImageTools.Controls
{
    public sealed class ImageExt : Control
    {
        ///// <summary>
        ///// Initializes a new instance of the <see cref="ImageExt"/> class.
        ///// </summary>
        public ImageExt()
        {
            this.DefaultStyleKey = typeof(ImageExt);
            _animationTimer = new DispatcherTimer();
            _animationTimer.Tick += timer_Tick;
        }

        #region Constants

        /// <summary>
        /// Defines the name of the 'Image' template part.
        /// This template part renders the image after it is converted to a writeable bitmap.
        /// </summary>
        public const string ImagePart = "Image";

        #endregion

        #region Invariant

#if !WINDOWS_PHONE
        [ContractInvariantMethod]
        private void AnimatedImageInvariantMethod()
        {
            Contract.Invariant(_animationFrameIndex >= 0);
            Contract.Invariant(_animationTimer != null);
            Contract.Invariant(_frames != null);
        }
#endif

        #endregion

        #region Fields

        private Image _image;
        private DispatcherTimer _animationTimer;
        private List<KeyValuePair<ImageBase, ImageSource>> _frames = new List<KeyValuePair<ImageBase, ImageSource>>();
        private int _animationFrameIndex;
        private bool _isLoadingImage;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the loading of the image has been completed.
        /// </summary>
        public event EventHandler LoadingCompleted;
        /// <summary>
        /// Raises the <see cref="E:LoadingCompleted"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void OnLoadingCompleted(EventArgs e)
        {
            EventHandler eventHandler = LoadingCompleted;

            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }

        /// <summary>
        /// Occurs when the loading of the image failed.
        /// </summary>
        public event EventHandler<Exception> LoadingFailed;
        /// <summary>
        /// Raises the <see cref="E:LoadingFailed"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.UnhandledExceptionEventArgs"/> instance 
        /// containing the event data.</param>
        protected void OnLoadingFailed(Exception e)
        {
            EventHandler<Exception> eventHandler = LoadingFailed;

            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Defines the <see cref="Pause"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PauseProperty =
            DependencyProperty.Register("Pause", typeof(bool), typeof(ImageExt), new PropertyMetadata(false));
        /// <summary>
        /// Gets or sets a value indicating if the animation is paused.
        /// </summary>
        /// <value>A value indicating if the animation is paused.</value>
        public bool Pause
        {
            [ContractVerification(false)]
            get { return (bool)GetValue(PauseProperty); }
            set { SetValue(PauseProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Filter"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.Register("Filter", typeof(IImageFilter), typeof(ImageExt), new PropertyMetadata(null, OnFilterPropertyChanged));

        /// <summary>
        /// Gets or sets the filter that will be used before the image will be applied.
        /// </summary>
        /// <value>The filter.</value>
        public IImageFilter Filter
        {
            get { return (IImageFilter)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        private static void OnFilterPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var owner = o as ImageExt;
            if (owner != null)
            {
                owner.OnFilterPropertyChanged();
            }
        }

        private void OnFilterPropertyChanged()
        {
            if (Source != null && Source.IsFilled)
            {
                LoadImage(Source);
            }
        }

        /// <summary>
        /// Identifies the <see cref="Stretch"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.Register("Stretch", typeof(Stretch),
                typeof(ImageExt), new PropertyMetadata(Stretch.Uniform));
        /// <summary>
        /// Gets or sets a value that describes how an <see cref="ImageExt"/> 
        /// should be stretched to fill the destination rectangle. This is a dependency property.
        /// </summary>
        /// <value>A value of the enumeration that specifies how the source image is applied if the 
        /// Height and Width of the Image are specified and are different than the source image's height and width.
        /// The default value is Uniform.</value>
        public Stretch Stretch
        {
            [ContractVerification(false)]
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="AutoSize"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty AutoSizeProperty =
            DependencyProperty.Register("AutoSize", typeof(bool),
                typeof(ImageExt), null);
        /// <summary>
        /// Gets or sets a value indicating whether the control should be auto sized. If the value is true
        /// the control will get the width and the height of its image source. This is a 
        /// dependency property.
        /// </summary>
        /// <value><c>true</c> if the size of the control should be set to the image
        /// width and height; otherwise, <c>false</c>.</value>
        public bool AutoSize
        {
            [ContractVerification(false)]
            get { return (bool)GetValue(AutoSizeProperty); }
            set { SetValue(AutoSizeProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="AnimationMode"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty AnimationModeProperty =
            DependencyProperty.Register("AnimationMode", typeof(AnimationMode),
                typeof(ImageExt), new PropertyMetadata(AnimationMode.Repeat));
        /// <summary>
        /// Gets or sets the animation mode of the image. This property will be just
        /// ignored if the specified source is not an animated image.
        /// </summary>
        /// <value>A value of the enumeration, that defines how to animate the image.</value>
        public AnimationMode AnimationMode
        {
            [ContractVerification(false)]
            get { return (AnimationMode)GetValue(AnimationModeProperty); }
            set { SetValue(AnimationModeProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Source"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(ExtendedImage),
                typeof(ImageExt), new PropertyMetadata(null, OnSourcePropertyChanged));
        /// <summary>
        /// Gets or sets the source for the image.
        /// </summary>
        /// <value>The source of the image control.</value>
        /// <remarks>
        /// The property supports the following types:
        /// <list type="table">
        /// <listheader>
        /// 	<term>Type</term>
        /// 	<description>Description</description>
        /// </listheader>
        /// <item>
        /// 	<term><see cref="String"/></term>
        /// 	<description>A string will be transformed to a <see cref="Uri"/> object with a relative path. A new BitmapImage
        ///     will be loaded asynchronously and assigned to the internal image element. Only png and .jpeg files
        ///     are supported usings string directly.</description>
        /// </item>
        /// <item>
        /// 	<term><see cref="ImageSource"/></term>
        /// 	<description>The image source will be directly assigned. No animations will be used.</description>
        /// </item>
        /// <item>
        /// 	<term><see cref="ImageExt"/></term>
        /// 	<description>The image will be assigned. Depending of the fact, if it is an animated image or not, 
        /// 	the animation will be started immediatly.</description>
        /// </item>
        /// 	</list>
        /// </remarks>
        /// <exception cref="ArgumentException">The specified value is not supported. Must be one of the types 
        /// defined below.</exception>
        public ExtendedImage Source
        {
            get { return (ExtendedImage)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        /// <summary>
        /// Called when source property is changed.
        /// </summary>
        /// <param name="d">The dependency object, which raised the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> 
        /// instance containing the event data.</param>
        private static void OnSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var owner = d as ImageExt;
            if (owner != null)
            {
                owner.OnSourceChanged();
            }
        }

        /// <summary>
        /// Called when the value of the source property is changed.
        /// </summary>
        protected void OnSourceChanged()
        {
            if (_image != null && Source != null)
            {
                if (!Source.IsFilled || Source.IsLoading)
                {
                    Source.LoadingCompleted += new EventHandler(image_LoadingCompleted);
                    Source.LoadingFailed += new EventHandler<Exception>(image_LoadingFailed);
                }
                else
                {
                    LoadImage(Source);
                }
            }
        }

        #endregion

        #region Constructors

        #endregion

        #region Methods

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or 
        /// internal processes (such as a rebuilding layout pass) 
        /// call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. 
        /// In simplest terms, this means the method is called just before a UI element 
        /// displays in an application. For more information, see Remarks.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            BindXaml();

            OnSourceChanged();
        }

        private void BindXaml()
        {
            _image = GetTemplateChild(ImagePart) as Image;
        }

        private void LoadImage(ExtendedImage image)
        {
            Contract.Requires(image != null);

            if (!_isLoadingImage)
            {
                _isLoadingImage = true;

                if (Filter != null)
                {
                    IImageFilter filter = Filter;

                    Task.Factory.StartNew(() =>
                        {
                            ExtendedImage filteredImage = ExtendedImage.ApplyFilters(image, filter);

                            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => AssignImage(filteredImage));
                        });
                    //ThreadPool.QueueUserWorkItem(x =>
                    //{
                    //    ExtendedImage filteredImage = ExtendedImage.ApplyFilters(image, filter);

                    //    Dispatcher.BeginInvoke(() => AssignImage(filteredImage));
                    //});
                }
                else
                {
                    AssignImage(image);
                }
            }
        }

        private void AssignImage(ExtendedImage image)
        {
            Contract.Requires(image != null);

            _isLoadingImage = false;

            if (image.IsFilled)
            {
                _frames.Clear();

                WriteableBitmap imageBitmap = image.ToBitmap();

                if (image.IsAnimated && AnimationMode != AnimationMode.None)
                {
                    Stop();

                    List<ImageBase> frames = new List<ImageBase>();
                    frames.Add(image);
                    frames.AddRange(image.Frames.OfType<ImageBase>());

                    foreach (ImageBase frame in frames)
                    {
                        if (frame != null && frame.IsFilled)
                        {
                            _frames.Add(new KeyValuePair<ImageBase, ImageSource>(frame, frame.ToBitmap()));
                        }
                    }

                    AnimateImage();

                    Start();
                }
                else
                {
                    if (_image != null)
                    {
                        //BitmapImage test = new BitmapImage(new Uri("ms-appx:///Images/Building.png"));
                        //_image.Source = test;
                        _image.Source = imageBitmap;
                    }
                }
            }
        }

        private void image_LoadingCompleted(object sender, EventArgs e)
        {
            ExtendedImage image = sender as ExtendedImage;

            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                LoadImage(image);

                image.LoadingCompleted -= new EventHandler(image_LoadingCompleted);
                image.LoadingFailed -= new EventHandler<Exception>(image_LoadingFailed);

                OnLoadingCompleted(e);
            });
        }

        private void image_LoadingFailed(object sender, Exception e)
        {
            ExtendedImage image = sender as ExtendedImage;

            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                image.LoadingCompleted -= new EventHandler(image_LoadingCompleted);
                image.LoadingFailed -= new EventHandler<Exception>(image_LoadingFailed);

                OnLoadingFailed(e);
            });
        }

        private void timer_Tick(object sender, object e)
        {
            if (!Pause)
            {
                AnimateImage();
            }
        }

        private void AnimateImage()
        {
            if (_animationFrameIndex < _frames.Count)
            {
                var currentFrame = _frames[_animationFrameIndex];

                if (currentFrame.Key != null)
                {
                    if (_image != null)
                    {
                        _image.Source = currentFrame.Value;
                    }

                    _animationTimer.Interval = new TimeSpan(0, 0, 0, 0, currentFrame.Key.DelayTime * 10);
                    _animationFrameIndex++;

                    if (_animationFrameIndex == _frames.Count)
                    {
                        if (AnimationMode == AnimationMode.PlayOnce)
                        {
                            Stop();
                        }

                        _animationFrameIndex = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Provides the behavior for the Measure pass of Silverlight layout. 
        /// Classes can override this method to define their own Measure pass behavior.
        /// </summary>
        /// <param name="availableSize">The available size that this object can give to child objects.</param>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its 
        /// calculations of the allocated sizes for child objects; 
        /// or based on other considerations, such as a fixed container size.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (Source != null && Source.IsFilled && AutoSize)
            {
                return new Size(Source.PixelWidth, Source.PixelHeight);
            }
            else
            {
                return base.MeasureOverride(availableSize);
            }
        }

        /// <summary>
        /// Starts the animation. If there is no image assigned or the 
        /// assigned image is not a animated image, this method will just be ignored. If 
        /// the animation was paused, the animation will continue where it was stopped.
        /// </summary>
        public void Start()
        {
            Pause = false;

            _animationTimer.Start();
        }

        /// <summary>
        /// Stops the animation. If there is no image assigned or the 
        /// assigned image is not a animated image, this method will just be ignored.
        /// </summary>
        public void Stop()
        {
            _animationFrameIndex = 0;

            _animationTimer.Stop();
        }

        #endregion
    }
}

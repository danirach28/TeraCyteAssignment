using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TeraCyteAssignment.Models;

namespace TeraCyteAssignment.ViewModels
{
    public partial class ImageResultPairViewModel : ObservableObject
    {
        public ImageResultPaireData Data { get; }


        public ImageSource Thumbnail { get; }

        private ImageSource? _fullResolutionImage;


        public ImageSource FullResolutionImage
        {
            get
            {
                // Lazy-loading pattern: If the image hasn't been created yet, create it now.
                if (_fullResolutionImage == null)
                {
                    _fullResolutionImage = CreateImageFromBase64(Data.imageBytes, isThumbnail: false);
                }
                return _fullResolutionImage;
            }
        }

        public string ImageId => Data.ImageId;

        public ImageResultPairViewModel(ImageResultPaireData data)
        {
            Data = data;
            Thumbnail = CreateImageFromBase64(data.imageBytes, isThumbnail: true);

        }

        private static ImageSource CreateImageFromBase64(byte[]? imageBytes, bool isThumbnail)
        {
            try
            {
                using var stream = new MemoryStream(imageBytes);
                var bitmap = new BitmapImage();
                bitmap.BeginInit();

                // If creating a thumbnail, set the DecodePixelWidth to save memory.
                if (isThumbnail)
                {
                    bitmap.DecodePixelWidth = 100;
                }

                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
            catch
            {
                return new BitmapImage();
            }
        }
    }
}


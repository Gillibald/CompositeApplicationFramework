#region Usings

using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

#endregion

namespace CompositeApplicationFramework.Helper
{
    public static class ImageHelper
    {
        public static BitmapImage GetImageFromUri(string uri)
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(uri);
            image.EndInit();
            return image;
        }

        public static BitmapImage ToBitmapImage(this WriteableBitmap bitmap)
        {
            return ByteArrayToBitmapImage(bitmap.ToByteArray());
        }

        public static BitmapImage ByteArrayToBitmapImage(byte[] imageData)
        {
            if (imageData.Length == 0)
            {
                return null;
            }
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(imageData);
            bitmapImage.EndInit();
            bitmapImage.Freeze();
            return bitmapImage;
        }

        public static byte[] ToByteArray(this ImageSource imageSource)
        {
            var bitmapSource = imageSource as BitmapSource;

            if (bitmapSource == null)
            {
                return new byte[0];
            }
            var encoder = new PngBitmapEncoder();
            var memoryStream = new MemoryStream();
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            encoder.Save(memoryStream);
            return memoryStream.ToArray();
        }
    }
}
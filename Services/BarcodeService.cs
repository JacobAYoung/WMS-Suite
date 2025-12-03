using BarcodeStandard;
using SkiaSharp;
using System;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Media.Imaging;
using Type = BarcodeStandard.Type;

namespace WMS_Suite.Services
{
    public class BarcodeService : IBarcodeService
    {
        public BitmapImage GenerateBarcode(string data, string label = null)
        {
            var b = new Barcode();
            b.IncludeLabel = !string.IsNullOrEmpty(label);
            var barcodeImage = b.Encode(Type.Code128, data, SKColors.Black, SKColors.White, 300, 100); // Adjust width/height as needed

            // Convert SKImage to BitmapImage for WPF
            using (var stream = new MemoryStream())
            {
                barcodeImage.Encode(SKEncodedImageFormat.Png, 100).SaveTo(stream);
                stream.Position = 0;
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }

        public void PrintBarcode(BitmapImage barcodeImage)
        {
            // Convert BitmapImage to System.Drawing.Image for printing
            using (var stream = new MemoryStream())
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(barcodeImage));
                encoder.Save(stream);
                stream.Position = 0;
                using (var bitmap = (Bitmap)Image.FromStream(stream))
                {
                    var pd = new PrintDocument();
                    pd.PrintPage += (sender, args) => args.Graphics.DrawImage(bitmap, 0, 0);
                    pd.Print();
                }
            }
        }
    }
}

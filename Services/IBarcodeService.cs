using SkiaSharp;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Media.Imaging;

namespace WMS_Suite.Services
{
    public interface IBarcodeService
    {
        BitmapImage GenerateBarcode(string data, string label = null); // Returns Bitmap for preview/print
    }
}

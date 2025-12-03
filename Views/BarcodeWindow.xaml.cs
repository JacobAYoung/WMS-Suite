using SkiaSharp;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using WMS_Suite.Services;

namespace WMS_Suite.Views
{
    /// <summary>
    /// Interaction logic for BarcodeWindow.xaml
    /// </summary>
    public partial class BarcodeWindow : Window
    {
        private BitmapImage _barcodeImage;

        public BarcodeWindow(BitmapImage barcodeImage)
        {
            InitializeComponent();
            _barcodeImage = barcodeImage;
            // Assume your Image control is named "BarcodeImage" in XAML: <Image x:Name="BarcodeImage" Height="100" Margin="5"/>
            BarcodeImage.Source = _barcodeImage;
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            new BarcodeService().PrintBarcode(_barcodeImage);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog { Filter = "PNG|*.png" };
            if (dialog.ShowDialog() == true)
            {
                using (var fileStream = new FileStream(dialog.FileName, FileMode.Create))
                {
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(_barcodeImage));
                    encoder.Save(fileStream);
                }
            }
        }
    }
}

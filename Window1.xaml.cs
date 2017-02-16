using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Media.Imaging;

using DYMO.Label.Framework;

using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Controls;

namespace Barcode
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1
    {
        private BitmapImage _currentQRImage;

        public Window1()
        {
            InitializeComponent();
            CalculateQRCode(this, null);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PrintersComboBox.ItemsSource = Framework.GetLabelWriterPrinters();
            PrintersComboBox.SelectedIndex = 0;
        }

        private void PrintBarcodeButton_Click(object sender, RoutedEventArgs e)
        {
            // load label template
            var label = DYMO.Label.Framework.Label.Open(
                Application.GetResourceStream(
                    new Uri("Barcode.label", UriKind.Relative)).Stream);

            // set barcode data
            label.SetObjectText("Barcode", "http://developers.dymo.com");

            // print
            try
            {
                label.Print(PrintersComboBox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error printing: " + ex.Message);
            }
        }

        private void PrintBarcodeAsImageButton_Click(object sender, RoutedEventArgs e)
        {
            // load label template
            var label = DYMO.Label.Framework.Label.Open(
                Application.GetResourceStream(
                    new Uri("BarcodeAsImage.label", UriKind.Relative)).Stream);

            // set data as a barcode image
            label.SetImagePngData("Image", ((BitmapImage)QRCodeImage.Source).StreamSource);

            // print
            try
            {
                label.Print(PrintersComboBox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error printing: " + ex.Message);
            }

        }

        private void CalculateQRCode(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            byte[] data;

            using (var ms = new MemoryStream())
            {
                var encoder = new QrEncoder();
                var qrCode = encoder.Encode(QRCodeURI.Text);
                var renderer = new Renderer(5);
                renderer.WriteToStream(qrCode.Matrix, ms, ImageFormat.Png);

                data = ms.ToArray();
            }

            var bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = new MemoryStream(data);
            bi.EndInit();
            try
            {
                QRCodeImage.Source = bi;
            }
            catch { }
        }
    }
}

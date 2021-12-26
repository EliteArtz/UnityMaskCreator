using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static UnityMaskCreator.Channels.Channels;

namespace UnityMaskCreator
{
    public partial class Main : Window
    {
        public static bool active = true;
        public static List<MaskInput> images = new List<MaskInput>();
        private static byte[] _pixelsFinal;
        private static byte[] _colorData;

        

        private static void FileDialog(object sender, Image img, Channel channel)
        {
            TextBlock tb = (TextBlock)sender;
            if (!active) return;

            using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "Image (*.png;*.jpg;*.bmp)|*.png;*.jpg;*.bmp" })
            {
                DialogResult res = ofd.ShowDialog();

                if (res != System.Windows.Forms.DialogResult.OK && string.IsNullOrWhiteSpace(ofd.FileName))
                {
                    ClearInput(tb, img, channel);
                    return;
                }


                MaskInput mi = new MaskInput { path = ofd.FileName, channel = channel };
                tb.Text = ofd.FileName;

                if (images.Count > 0 && images.FindIndex(image => image.channel == channel) != -1)
                    images.Remove(images[images.FindIndex(image => image.channel == channel)]);

                images.Add(mi);

                SetImage(ofd.FileName, img);
            }
        }
        private static void SetImage(string path, Image img)
        {
            BitmapImage src = new BitmapImage();

            src.BeginInit();
            src.UriSource = new Uri(path, UriKind.Absolute);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();

            img.Source = src;
        }

        private static BitmapSource CreateMaskImg(MaskInput image, int imgIndex)
        {
            string currentImg = image.path;
            BitmapImage bmp = new BitmapImage();

            bmp.BeginInit();
            bmp.UriSource = new Uri(currentImg, UriKind.Absolute);
            bmp.EndInit();


            PixelFormat pf = IsChannelUsed(Channel.A) ? PixelFormats.Pbgra32 : PixelFormats.Bgr32;
            int stride = bmp.PixelWidth * 4;
            int size = bmp.PixelHeight * stride;
            byte[] pixels = new byte[size];
            bmp.CopyPixels(pixels, stride, 0);

            // Check if the final color data or final pixels is empty, we will fill it with data to start if true.
            if (_pixelsFinal == null)
                _pixelsFinal = new byte[size];
            if (_colorData == null)
                _colorData = new byte[size * 4];

            byte[] newPixels = new byte[size];
            WriteableBitmap crtBmp = new WriteableBitmap(bmp);

            // Accessing bitmap in multiple threads returns an exception, here I am saving important data for acessing.
            int width = crtBmp.PixelWidth,
                height = crtBmp.PixelHeight;
            for(int x = 0; x < width;x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int index = y * stride + 4 * x;

                    switch (image.channel)
                    {
                        case Channel.A:
                            _colorData.SetValue(pixels[index], index + 3);
                            break;
                        case Channel.R:
                            _colorData.SetValue(pixels[index + 2], index + 2);
                            break;
                        case Channel.G:
                            _colorData.SetValue(pixels[index + 1], index + 1);
                            break;
                        case Channel.B:
                            _colorData.SetValue(pixels[index], index);
                            break;
                    }
                }
            };

            if (images.Count != (imgIndex + 1)) return null; //check if this is(n't) the last channel to calculate


            crtBmp.WritePixels(new Int32Rect(0, 0, crtBmp.PixelWidth, crtBmp.PixelHeight), _colorData, stride, 0);
            crtBmp.CopyPixels(newPixels, stride, 0);
            for (int i = 0; i < newPixels.Length; i++)
            {
                if (_pixelsFinal[i] == 0) _pixelsFinal[i] = newPixels[i];
            }

            // Create mask as Bitmap
            BitmapSource bitmap = BitmapSource.Create(crtBmp.PixelWidth, crtBmp.PixelHeight, 96, 96, pf, null, _pixelsFinal, stride);

            string path = $"./output/{DateTime.Now:HH_mm_ss-MM_dd_yyyy}.png";

            return SaveMaskImg(bitmap, path);
        }



        private static void ClearInput(TextBlock tb, Image img, Channel channel)
        {
            if (images.Count == 0 && !active) return;
            if (images.FindIndex(image => image.channel == channel) == -1) return;

            images.Remove(images[images.FindIndex(image => image.channel == channel)]);
            img.Source = null;
            tb.Text = string.Empty;
            return;
        }

        private static BitmapSource SaveMaskImg(BitmapSource bitmapMask, string destinationPath = "./")
        {
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapMask));

            if (!Directory.Exists("./output/"))
                Directory.CreateDirectory("./output/");

            using (FileStream fs = new FileStream(destinationPath, FileMode.OpenOrCreate))
                encoder.Save(fs);

            return bitmapMask;
        }

        private void DisposeData()
        {
            Array.Clear(_pixelsFinal, 0, _pixelsFinal.Length);
            Array.Clear(_colorData, 0, _colorData.Length);
        }
    }
}

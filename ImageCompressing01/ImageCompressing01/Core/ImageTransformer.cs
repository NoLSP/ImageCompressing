using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageCompressing01.Core
{
    public static class ImageTransformer
    {
        public static Bitmap ToGrayByEqualWeights(Image image)
        {
            var bitmap = new Bitmap(image);

            for (var i = 0; i < bitmap.Width; i++)
            {
                for (var j = 0; j < bitmap.Height; j++)
                {
                    var pixel = bitmap.GetPixel(i, j);
                    var Y = (byte)((pixel.R + pixel.G + pixel.B) / 3);
                    bitmap.SetPixel(i, j, Color.FromArgb(Y, Y, Y));
                }
            }

            return bitmap;
        }

        public static Bitmap ToGrayByCCIR(Image image)
        {
            var bitmap = new Bitmap(image);

            for (var i = 0; i < bitmap.Width; i++)
            {
                for (var j = 0; j < bitmap.Height; j++)
                {
                    var pixel = bitmap.GetPixel(i, j);
                    var Y = 0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B;
                    bitmap.SetPixel(i, j, Color.FromArgb((int)Math.Round(Y), (int)Math.Round(Y), (int)Math.Round(Y)));
                }
            }

            return bitmap;
        }

        public static Bitmap ToYCbCr(Image image)
        {
            var bitmap = new Bitmap(image);

            for (var i = 0; i < bitmap.Width; i++)
            {
                for (var j = 0; j < bitmap.Height; j++)
                {
                    var pixel = bitmap.GetPixel(i, j);

                    var doubleY = 0.301 * pixel.R + 0.5859 * pixel.G + 0.1133 * pixel.B;
                    var Y = Math.Round(doubleY) < 0 ? 0 : Math.Round(doubleY) > 255 ? 255 : Math.Round(doubleY);

                    var doubleCb = 0.5625 * (pixel.B - Y) + 128;
                    var doubleCr = 0.7148 * (pixel.R - Y) + 128;

                    var Cb = Math.Round(doubleCb) < 0 ? 0 : Math.Round(doubleCb) > 255 ? 255 : Math.Round(doubleCb);
                    var Cr = Math.Round(doubleCr) < 0 ? 0 : Math.Round(doubleCr) > 255 ? 255 : Math.Round(doubleCr);

                    bitmap.SetPixel(i, j, Color.FromArgb((int)Y, (int)Cb, (int)Cr));
                }
            }

            return bitmap;
        }

        public static Bitmap ToRGB(Image image)
        {
            var bitmap = new Bitmap(image);

            for (var i = 0; i < bitmap.Width; i++)
            {
                for (var j = 0; j < bitmap.Height; j++)
                {
                    var pixel = bitmap.GetPixel(i, j);
                    var doubleR = pixel.R + 1.3989 * (pixel.B - 128);
                    var doubleG = pixel.R - 0.3442 * (pixel.G - 128) - 0.7172 * (pixel.B - 128);
                    var doubleB = pixel.R + 1.7778 * (pixel.G - 128);

                    var R = Math.Round(doubleR) < 0 ? 0 : Math.Round(doubleR) > 255 ? 255 : Math.Round(doubleR);
                    var G = Math.Round(doubleG) < 0 ? 0 : Math.Round(doubleG) > 255 ? 255 : Math.Round(doubleG);
                    var B = Math.Round(doubleB) < 0 ? 0 : Math.Round(doubleB) > 255 ? 255 : Math.Round(doubleB);

                    bitmap.SetPixel(i, j, Color.FromArgb((int)R, (int)G, (int)B));
                }
            }

            return bitmap;
        }

        public static PSNR CountPSNR(Image firstImage, Image secondImage)
        {
            try
            {
                var mse = CountMSE(new Bitmap(firstImage), new Bitmap(secondImage));
                var countedPsnrR = (double)0;
                var countedPsnrG = (double)0;
                var countedPsnrB = (double)0;
                var result = new PSNR() { MSE = mse, MSEFull = (mse.R + mse.G + mse.B) / 3 };

                //R
                if (mse.R == 0)
                {
                    result.R = "inf";
                }
                else
                {
                    countedPsnrR = CountPSNR(mse.R);
                    result.R = countedPsnrR.ToString("0.###");
                }
                //G
                if (mse.G == 0)
                {
                    result.G = "inf";
                }
                else
                {
                    countedPsnrG = CountPSNR(mse.G);
                    result.G = countedPsnrG.ToString("0.###");
                }
                //B
                if (mse.B == 0)
                {
                    result.B = "inf";
                }
                else
                {
                    countedPsnrB = CountPSNR(mse.B);
                    result.B = countedPsnrB.ToString("0.###");
                }

                //Full
                var sumPsnr = (mse.R != 0 && mse.G != 0 && mse.B != 0) ? countedPsnrR + countedPsnrG + countedPsnrB : -1;
                result.Full = sumPsnr == -1 ? "inf" : (sumPsnr / 3).ToString("0.###");

                return result;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        private static double CountPSNR(int mse)
        {
            return 10 * Math.Log10(255 * 255 / mse);
        }

        private static MSE CountMSE(Bitmap firstImage, Bitmap secondImage)
        {
            try
            {
                var m = firstImage.Width;
                var n = firstImage.Height;
                var sumR = 0;
                var sumG = 0;
                var sumB = 0;

                for (var i = 0; i < m; i++)
                {
                    for (var j = 0; j < n; j++)
                    {
                        var firstPixel = firstImage.GetPixel(i, j);
                        var secondPixel = secondImage.GetPixel(i, j);
                        sumR += (firstPixel.R - secondPixel.R) * (firstPixel.R - secondPixel.R);
                        sumG += (firstPixel.G - secondPixel.G) * (firstPixel.G - secondPixel.G);
                        sumB += (firstPixel.B - secondPixel.B) * (firstPixel.B - secondPixel.B);
                    }
                }

                sumR = sumR / (m * n);
                sumB = sumB / (m * n);
                sumG = sumG / (m * n);

                return new MSE() { R = sumR, G = sumG, B = sumB };
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}

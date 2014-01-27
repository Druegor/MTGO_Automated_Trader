using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AutoItBot.OCR
{
    public static class MedianFilter
    {
        internal static BitmapData LockImage(Bitmap Image)
        {
            return Image.LockBits(new Rectangle(0, 0, Image.Width, Image.Height), ImageLockMode.ReadWrite,
                                  Image.PixelFormat);
        }

        internal static int GetPixelSize(BitmapData Data)
        {
            if (Data.PixelFormat == PixelFormat.Format24bppRgb)
            {

                return 3;
            }

            if (Data.PixelFormat == PixelFormat.Format32bppArgb
                || Data.PixelFormat == PixelFormat.Format32bppPArgb
                || Data.PixelFormat == PixelFormat.Format32bppRgb)
            {
                return 4;
            }

            return 0;
        }

        internal static unsafe Color GetPixel(BitmapData Data, int x, int y, int PixelSizeInBytes)
        {
            try
            {
                byte* DataPointer = (byte*) Data.Scan0;
                DataPointer = DataPointer + (y*Data.Stride) + (x*PixelSizeInBytes);
                if (PixelSizeInBytes == 3)
                {
                    return Color.FromArgb(DataPointer[2], DataPointer[1], DataPointer[0]);
                }
                return Color.FromArgb(DataPointer[3], DataPointer[2], DataPointer[1], DataPointer[0]);
            }
            catch
            {
                throw;
            }
        }

        internal static unsafe void SetPixel(BitmapData Data, int x, int y, Color PixelColor, int PixelSizeInBytes)
        {
            try
            {
                byte* DataPointer = (byte*) Data.Scan0;
                DataPointer = DataPointer + (y*Data.Stride) + (x*PixelSizeInBytes);
                if (PixelSizeInBytes == 3)
                {
                    DataPointer[2] = PixelColor.R;
                    DataPointer[1] = PixelColor.G;
                    DataPointer[0] = PixelColor.B;
                    return;
                }
                DataPointer[3] = PixelColor.A;
                DataPointer[2] = PixelColor.R;
                DataPointer[1] = PixelColor.G;
                DataPointer[0] = PixelColor.B;
            }
            catch
            {
                throw;
            }
        }

        internal static void UnlockImage(Bitmap Image, BitmapData ImageData)
        {
            Image.UnlockBits(ImageData);
        }

        public static Bitmap SetFilter(Bitmap OriginalImage, int Size)
        {
            try
            {
                Bitmap NewBitmap = new Bitmap(OriginalImage.Width, OriginalImage.Height);
                BitmapData NewData = LockImage(NewBitmap);
                BitmapData OldData = LockImage(OriginalImage);
                int NewPixelSize = GetPixelSize(NewData);
                int OldPixelSize = GetPixelSize(OldData);
                Random TempRandom = new Random();
                int ApetureMin = -(Size/2);
                int ApetureMax = (Size/2);
                for (int x = 0; x < NewBitmap.Width; ++x)
                {
                    for (int y = 0; y < NewBitmap.Height; ++y)
                    {
                        List<int> RValues = new List<int>();
                        List<int> GValues = new List<int>();
                        List<int> BValues = new List<int>();
                        for (int x2 = ApetureMin; x2 < ApetureMax; ++x2)
                        {
                            int TempX = x + x2;
                            if (TempX >= 0 && TempX < NewBitmap.Width)
                            {
                                for (int y2 = ApetureMin; y2 < ApetureMax; ++y2)
                                {
                                    int TempY = y + y2;
                                    if (TempY >= 0 && TempY < NewBitmap.Height)
                                    {
                                        Color TempColor = GetPixel(OldData, TempX, TempY, OldPixelSize);
                                        RValues.Add(TempColor.R);
                                        GValues.Add(TempColor.G);
                                        BValues.Add(TempColor.B);
                                    }
                                }
                            }
                        }
                        Color MedianPixel = Color.FromArgb(Median<int>(RValues),
                                                           Median<int>(GValues),
                                                           Median<int>(BValues));
                        SetPixel(NewData, x, y, MedianPixel, NewPixelSize);
                    }
                }
                UnlockImage(NewBitmap, NewData);
                UnlockImage(OriginalImage, OldData);
                return NewBitmap;
            }
            catch
            {
                throw;
            }

        }

        public static T Median<T>(List<T> Values)
        {
            if (Values.Count == 0)
                return default(T);
            Values.Sort();
            return Values[(Values.Count / 2)];
        }
    }
}
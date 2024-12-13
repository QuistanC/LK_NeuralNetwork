using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace LK_NeuronalNetwork.Utilities.ImageHelpers;

public class ImageConverter
{
    public static byte[,,]? ImageToArray(Bitmap image)
    {
#pragma warning disable CA1416 // Validate platform compatibility

        int bytesPerPixel = Image.GetPixelFormatSize(image.PixelFormat) / 8;
        Rectangle rect = new(0, 0, image.Width, image.Height);
        BitmapData imageData = image.LockBits(rect, ImageLockMode.ReadWrite, image.PixelFormat);
        int byteCount = Math.Abs(imageData.Stride) * image.Height;
        byte[] bytes = new byte[byteCount];
        Marshal.Copy(imageData.Scan0, bytes, 0, byteCount);
        image.UnlockBits(imageData);

        byte[,,] pixelValues = new byte[image.Height, image.Width, 3];
        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                int offset = y * imageData.Stride + x * bytesPerPixel;
                pixelValues[y, x, 0] = bytes[offset + 2]; //red
                pixelValues[y, x, 1] = bytes[offset + 1]; //green
                pixelValues[y, x, 2] = bytes[offset + 0]; //blue

            }
        }
#pragma warning restore CA1416 // Validate platform compatibility

        return pixelValues;
    }

    public static Bitmap ArrayToImage(byte[,,] pixelArray)
    {
#pragma warning disable CA1416 // Validate platform compatibility

        int width = pixelArray.GetLength(1);
        int height = pixelArray.GetLength(0);
        int stride = width % 4 == 0 ? width : width + 4 - width % 4;
        int bytesPerPixel = 3;

        byte[] bytes = new byte[stride * height * bytesPerPixel];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int offset = (y * stride + x) * bytesPerPixel;
                bytes[offset + 0] = pixelArray[y, x, 2]; //blue
                bytes[offset + 1] = pixelArray[y, x, 1]; //green
                bytes[offset + 2] = pixelArray[y, x, 0]; //red
            }
        }

        PixelFormat formatOutput = PixelFormat.Format24bppRgb;
        Rectangle rect = new Rectangle(0, 0, width, height);
        Bitmap image = new(stride, height, formatOutput);
        BitmapData imageData = image.LockBits(rect, ImageLockMode.ReadOnly, formatOutput);
        Marshal.Copy(bytes, 0, imageData.Scan0, bytes.Length);
        image.UnlockBits(imageData);

        Bitmap image2 = new(width, height, PixelFormat.Format32bppArgb);
        Graphics g = Graphics.FromImage(image2);
        g.DrawImage(image, 0, 0);

#pragma warning restore CA1416 // Validate platform compatibility

        return image2;
    }

    //public byte[] imageToByteArray(Image imageIn)
    //{
    //    MemoryStream ms = new MemoryStream();
    //    imageIn.Save(ms, ImageFormat.Gif);
    //    return ms.ToArray();
    //}

    //public Image byteArrayToImage(byte[] byteArrayIn)
    //{
    //    MemoryStream ms = new MemoryStream(byteArrayIn);
    //    Image returnImage = Image.FromStream(ms);
    //    return returnImage;
    //}

    public static byte[] ImageToByteArray(string imagePath)
    {
        using (Bitmap bitmap = new Bitmap(imagePath))
        {
            Bitmap resized = new Bitmap(bitmap, new Size(28, 28)); // Resize
            byte[] byteArray = new byte[28 * 28];

            for (int y = 0; y < 28; y++)
            {
                for (int x = 0; x < 28; x++)
                {
                    Color pixelColor = resized.GetPixel(x, y);
                    byte gray = (byte)((pixelColor.R + pixelColor.G + pixelColor.B) / 3); // Convert to grayscale
                    byteArray[y * 28 + x] = gray;
                }
            }
            return byteArray;
        }
    }

}

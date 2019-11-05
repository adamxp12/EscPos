using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace EscPos
{
    class ImageTools

    {

        public static string GetLogo2()
        {
            string logo = "";
            if (!File.Exists(@"C:\local\tescom.bmp"))
                return null;
            BitmapData data = GetBitmapData(@"C:\local\tescom.bmp");
            BitArray dots = data.Dots;
            byte[] width = BitConverter.GetBytes(data.Width);

            int offset = 0;
            MemoryStream stream = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(stream);
            // So we have our bitmap data sitting in a bit array called "dots."
            // This is one long array of 1s (black) and 0s (white) pixels arranged
            // as if we had scanned the bitmap from top to bottom, left to right.
            // The printer wants to see these arranged in bytes stacked three high.
            // So, essentially, we need to read 24 bits for x = 0, generate those
            // bytes, and send them to the printer, then keep increasing x. If our
            // image is more than 24 dots high, we have to send a second bit image
            // command to draw the next slice of 24 dots in the image.
            // Set the line spacing to 24 dots, the height of each "stripe" of the
            // image that we're drawing. If we don't do this, and we need to
            // draw the bitmap in multiple passes, then we'll end up with some
            // whitespace between slices of the image since the default line
            // height--how much the printer moves on a newline--is 30 dots.

            bw.Write((char)0x1B);
            bw.Write((char)0x33); // '3' just means 'change line height command'
            bw.Write((byte)24);

            // OK. So, starting from x = 0, read 24 bits down and send that data
            // to the printer. The offset variable keeps track of our global 'y'
            // position in the image. For example, if we were drawing a bitmap
            // that is 48 pixels high, then this while loop will execute twice,
            // once for each pass of 24 dots. On the first pass, the offset is
            // 0, and on the second pass, the offset is 24. We keep making
            // these 24-dot stripes until we've run past the height of the
            // bitmap.
            while (offset < data.Height)
            {
                // The third and fourth parameters to the bit image command are
                // 'nL' and 'nH'. The 'L' and the 'H' refer to 'low' and 'high', respectively.
                // All 'n' really is is the width of the image that we're about to draw.
                // Since the width can be greater than 255 dots, the parameter has to
                // be split across two bytes, which is why the documentation says the
                // width is 'nL' + ('nH' * 256).
                bw.Write((char)0x1B);
                bw.Write('*');         // bit-image mode
                bw.Write((byte)33);    // 24-dot double-density
                bw.Write(width[0]);  // width low byte
                bw.Write(width[1]);  // width high byte
                for (int x = 0; x < data.Width; ++x)
                {
                    // Remember, 24 dots = 24 bits = 3 bytes. 
                    // The 'k' variable keeps track of which of those
                    // three bytes that we're currently scribbling into.
                    for (int k = 0; k < 3; ++k)
                    {
                        byte slice = 0;
                        // A byte is 8 bits. The 'b' variable keeps track
                        // of which bit in the byte we're recording.                 
                        for (int b = 0; b < 8; ++b)
                        {
                            // Calculate the y position that we're currently
                            // trying to draw. We take our offset, divide it
                            // by 8 so we're talking about the y offset in
                            // terms of bytes, add our current 'k' byte
                            // offset to that, multiple by 8 to get it in terms
                            // of bits again, and add our bit offset to it.
                            int y = (((offset / 8) + k) * 8) + b;
                            // Calculate the location of the pixel we want in the bit array.
                            // It'll be at (y * width) + x.
                            int i = (y * data.Width) + x;
                            // If the image (or this stripe of the image)
                            // is shorter than 24 dots, pad with zero.
                            bool v = false;
                            if (i < dots.Length)
                            {
                                v = dots[i];
                            }
                            // Finally, store our bit in the byte that we're currently
                            // scribbling to. Our current 'b' is actually the exact
                            // opposite of where we want it to be in the byte, so
                            // subtract it from 7, shift our bit into place in a temp
                            // byte, and OR it with the target byte to get it into there.
                            slice |= (byte)((v ? 1 : 0) << (7 - b));
                        }
                        // Phew! Write the damn byte to the buffer
                        bw.Write(slice);
                    }
                }
                // We're done with this 24-dot high pass. Render a newline
                // to bump the print head down to the next line
                // and keep on trucking.
                offset += 24;
                bw.Write("\n");
            }
            // Restore the line spacing to the default of 30 dots.
            bw.Write((char)0x1B);
            bw.Write((char)0x32);
            bw.Write("\n");
            bw.Flush();
            byte[] bytes = stream.ToArray();
            return logo + Encoding.Default.GetString(bytes);
        }

        public static string GetLogo()
        {
            string logo = "";
            if (!File.Exists(@"C:\local\tescom.bmp"))
                return null;
            BitmapData data = GetBitmapData(@"C:\local\tescom.bmp");
            BitArray dots = data.Dots;
            byte[] width = BitConverter.GetBytes(data.Width);

            int offset = 1;
            MemoryStream stream = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(stream);

            //bw.Write((char)0x1B);
            //bw.Write('@');

            bw.Write((char)0x1B);
            bw.Write('3');
            bw.Write((byte)24);

            while (offset < data.Height)
            {
                bw.Write((char)0x1B);
                bw.Write('*');         // bit-image mode
                bw.Write((byte)33);    // 24-dot double-density
                bw.Write(width[0]);  // width low byte
                bw.Write(width[1]);  // width high byte

                for (int x = 0; x < data.Width; ++x)
                {
                    for (int k = 0; k < 3; ++k)
                    {
                        byte slice = 0;
                        for (int b = 0; b < 8; ++b)
                        {
                            int y = (((offset / 8) + k) * 8) + b;
                            // Calculate the location of the pixel we want in the bit array.
                            // It'll be at (y * width) + x.
                            int i = (y * data.Width) + x;

                            // If the image is shorter than 24 dots, pad with zero.
                            bool v = false;
                            if (i < dots.Length)
                            {
                                v = dots[i];
                            }
                            slice |= (byte)((v ? 1 : 0) << (7 - b));
                        }

                        bw.Write(slice);
                    }
                }
                offset += 24;
                bw.Write((char)0x0A);
            }
            // Restore the line spacing to the default of 30 dots.
            bw.Write((char)0x1B);
            bw.Write((byte)0x32);

            bw.Flush();
            byte[] bytes = stream.ToArray();
            return logo + Encoding.Default.GetString(bytes);
        }

        private static BitmapData GetBitmapData(string bmpFileName)
        {
            using (var bitmap = (Bitmap)Bitmap.FromFile(bmpFileName))
            {
                var threshold = 127;
                var index = 0;
                var dimensions = bitmap.Width * bitmap.Height;
                var dots = new BitArray(dimensions);
                for (var y = 0; y < bitmap.Height; y++)
                {
                    for (var x = 0; x < bitmap.Width; x++)
                    {
                        var color = bitmap.GetPixel(x, y);
                        var luminance = (int)(color.R * 0.3 + color.G * 0.59 + color.B * 0.11);
                        dots[index] = (luminance < threshold);
                        index++;
                    }
                }
                return new BitmapData()
                {
                    Dots = dots,
                    Height = bitmap.Height,
                    Width = bitmap.Width
                };
            }
        }
    }

    public class BitmapData
    {
        public BitArray Dots
        {
            get;
            set;
        }

        public int Height
        {
            get;
            set;
        }

        public int Width
        {
            get;
            set;
        }
    }
}

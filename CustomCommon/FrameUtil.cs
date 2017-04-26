using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Media.Imaging;

namespace HDVietNam
{
    public static class FrameUtil
    {
        public static byte[] ImageStart = new byte[] { 0x50, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x50 };

        public static byte[] ImageEncoder(int width, int height, System.Windows.Media.PixelFormat format, byte[] pixels, int dpix = 96, int dpiy = 96)
        {
            try
            {
                int stride = width * 4;
                BitmapSource img = BitmapSource.Create(width, height, dpix, dpiy, format, null, pixels, stride);
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(img));
                byte[] encoderData = null;
                using (MemoryStream stream = new MemoryStream())
                {
                    encoder.Save(stream);
                    stream.Position = 0;
                    encoderData = new byte[stream.Length];
                    stream.Read(encoderData, 0, encoderData.Length);
                }
                if (encoderData != null && encoderData.Length > 0)
                    return encoderData;
            }
            catch { }
            return null;
        }

        public static ImageResultWithData ImageDecoder(Stream stream)
        {
            try
            {
                PngBitmapDecoder decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.None, BitmapCacheOption.Default);
                if (decoder.Frames.Count > 0)
                {
                    BitmapSource image = decoder.Frames[0];
                    var result = new ImageResultWithData()
                    {
                        Format = image.Format == System.Windows.Media.PixelFormats.Bgra32 || image.Format == System.Windows.Media.PixelFormats.Bgr32 ? PixelFormat.bgra : PixelFormat.rgba,
                        Width = image.PixelWidth,
                        Height = image.PixelHeight,
                        Pixels = new byte[image.PixelWidth * image.PixelHeight * 4]
                    };
                    image.CopyPixels(result.Pixels, result.Width * 4, 0);
                    return result;
                }
            }
            catch { }
            return null;
        }

        public static ImageResultWithData ImageDecoder(byte[] data)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    stream.Write(data, 0, data.Length);
                    stream.Position = 0;
                    return ImageDecoder(stream);
                }
            }
            catch { }
            return null;
        }

        public static void RgbaToBgra(byte[] pixels)
        {
            for (int i = 0; i < pixels.Length; i += 4)
            {
                var tmp = pixels[i];
                pixels[i] = pixels[i + 2];
                pixels[i + 2] = tmp;
            }
        }

        public static void RealViewAlphaImage(byte[] pixels)
        {
            for (int i = 0; i < pixels.Length; i += 4)
            {
                var alpha = pixels[i + 3];
                if(alpha != 255)
                {
                    pixels[i] = (byte)(pixels[i] * alpha / 255);
                    pixels[i + 1] = (byte)(pixels[i + 1] * alpha / 255);
                    pixels[i + 2] = (byte)(pixels[i + 2] * alpha / 255);
                }
            }
        }

        public static byte[] ImageEncoder2(int width, int height, System.Windows.Media.PixelFormat format, byte[] pixels, int dpix = 96, int dpiy = 96)
        {
            try
            {
                byte[] colorChannel = new byte[width * height * 3];
                byte[] alphaChannel = null;
                if (format == System.Windows.Media.PixelFormats.Bgra32 || format == System.Windows.Media.PixelFormats.Bgr32)
                    alphaChannel = new byte[width * height];
                int npixels = width * height;
                int add = alphaChannel == null ? 3 : 4;
                for (int i = 0; i < npixels; ++i)
                {
                    colorChannel[i * 3 + 0] = pixels[i * add + 0];
                    colorChannel[i * 3 + 1] = pixels[i * add + 1];
                    colorChannel[i * 3 + 2] = pixels[i * add + 2];
                    if (alphaChannel != null)
                        alphaChannel[i] = pixels[i * add + 3];
                }

                byte[] colorData = null;
                if (colorChannel != null)
                {
                    BitmapSource imgColor = BitmapSource.Create(width, height, dpix, dpiy
                        , format == System.Windows.Media.PixelFormats.Bgr32
                            || format == System.Windows.Media.PixelFormats.Bgra32
                            || format == System.Windows.Media.PixelFormats.Bgr24
                            ? System.Windows.Media.PixelFormats.Bgr24
                            : System.Windows.Media.PixelFormats.Rgb24
                        , null, colorChannel, width * 3);
                    JpegBitmapEncoder encoderColor = new JpegBitmapEncoder();
                    encoderColor.Frames.Add(BitmapFrame.Create(imgColor));
                    using (MemoryStream stream = new MemoryStream())
                    {
                        encoderColor.Save(stream);
                        colorData = stream.ToArray();
                    }
                }

                byte[] alphaData = null;
                if(alphaChannel != null)
                {
                    BitmapSource imgAlpha = BitmapSource.Create(width, height, dpix, dpiy
                        , System.Windows.Media.PixelFormats.Gray8, null, alphaChannel, width);
                    JpegBitmapEncoder encoderAlpha = new JpegBitmapEncoder();
                    encoderAlpha.Frames.Add(BitmapFrame.Create(imgAlpha));
                    using (MemoryStream stream = new MemoryStream())
                    {
                        encoderAlpha.Save(stream);
                        alphaData = stream.ToArray();
                    }
                }

                if (colorData != null)
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        stream.Write(BitConverter.GetBytes(colorData.Length), 0, 4);
                        stream.Write(colorData, 0, colorData.Length);
                        if (alphaData != null)
                        {
                            stream.Write(BitConverter.GetBytes(alphaData.Length), 0, 4);
                            stream.Write(alphaData, 0, alphaData.Length);
                        }
                        return stream.ToArray();
                    }
                }
            }
            catch { }

            return null;
        }
        
        public static ImageResultWithData ImageDecoder2(byte[] data)
        {
            try
            {
                int colorDataLength = BitConverter.ToInt32(data, 0);
                if (colorDataLength > 0 && data.Length - 4 >= colorDataLength)
                {
                    int width = 0, height = 0;
                    byte[] colorChannel = null;
                    System.Windows.Media.PixelFormat colorFormat = System.Windows.Media.PixelFormats.Bgra32;

                    using (MemoryStream memory = new MemoryStream(data, 4, colorDataLength))
                    {
                        JpegBitmapDecoder decoder = new JpegBitmapDecoder(memory, BitmapCreateOptions.None, BitmapCacheOption.Default);
                        if (decoder.Frames.Count > 0)
                        {
                            BitmapSource image = decoder.Frames[0];
                            width = image.PixelWidth;
                            height = image.PixelHeight;
                            colorChannel = new byte[width * height * 4];
                            image.CopyPixels(colorChannel, width * 4, 0);
                        }
                    }

                    byte[] alphaChannel = null;
                    if (data.Length - 4 > colorDataLength)
                    {
                        int alphaDataLength = BitConverter.ToInt32(data, 4 + colorDataLength);
                        if (alphaDataLength > 0 && data.Length - 8 - colorDataLength == alphaDataLength)
                        {
                            using (MemoryStream memory = new MemoryStream(data, 8 + colorDataLength, alphaDataLength))
                            {
                                JpegBitmapDecoder decoder = new JpegBitmapDecoder(memory, BitmapCreateOptions.None, BitmapCacheOption.Default);
                                if (decoder.Frames.Count > 0)
                                {
                                    BitmapSource image = decoder.Frames[0];
                                    if (image.PixelWidth == width && image.PixelHeight == height)
                                    {
                                        alphaChannel = new byte[width * height];
                                        image.CopyPixels(alphaChannel, width, 0);
                                    }
                                }
                            }
                        }
                    }

                    if (alphaChannel != null)
                    {
                        for (int i = 0; i < width * height; ++i)
                        {
                            colorChannel[i * 4 + 3] = alphaChannel[i];
                        }
                    }

                    return new ImageResultWithData()
                    {
                        Format = colorFormat == System.Windows.Media.PixelFormats.Bgra32 ? PixelFormat.bgra : PixelFormat.rgba,
                        Width = width,
                        Height = height,
                        Pixels = colorChannel
                    };
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Decode error: " + ex.Message);
            }
            return null;
        }
    }

    public class ImageInput
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public System.Windows.Media.PixelFormat Format { get; set; }

        public byte[] Pixels { get; set; }

        public int DpiX { get; set; }

        public int DpiY { get; set; }
    }

    public class EncoderOutputArgs : EventArgs
    {
        public byte[] Data { get; set; }
    }
    
    public class ImageEncoder : IDisposable
    {
        public event EventHandler<EncoderOutputArgs> OnOutput;

        const int NumberThread = 2;

        Queue<ImageInput> queueImages = new Queue<ImageInput>();

        public ImageEncoder()
        {
            _Running = true;

            thrCompress = new Thread(CompressThread);
            thrCompress.IsBackground = true;
            thrCompress.Start();
        }

        bool _Running = true;

        ~ImageEncoder()
        {
            Dispose();
        }

        public void Dispose()
        {
            _Running = false;

            lock(lockThread)
            {
                Monitor.PulseAll(lockThread);
            }

            lock(queueImages)
            {
                Monitor.PulseAll(queueImages);
            }

            thrCompress.Join();
        }

        public void Push(ImageInput image)
        {
            if (_Running)
            {
                lock(queueImages)
                {
                    if (queueImages.Count < 10)
                    {
                        queueImages.Enqueue(image);
                        Monitor.PulseAll(queueImages);
                    }
                    else
                        Console.WriteLine("Skip frame");
                }
            }
        }

        public void WaitPush()
        {
            if(_Running)
            {
                lock(queueImages)
                {
                    while (_Running && queueImages.Count >= 10)
                        Monitor.Wait(queueImages);
                }
            }
        }

        int nbThread = 0;
        object lockThread = new object();

        Thread thrCompress = null;
        void CompressThread()
        {
            Thread lastThread = null;
            while(_Running)
            {
                ImageInput img = null;
                lock (queueImages)
                {
                    while (_Running && queueImages.Count == 0)
                        Monitor.Wait(queueImages);
                    if (_Running)
                    {
                        img = queueImages.Dequeue();
                        Monitor.PulseAll(queueImages);
                    }
                }

                if (img != null)
                {
                    Thread nextThread = new Thread(new ParameterizedThreadStart(CompressOne));
                    nextThread.IsBackground = true;
                    nextThread.Start(new CompressOneParameter()
                    {
                        LastThread = lastThread,
                        Image = img
                    });

                    lastThread = nextThread;

                    lock(lockThread)
                    {
                        nbThread++;
                        while (_Running && nbThread >= NumberThread)
                            Monitor.Wait(lockThread);
                    }
                }
            }
        }

        class CompressOneParameter
        {
            public Thread LastThread { get; set; }

            public ImageInput Image { get; set; }
        }

        void CompressOne(object param)
        {
            CompressOneParameter par = (CompressOneParameter)param;
            var data = FrameUtil.ImageEncoder2(par.Image.Width, par.Image.Height, par.Image.Format, par.Image.Pixels, par.Image.DpiX, par.Image.DpiY);
            if (par.LastThread != null)
            {
                try
                {
                    par.LastThread.Join();
                }
                catch { }
            }
            if (OnOutput != null)
                OnOutput(this, new EncoderOutputArgs()
                {
                    Data = data
                });
            lock(lockThread)
            {
                nbThread--;
                Monitor.PulseAll(lockThread);
            }
        }
    }
}

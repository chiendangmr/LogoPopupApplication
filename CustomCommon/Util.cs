using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace HDVietNam
{
    public static class Util
    {
        public static IntPtr ObjectToPtr<T>(this T obj)
        {
            if (obj == null)
                return IntPtr.Zero;

            GCHandle handle = GCHandle.Alloc(obj);
            return (IntPtr)handle;
        }

        public static T PtrToObject<T>(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                return default(T);

            GCHandle handle = (GCHandle)ptr;
            return (T)handle.Target;
        }

        public static int DiagnosticsColor(float r, float g, float b, float a)
        {
            int code = 0;
            code |= (((int)(r * 255.0f + 0.5f)) << 24);
            code |= (((int)(g * 255.0f + 0.5f)) << 16);
            code |= (((int)(b * 255.0f + 0.5f)) << 8);
            code |= (((int)(a * 255.0f + 0.5f)) << 0);
            return code;
        }

        public static int DiagnosticsColor(float r, float g, float b)
        {
            return DiagnosticsColor(r, g, b, 1.0f);
        }

        public static int DiagnosticsColorRed(int color)
        {
            return (color >> 24) & 0xff;
        }

        public static int DiagnosticsColorGreen(int color)
        {
            return (color >> 16) & 0xff;
        }

        public static int DiagnosticsColorBlue(int color)
        {
            return (color >> 8) & 0xff;
        }

        public static int DiagnosticsColorAlpha(int color)
        {
            return (color >> 0) & 0xff;
        }

        public static string ObjectToXml<T>(this T obj)
        {
            string xmlStr = string.Empty;

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = false;
            settings.OmitXmlDeclaration = true;
            settings.NewLineChars = string.Empty;
            settings.NewLineHandling = NewLineHandling.None;

            using (StringWriter stringWriter = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, settings))
                {
                    XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                    namespaces.Add(string.Empty, string.Empty);

                    XmlSerializer serializer = new XmlSerializer(obj.GetType());
                    serializer.Serialize(xmlWriter, obj, namespaces);

                    xmlStr = stringWriter.ToString();
                    xmlWriter.Close();
                }

                stringWriter.Close();
            }

            return xmlStr;
        }

        public static T ObjectFromXml<T>(string xml)
        {
            T obj = default(T);

            using (StringReader stringReader = new StringReader(xml))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                obj = (T)serializer.Deserialize(stringReader);

                stringReader.Close();
            }

            return obj;
        }

        public static string CompressString(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            var memoryStream = new MemoryStream();
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                gZipStream.Write(buffer, 0, buffer.Length);
            }

            memoryStream.Position = 0;

            var compressedData = new byte[memoryStream.Length];
            memoryStream.Read(compressedData, 0, compressedData.Length);

            var gZipBuffer = new byte[compressedData.Length + 4];
            Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
            return Convert.ToBase64String(gZipBuffer);
        }

        public static string DecompressString(string compressedText)
        {
            byte[] gZipBuffer = Convert.FromBase64String(compressedText);
            using (var memoryStream = new MemoryStream())
            {
                int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

                var buffer = new byte[dataLength];

                memoryStream.Position = 0;
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    gZipStream.Read(buffer, 0, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer);
            }
        }

        public static string CompressConsoleString(string str)
        {
            return str.Replace("\"", "<<>>").Replace("\r", "<<r>>").Replace("\n", "<<n>>");
        }

        public static string DecompressConsoleString(string str)
        {
            return str.Replace("<<n>>", "\n").Replace("<<r>>", "\n").Replace("<<>>", "\"");
        }

        public delegate string ReadLineDelegate();

        public static string ReadLineWithTimeOut(int timeOutMs)
        {
            ReadLineDelegate d = Console.ReadLine;
            IAsyncResult result = d.BeginInvoke(null, null);
            result.AsyncWaitHandle.WaitOne(timeOutMs);//timeout e.g. 15000 for 15 secs
            if (result.IsCompleted)
            {
                string resultstr = d.EndInvoke(result);
                return resultstr;
            }
            else
                return "";
        }

        public static byte[] ObjectToByteArray(this object obj)
        {
            try
            {
                if (obj != null)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    byte[] result = null;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        bf.Serialize(ms, obj);
                        result = ms.ToArray();
                    }
                    return result;
                }
            }
            catch { }
            return null;
        }

        public static object ByteArrayToObject(this byte[] arrByte)
        {
            try
            {
                object result = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    ms.Write(arrByte, 0, arrByte.Length);
                    ms.Position = 0;
                    BinaryFormatter bf = new BinaryFormatter();
                    result = bf.Deserialize(ms);
                }
                return result;
            }
            catch { }
            return null;
        }

        public static byte[] CompressData(byte[] data)
        {
            byte[] result = null;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
                {
                    gZipStream.Write(data, 0, data.Length);
                }

                result = memoryStream.ToArray();
            }

            return result;
        }

        public static byte[] DecompressData(byte[] data)
        {
            byte[] result = null;
            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Write(data, 0, data.Length);
                memoryStream.Position = 0;
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    const int size = 4096;
                    byte[] buffer = new byte[size];
                    using (MemoryStream ostream = new MemoryStream())
                    {
                        int count = 0;
                        do
                        {
                            count = gZipStream.Read(buffer, 0, size);
                            if (count > 0)
                                ostream.Write(buffer, 0, count);
                        }
                        while (count > 0);
                        result = ostream.ToArray();
                    }
                }
            }
            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDVietNam
{
    public class ImageResultWithData
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public byte[] Pixels { get; set; }

        public PixelFormat Format { get; set; }

        public ImageResultWithData()
        {
            Format = PixelFormat.rgba;
            Width = 0;
            Height = 0;
            Pixels = null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDVietNam
{
    public class ConstFrame
    {
        public PixelFormat PixelFormat { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public byte[] Pixels { get; set; }

        public PointF Position { get; set; }

        public SizeF Size { get; set; }

        public PointF CropStart { get; set; }
        public PointF CropEnd { get; set; }

        public double Opacity { get; set; }

        public ConstFrame()
        {
            Pixels = null;
            Position = new PointF() { X = 0.0, Y = 0.0 };
            Size = new SizeF() { Width = 1.0, Height = 1.0 };
            CropStart = new PointF() { X = 0.0, Y = 0.0 };
            CropEnd = new PointF() { X = 1.0, Y = 1.0 };
            Opacity = 1.0;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDVietNam
{
    public class SizeF
    {
        public double Width { get; set; }

        public double Height { get; set; }

        public SizeF()
        {
            Width = 0.0;
            Height = 0.0;
        }

        public SizeF(double width, double height)
        {
            this.Width = width;
            this.Height = height;
        }

        public override string ToString()
        {
            return Width.ToString() + " x " + Height.ToString();
        }
    }
}

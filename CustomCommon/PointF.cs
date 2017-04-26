using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDVietNam
{
    public class PointF
    {
        public double X { get; set; }

        public double Y { get; set; }

        public PointF()
        {
            X = 0.0;
            Y = 0.0;
        }

        public PointF(double x, double y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return X.ToString() + " , " + Y.ToString();
        }
    }
}

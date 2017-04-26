using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDVietNam
{
    public class GraphParameterArgs : EventArgs
    {
        public string Name { get; set; }

        public double Value { get; set; }
    }
}

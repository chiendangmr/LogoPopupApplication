using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDVietNam
{
    public class GraphParameter
    {
        public GraphParameter()
        {

        }

        public GraphParameter(string name, int color)
        {
            this.Name = name;
            this.Color = color;
        }

        public string Name { get; set; }

		public int Color { get; set; }
    }
}

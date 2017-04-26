using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDVietNam
{
    public class InfoParameter
    {
        public InfoParameter()
        {

        }

        public InfoParameter(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        public string Name { get; set; }

		public string Value { get; set; }
    }
}

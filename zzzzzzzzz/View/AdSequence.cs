using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zzzzzzzzz.View
{
    public class AdSequence
    {
        public string StartTime { get; set; }
        public lstAdd lstAddObj
        {
            get; set;
        }
    }
    public class lstAdd
    {
        public AdObject adObj { get; set; }
    }
    public class AdObject
    {
        public string FileName { get; set; }
        public long Duration { get; set; }
        public string State { get; set; }
    }
}

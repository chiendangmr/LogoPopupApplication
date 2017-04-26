using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDVietNam
{
    public class LogArgs : EventArgs
    {
        public LogLevel Level { get; set; }

        public string Message { get; set; }
    }
}

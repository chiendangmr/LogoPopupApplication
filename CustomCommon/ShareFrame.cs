using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDVietNam
{
    public class ShareFrame
    {
        public ConstFrame Image { get; set; }

        public List<ShareFrame> Interlace { get; set; }

        public List<ShareFrame> Frames { get; set; }

        public ShareFrame()
        {
            Image = null;
            Interlace = null;
            Frames = null;
        }
    }
}

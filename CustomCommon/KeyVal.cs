using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDVietNam
{
    public class KeyVal<Key, Val>
    {
        public Key Id { get; set; }

        public Val Value { get; set; }

        public KeyVal()
        {

        }

        public KeyVal(Key key, Val val)
        {
            this.Id = key;
            this.Value = val;
        }
    }
}

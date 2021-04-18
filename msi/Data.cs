using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace msi
{
    [Serializable]
    class Data
    {
        public Data()
        {
            Q = new Set();
            R = new Set();
        }
        public int Number { get; set; }
        public string Name { get; set; }
        public Set Q { get; set; }
        public Set R { get; set; }

        public Data Clone()
        {
            Data clone = new Data();
            clone.Name = Name;
            clone.Number = Number;
            clone.Q = Q.Clone();
            clone.R = R.Clone();
            return clone;
        }
    }
}

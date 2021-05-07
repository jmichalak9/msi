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
            Q = new InputSet();
            R = new InputSet();
        }
        public int Number { get; set; }
        public string Name { get; set; }
        public InputSet Q { get; set; }
        public InputSet R { get; set; }

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

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
            Q = new InputSet<float>();
            R = new InputSet<float>();
        }
        public int Number { get; set; }
        public string Name { get; set; }
        public InputSet<float> Q { get; set; }
        public InputSet<float> R { get; set; }

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

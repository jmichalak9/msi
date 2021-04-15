using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace msi
{
    struct Set
    {
        public int ColCount { get => ColNames.Length; }
        public int RowCount { get => RowNames.Length; }
        public string[] ColNames { get; set; }
        public string[] RowNames { get; set; }
        public float[,] Numbers { get; set; }
    }
}

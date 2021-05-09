﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace msi
{
    [Serializable]
    class InputSet
    {
        public InputSet()
        {
            ColNames = new string[0];
            RowNames = new string[0];
            Numbers = new float[0, 0];
        }
        public int ColCount { get => ColNames.Length; }
        public int RowCount { get => RowNames.Length; }
        public string[] ColNames { get; set; }
        public string[] RowNames { get; set; }
        public float[,] Numbers { get; set; }

        public InputSet Clone()
        {
            InputSet clone = new InputSet();
            clone.ColNames = (string[])ColNames.Clone();
            clone.RowNames = (string[])RowNames.Clone();
            clone.Numbers = (float[,])Numbers.Clone();
            return clone;
        }
    }
}

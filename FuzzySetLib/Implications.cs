using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuzzySetLib
{
    public class Implications
    {
        public static float Godel(float x, float y)
        {
            CheckImplArguments(x, y);
            if (x <= y)
                return 1;
            return y;
        }
		
        public static float GoguenGaines(float x, float y)
        {
            CheckImplArguments(x, y);
            if (x <= y)
                return 1;
            return y/x;
        }

        public static float Lukasiewicz(float x, float y)
        {
            CheckImplArguments(x, y);
            return Math.Min(1, 1 - x + y);
        }

        private static void CheckImplArguments(float x, float y)
        {
            if (x < 0 || x > 1)
                throw new ArgumentException($"invalid x value: {x}");
            if (y < 0 || y > 1)
                throw new ArgumentException($"invalid y value: {y}");
        }
    }
}

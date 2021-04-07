using System;
using System.Collections.Generic;
using System.Linq;

namespace FuzzySetLib
{
    public struct Bounds
    {
        public float lower;
        public float upper;
    }
    public class Approximations
    {

        public static Bounds[,] ObjectBoundApproximation(Func<float,float,float> norm, Func<float,float,float> impl, float[,] Q, float[,] R)
        {
            var approx = new Bounds[R.GetLength(0),R.GetLength(1)];

            for (int i = 0; i < R.GetLength(0); i++)
            {
                var r = Enumerable.Range(0, R.GetLength(1))
                    .Select(x => R[i, x])
                    .ToArray();
                var lowerApprox = ObjectLowerApprox(norm, impl, Q, r);
                var upperApprox = ObjectUpperApprox(norm, impl, Q, r);
                for (int j = 0; j < r.GetLength(0); j++)
                {
                    approx[i,j].lower = lowerApprox[j];
                    approx[i,j].upper = upperApprox[j];
                }
            }

            return approx;
        }

        // aka triangle pointed up
        private static float[] ObjectLowerApprox(Func<float, float, float> norm, Func<float, float, float> impl, float[,] Q, float[] R)
        {
            var necessities = new List<float>();
            for (int j = 0; j < R.GetLength(0); j++)
            {
                necessities.Add(FuzzyNecessity(impl, Q, R, j));
            }
            var possibilities = new List<float>();
            for (int j = 0; j < R.GetLength(0); j++)
            {
                possibilities.Add(FuzzyPossibility(norm, Transpose(Q), necessities.ToArray(), j));
            }
            return possibilities.ToArray();
        }

        // aka triangle pointed down
        private static float[] ObjectUpperApprox(Func<float, float, float> norm, Func<float, float, float> impl, float[,] Q, float[] R)
        {
            var possibilities = new List<float>();
            for (int j = 0; j < R.GetLength(0); j++)
            {
                possibilities.Add(FuzzyPossibility(norm, Q, R, j));
            }
            var necessities = new List<float>();
            for (int j = 0; j < R.GetLength(0); j++)
            {
                necessities.Add(FuzzyNecessity(impl, Transpose(Q), possibilities.ToArray(), j));
            }
            return necessities.ToArray();
        }

        // aka square brackets operator
        private static float FuzzyNecessity(Func<float,float,float> impl, float[,] Q, float[] A, int x)
        {
            float necessity = float.MaxValue;
            for (int y = 0; y < Q.GetLength(1); y++)
                necessity = Math.Min(necessity, impl(Q[x, y], A[y]));
            return necessity;
        }

        // aka triangle brackets operator
        private static float FuzzyPossibility(Func<float, float, float> norm, float[,] Q, float[] A, int x)
        {
            float possibility = float.MinValue;
            for (int y = 0; y < Q.GetLength(1); y++)
                possibility = Math.Max(possibility, norm(Q[x, y], A[y]));
            return possibility;
        }

        private static float[,] Transpose(float[,] matrix)
        {
            int w = matrix.GetLength(0);
            int h = matrix.GetLength(1);

            float[,] result = new float[h, w];

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    result[j, i] = matrix[i, j];
                }
            }

            return result;
        }
    }
}

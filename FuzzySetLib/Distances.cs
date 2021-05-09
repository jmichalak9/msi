using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuzzySetLib
{
    public class Distances
    {
        public static float[,] HammingDistance(Func<float, float, float> norm, Func<float, float, float> impl, float[,] Q, float[,] R)
        {
            float[,] distances = new float[Q.GetLength(0), R.GetLength(0)];

            var objectBounds = Approximations.ObjectBoundApproximation(norm, impl, Q, R);
            var propertyBounds = Approximations.PropertyBoundApproximation(norm, impl, Q, R);

            for(int i=0;i<distances.GetLength(0);i++)
            {
                for(int j=0;j<distances.GetLength(1);j++)
                {
                    for(int k=0;k<objectBounds.GetLength(1);k++)
                    {
                        distances[i, j] += HammingDistance(propertyBounds[i, k], objectBounds[j, k]);
                    }
                    distances[i, j] /= 2 * objectBounds.GetLength(1);
                }
            }

            return distances;
        }

        public static float[,] EuclideanDistance(Func<float, float, float> norm, Func<float, float, float> impl, float[,] Q, float[,] R)
        {
            float[,] distances = new float[Q.GetLength(0), R.GetLength(0)];

            var objectBounds = Approximations.ObjectBoundApproximation(norm, impl, Q, R);
            var propertyBounds = Approximations.PropertyBoundApproximation(norm, impl, Q, R);

            for (int i = 0; i < distances.GetLength(0); i++)
            {
                for (int j = 0; j < distances.GetLength(1); j++)
                {
                    for (int k = 0; k < objectBounds.GetLength(1); k++)
                    {
                        distances[i, j] += EuclideanDistance(propertyBounds[i, k], objectBounds[j, k]);
                    }
                    distances[i, j] = (float)Math.Sqrt(distances[i, j] / (2 * objectBounds.GetLength(1)));
                }
            }

            return distances;
        }

        private static float HammingDistance(Bounds A, Bounds B)
        {
            return Math.Abs(A.upper - B.upper) + Math.Abs(A.lower - B.lower);
        }

        private static float EuclideanDistance(Bounds A, Bounds B)
        {
            return (A.upper - B.upper) * (A.upper - B.upper) + (A.lower - B.lower) * (A.lower - B.lower);
        }

    }
}

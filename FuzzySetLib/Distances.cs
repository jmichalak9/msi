using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuzzySetLib
{
    public class Distances
    {
        public static float[,] CalculateDistance(Func<Bounds,Bounds, float> distance, Func<float, float, float> norm, Func<float, float, float> impl, float[,] Q, float[,] R)
        {
            float[,] distances = new float[R.GetLength(0), R.GetLength(1)];

            var objectBounds = Approximations.ObjectBoundApproximation(norm, impl, Q, R);
            var propertyBounds = Approximations.PropertyBoundApproximation(norm, impl, Q, R);

            for(int i=0;i<distances.GetLength(0);i++)
            {
                for(int j=0;i<distances.GetLength(1);j++)
                {
                    distances[i, j] = distance(objectBounds[i, j], propertyBounds[i, j]);
                }
            }

            return distances;
        }

        public float HammingDistance(Bounds A, Bounds B)
        {
            return Math.Abs(A.upper - B.upper) + Math.Abs(A.lower - B.lower);
        }

        public float EuclideanDistance(Bounds A, Bounds B)
        {
            return (A.upper - B.upper) * (A.upper - B.upper) + (A.lower - B.lower) * (A.lower - B.lower);
        }

    }
}

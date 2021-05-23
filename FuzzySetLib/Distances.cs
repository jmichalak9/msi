
using System;

namespace FuzzySetLib
{
	public class Distances
	{
		public static float[,] HammingSetDistance(Bounds[,] objectBounds, Bounds[,] propertyBounds)
		{
			float[,] distances = new float[objectBounds.GetLength(0), propertyBounds.GetLength(0)];

			for (int i = 0; i < distances.GetLength(0); i++)
			{
				for (int j = 0; j < distances.GetLength(1); j++)
				{
					for (int k = 0; k < objectBounds.GetLength(1); k++)
					{
						distances[i, j] += HammingBoundDistance(propertyBounds[j, k], objectBounds[i, k]);
					}
					distances[i, j] /= 2 * objectBounds.GetLength(1);
				}
			}

			return distances;
		}

		public static float[,] EuclideanSetDistance(Bounds[,] objectBounds, Bounds[,] propertyBounds)
		{
			float[,] distances = new float[objectBounds.GetLength(0), propertyBounds.GetLength(0)];

			for (int i = 0; i < distances.GetLength(0); i++)
			{
				for (int j = 0; j < distances.GetLength(1); j++)
				{
					for (int k = 0; k < objectBounds.GetLength(1); k++)
					{
						distances[i, j] += EuclideanBoundDistance(propertyBounds[j, k], objectBounds[i, k]);
					}
					distances[i, j] = (float)Math.Sqrt(distances[i, j] / (2 * objectBounds.GetLength(1)));
				}
			}

			return distances;
		}

		private static float HammingBoundDistance(Bounds A, Bounds B)
		{
			return Math.Abs(A.upper - B.upper) + Math.Abs(A.lower - B.lower);
		}

		private static float EuclideanBoundDistance(Bounds A, Bounds B)
		{
			return (A.upper - B.upper) * (A.upper - B.upper) + (A.lower - B.lower) * (A.lower - B.lower);
		}

	}
}

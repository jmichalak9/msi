
using System;

namespace FuzzySetLib
{
	public class Norms
	{
		public static float Standard(float x, float y)
		{
			CheckNormArguments(x, y);
			return Math.Min(x, y);
		}

		public static float Product(float x, float y)
		{
			CheckNormArguments(x, y);
			return x * y;
		}

		public static float Lukasiewicz(float x, float y)
		{
			CheckNormArguments(x, y);
			return Math.Max(0, x + y - 1);
		}

		private static void CheckNormArguments(float x, float y)
		{
			if (x < 0 || x > 1)
				throw new ArgumentException($"invalid x value: {x}");
			if (y < 0 || y > 1)
				throw new ArgumentException($"invalid y value: {y}");
		}
	}
}

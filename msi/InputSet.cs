
using System;

namespace msi
{
	[Serializable]
	class InputSet<T>
	{
		public InputSet()
		{
			ColNames = Array.Empty<string>();
			RowNames = Array.Empty<string>();
			Numbers = new T[0, 0];
		}
		public int ColCount { get => ColNames.Length; }
		public int RowCount { get => RowNames.Length; }
		public string[] ColNames { get; set; }
		public string[] RowNames { get; set; }
		public T[,] Numbers { get; set; }

		public InputSet<T> Clone()
		{
			InputSet<T> clone = new InputSet<T>
			{
				ColNames = (string[])ColNames.Clone(),
				RowNames = (string[])RowNames.Clone(),
				Numbers = (T[,])Numbers.Clone()
			};
			return clone;
		}
	}
}

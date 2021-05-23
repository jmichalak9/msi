
using System;

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
			Data clone = new Data
			{
				Name = Name,
				Number = Number,
				Q = Q.Clone(),
				R = R.Clone()
			};
			return clone;
		}
	}
}

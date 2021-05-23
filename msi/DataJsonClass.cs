
using System.Collections.Generic;
using System.Linq;

namespace msi
{
    class DataJsonClass
    {
        public static implicit operator DataJsonClass(Data data)
        {
            float[][] candidatesRates = new float[data.Q.RowNames.Length][];
            for (int i = 0; i < data.Q.RowNames.Length; i++)
            {
                candidatesRates[i] = new float[data.Q.ColNames.Length];
                for (int j = 0; j < data.Q.ColNames.Length; j++)
                {
                    candidatesRates[i][j] = data.Q.Numbers[i,j];
                }
            }
            float[][] jobPositionsRates = new float[data.R.RowNames.Length][];
            for (int i = 0; i < data.R.RowNames.Length; i++)
            {
                jobPositionsRates[i] = new float[data.R.ColNames.Length];
                for (int j = 0; j < data.R.ColNames.Length; j++)
                {
                    jobPositionsRates[i][j] = data.R.Numbers[i, j];
                }
            }
            return new DataJsonClass
            {
                Candidates = data.Q.RowNames.ToList(),
                JobPositions = data.R.RowNames.ToList(),
                Skills = data.Q.ColNames.ToList(),
                CandidatesRates = candidatesRates,
                JobPositionsRates = jobPositionsRates
            };
        }
        public static implicit operator Data(DataJsonClass data)
        {
            float[,] QNumbers = new float[data.Candidates.Count, data.Skills.Count];
            for (int i = 0; i < data.Candidates.Count; i++)
            {
                for (int j = 0; j < data.Skills.Count; j++)
                {
                    QNumbers[i, j] = data.CandidatesRates[i][j];
                }
            }
            InputSet<float> Q = new InputSet<float>
            {
                ColNames = data.Skills.ToArray(),
                RowNames = data.Candidates.ToArray(),
                Numbers = QNumbers
            };
            float[,] RNumbers = new float[data.JobPositions.Count, data.Skills.Count];
            for (int i = 0; i < data.JobPositions.Count; i++)
            {
                for (int j = 0; j < data.Skills.Count; j++)
                {
                    RNumbers[i, j] = data.JobPositionsRates[i][j];
                }
            }
            InputSet<float> R = new InputSet<float>
            {
                ColNames = data.Skills.ToArray(),
                RowNames = data.JobPositions.ToArray(),
                Numbers = RNumbers
            };
            return new Data
            {
                Q = Q,
                R = R
            };
        }
        public List<string> Candidates { get; set; }
        public List<string> JobPositions { get; set; }
        public List<string> Skills { get; set; }
        public float[][] CandidatesRates { get; set; }
        public float[][] JobPositionsRates { get; set; }
    }
}

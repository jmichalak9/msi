using FuzzySetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace FuzzySetLibTests
{
    public class DistancesTest
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly int _precision = 2;
        public DistancesTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void HammingDistanceTest()
        {
            var Q = new float[,]
            {
                  { 0.4f, 0.3f, 0.1f, 0.4f, 0.1f },
                  { 0.7f, 0.1f, 0.0f, 0.7f, 0.1f },
                  { 0.3f, 0.6f, 0.2f, 0.2f, 0.1f },
                  { 0.1f, 0.2f, 0.8f, 0.2f, 0.2f },
                  { 0.1f, 0.0f, 0.2f, 0.2f, 0.8f },
            };

            var R = new float[,]
            {
                  { 0.8f, 0.6f, 0.2f, 0.6f, 0.1f },
                  { 0.0f, 0.4f, 0.6f, 0.1f, 0.1f },
                  { 0.8f, 0.8f, 0.0f, 0.2f, 0.0f },
                  { 0.6f, 0.5f, 0.3f, 0.7f, 0.3f },
            };

            var exp = new float[,]
            {
                {0.18f, 0.19f, 0.19f, 0.19f },
                {0.13f, 0.34f, 0.2f, 0.14f  },
                {0.17f, 0.18f, 0.18f, 0.22f },
                {0.38f, 0.09f, 0.33f, 0.33f },
                {0.40f, 0.27f, 0.35f, 0.35f }
            };

            var result = Distances.HammingDistance(Norms.Lukasiewicz, Implications.Lukasiewicz, Q, R);

            for(int i=0;i<result.GetLength(0);i++)
            {
                for(int j=0;j<result.GetLength(1);j++)
                {
                    Assert.Equal(exp[i, j], result[i, j], _precision);
                }
            }

        }

        [Fact]
        public void EuclideanDistance()
        {
            var Q = new float[,]
            {
                  { 0.4f, 0.3f, 0.1f, 0.4f, 0.1f },
                  { 0.7f, 0.1f, 0.0f, 0.7f, 0.1f },
                  { 0.3f, 0.6f, 0.2f, 0.2f, 0.1f },
                  { 0.1f, 0.2f, 0.8f, 0.2f, 0.2f },
                  { 0.1f, 0.0f, 0.2f, 0.2f, 0.8f },
            };

            var R = new float[,]
            {
                  { 0.8f, 0.6f, 0.2f, 0.6f, 0.1f },
                  { 0.0f, 0.4f, 0.6f, 0.1f, 0.1f },
                  { 0.8f, 0.8f, 0.0f, 0.2f, 0.0f },
                  { 0.6f, 0.5f, 0.3f, 0.7f, 0.3f },
            };

            var exp = new float[,]
            {
                { 0.23f, 0.27f, 0.24f, 0.20f },
                { 0.19f, 0.43f, 0.28f, 0.18f },
                { 0.27f, 0.24f, 0.25f, 0.26f },
                { 0.43f, 0.12f, 0.42f, 0.37f },
                { 0.46f, 0.36f, 0.43f, 0.39f }
            };

            var result = Distances.EuclideanDistance(Norms.Lukasiewicz, Implications.Lukasiewicz, Q, R);

            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    Assert.Equal(exp[i, j], result[i, j], _precision);
                }
            }
        }
    }
}

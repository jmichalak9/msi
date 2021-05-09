using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FuzzySetLibTests
{
    public class DistancesTest
    {

        [Fact]
        public void HammingDistanceTest()
        {
            var Q = new float[,]{
                  { 0.4f, 0.3f, 0.1f, 0.4f, 0.1f },
                  { 0.7f, 0.1f, 0.0f, 0.7f, 0.1f },
                  { 0.3f, 0.6f, 0.2f, 0.2f, 0.1f },
                  { 0.1f, 0.2f, 0.8f, 0.2f, 0.2f },
                  { 0.1f, 0.0f, 0.2f, 0.2f, 0.8f },
            };

            var R = new float[,]{
                  { 0.8f, 0.6f, 0.2f, 0.6f, 0.1f },
                  { 0.0f, 0.4f, 0.6f, 0.1f, 0.1f },
                  { 0.8f, 0.8f, 0.0f, 0.2f, 0.0f },
                  { 0.6f, 0.5f, 0.3f, 0.7f, 0.3f },
            };

        }
    }
}

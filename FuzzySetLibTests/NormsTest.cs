using FuzzySetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FuzzySetLibTests
{
    public class NormsTest
    {
        [Fact]
        public void StandardTest()
        {
            AssertNaturalNorm(Norms.Standard);
        }

        [Fact]
        public void ProductTest()
        {
            AssertNaturalNorm(Norms.Product);
        }

        [Fact]
        public void LukasiewiczTest()
        {
            AssertNaturalNorm(Norms.Lukasiewicz);
        }

        private static void AssertNaturalNorm(Func<float, float, float> norm)
        {
            float x = 0.2f;
            float y = 0.5f;
            float z = 0.7f;
            int precision = 4;

            //T-norm conditions
            Assert.Equal(norm(x, y), norm(y, x));
            Assert.Equal(norm(x, norm(y, z)), norm(norm(x, y), z));
            Assert.True(norm(x, y) <= norm(z, y));
            Assert.Equal(x, norm(x, 1), precision);

            Assert.Throws<ArgumentException>(() => norm(-1, 0));
            Assert.Throws<ArgumentException>(() => norm(2, 0));
            Assert.Throws<ArgumentException>(() => norm(0, -1));
            Assert.Throws<ArgumentException>(() => norm(0, 2));
        }
    }
}

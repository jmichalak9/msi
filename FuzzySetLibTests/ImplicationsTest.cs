﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FuzzySetLib;

namespace FuzzySetLibTests
{
    public class ImplicationsTest
    {
        [Fact]
        public void GodelTest()
        {
            AssertNaturalImplication(Implications.Godel);
        }

        [Fact]
        public void KleeneDienesTest()
        {
            AssertNaturalImplication(Implications.KleeneDienes);
        }

        [Fact]
        public void LukasiewiczTest()
        {
            AssertNaturalImplication(Implications.Lukasiewicz);
        }

        private static void AssertNaturalImplication(Func<float, float, float> implication)
        {
            Assert.Equal(1, implication(0, 0));
            Assert.Equal(1, implication(0, 1));
            Assert.Equal(0, implication(1, 0));
            Assert.Equal(1, implication(1, 1));

            Assert.Throws<ArgumentException>(() => implication(-1, 0));
            Assert.Throws<ArgumentException>(() => implication(2, 0));
            Assert.Throws<ArgumentException>(() => implication(0, -1));
            Assert.Throws<ArgumentException>(() => implication(0, 2));

        }
    }
}

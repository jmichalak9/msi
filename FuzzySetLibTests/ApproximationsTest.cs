using FuzzySetLib;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Xunit;
using Xunit.Abstractions;

namespace FuzzySetLibTests
{
    public class ApproximationsTest
    {    
        private readonly ITestOutputHelper _testOutputHelper;
        public ApproximationsTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }
        [Fact]
        public void ObjectBoundApproximationTest()
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

            var exp = new Bounds[,]
            {
                { b(0.6,0.8), b(0.6,0.6), b(0.2,0.2), b(0.6,0.8), b(0.1,0.2), },
                { b(0.0,0.3), b(0.3,0.4), b(0.6,0.6), b(0.0,0.3), b(0.1,0.2), },
                { b(0.2,0.8), b(0.4,0.8), b(0.0,0.2), b(0.2,0.8), b(0.0,0.2), },
                { b(0.6,0.7), b(0.5,0.5), b(0.3,0.3), b(0.6,0.7), b(0.3,0.3), },
            };

            var act = Approximations.ObjectBoundApproximation(Norms.Lukasiewicz, Implications.Lukasiewicz, Q, R);

            AssertApproximationsEqual(exp, act);
        }
        
        [Fact]
        public void PropertyBoundApproximationTest()
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

            var exp = new Bounds[,]
            {
                { b(0.4,0.4), b(0.3,0.4), b(0.1,0.2), b(0.4,0.4), b(0.1,0.2), },
                { b(0.7,0.7), b(0.1,0.4), b(0.0,0.2), b(0.7,0.7), b(0.1,0.2), },
                { b(0.3,0.3), b(0.6,0.6), b(0.2,0.2), b(0.2,0.3), b(0.1,0.2), },
                { b(0.1,0.3), b(0.2,0.4), b(0.8,0.8), b(0.2,0.3), b(0.2,0.2), },
                { b(0.1,0.3), b(0.0,0.4), b(0.2,0.2), b(0.2,0.3), b(0.8,0.8), },
            };

            var act = Approximations.PropertyBoundApproximation(Norms.Lukasiewicz, Implications.Lukasiewicz, Q, R);

            AssertApproximationsEqual(exp, act);
        }

        private Bounds b(double lower, double upper)
        {
            return new Bounds
            {
                lower = (float)lower,
                upper = (float)upper,
            };
        }
        private void AssertApproximationsEqual(Bounds[,] exp, Bounds[,] act)
        {
            Assert.Equal(exp.GetLength(0), act.GetLength(0));
            Assert.Equal(exp.GetLength(1), act.GetLength(1));
            for (int i = 0; i < exp.GetLength(0); i++)
                for (int j = 0; j < exp.GetLength(1); j++)
                {
                    Assert.Equal(exp[i, j], act[i, j], new ApproximationComparer());
                }
        }
        private class ApproximationComparer : IEqualityComparer<Bounds>
        {
            public bool Equals(Bounds x, Bounds y)
            {
                var precision = 0.05;
                return Math.Abs(x.lower - y.lower) < precision && Math.Abs(x.upper - y.upper) < precision;
            }

            public int GetHashCode([DisallowNull] Bounds obj)
            {
                throw new NotImplementedException();
            }
        }

    }
}

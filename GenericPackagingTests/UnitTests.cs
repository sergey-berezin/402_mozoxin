using System.Xml.Serialization;
using FluentAssertions;

using GeneticPackaging;

namespace GenericPackagingTests
{
    public class UnitTests
    {
        [Fact]
        public void InterceptionTest1()
        {
            Candidate solution = new Candidate();
            solution.gens = new int[][] { new int[] { 0, 0 },
                                          new int[] { 2, 2 }, 
                                          new int[] { 6, 6 }, 
                                          new int[] { 8, 8 }, 
                                          new int[] { 9, 9 } };

            solution.TestIntersection().Should().BeTrue();
        }

        [Fact]
        public void InterceptionTest2()
        {
            Candidate solution = new Candidate();
            solution.gens = new int[][] { new int[] { 0, 0 },
                                          new int[] { 3, 3 },
                                          new int[] { 6, 6 },
                                          new int[] { 8, 8 },
                                          new int[] { 9, 9 } };

            solution.TestIntersection().Should().BeFalse();
        }

        [Fact]
        public void InterceptionTest3()
        {
            Candidate solution = new Candidate();
            solution.gens = new int[][] { new int[] { 0, 0 },
                                          new int[] { 3, 2 },
                                          new int[] { 6, 6 },
                                          new int[] { 8, 8 },
                                          new int[] { 9, 9 } };

            solution.TestIntersection().Should().BeFalse();
        }

        [Fact]
        public void InterceptionTest4()
        {
            Candidate solution = new Candidate();
            solution.gens = new int[][] { new int[] { 0, 0 },
                                          new int[] { 3, 3 },
                                          new int[] { 0, 0 },
                                          new int[] { 8, 8 },
                                          new int[] { 9, 9 } };

            solution.TestIntersection().Should().BeTrue();
        }

        [Fact]
        public void InterceptionTest5()
        {
            Candidate solution = new Candidate();
            solution.gens = new int[][] { new int[] { 3, 3 },
                                          new int[] { 3, 2 },
                                          new int[] { 6, 6 },
                                          new int[] { 8, 8 },
                                          new int[] { 9, 9 } };

            solution.TestIntersection().Should().BeTrue();
        }

        [Fact]
        public void InterceptionTest6()
        {
            Candidate solution = new Candidate();
            solution.gens = new int[][] { new int[] { 3, 3 },
                                          new int[] { 3, 1 },
                                          new int[] { 6, 6 },
                                          new int[] { 8, 8 },
                                          new int[] { 9, 9 } };

            solution.TestIntersection().Should().BeFalse();
        }

    }
}

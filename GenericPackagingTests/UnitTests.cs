using System.Xml.Serialization;
using FluentAssertions;

using GeneticPackaging;

namespace GenericPackagingTests
{
    public class UnitTests
    {
        static int FIGURES_AMOUNT = 5;
        static int[] FIGURES_SIZES = { 3, 2, 2, 1, 1 };

        [Fact]
        public void InterceptionTest1()
        {
            GeneticComputation computation = new GeneticComputation(100, 1, 500, FIGURES_AMOUNT, FIGURES_SIZES, new CancellationTokenSource().Token);
            Candidate solution = new Candidate(computation);
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
            GeneticComputation computation = new GeneticComputation(100, 1, 500, FIGURES_AMOUNT, FIGURES_SIZES, new CancellationTokenSource().Token);
            Candidate solution = new Candidate(computation);
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
            GeneticComputation computation = new GeneticComputation(100, 1, 500, FIGURES_AMOUNT, FIGURES_SIZES, new CancellationTokenSource().Token);
            Candidate solution = new Candidate(computation);
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
            GeneticComputation computation = new GeneticComputation(100, 1, 500, FIGURES_AMOUNT, FIGURES_SIZES, new CancellationTokenSource().Token);
            Candidate solution = new Candidate(computation);
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
            GeneticComputation computation = new GeneticComputation(100, 1, 500, FIGURES_AMOUNT, FIGURES_SIZES, new CancellationTokenSource().Token);
            Candidate solution = new Candidate(computation);
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
            GeneticComputation computation = new GeneticComputation(100, 1, 500, FIGURES_AMOUNT, FIGURES_SIZES, new CancellationTokenSource().Token);
            Candidate solution = new Candidate(computation);
            solution.gens = new int[][] { new int[] { 3, 3 },
                                          new int[] { 3, 1 },
                                          new int[] { 6, 6 },
                                          new int[] { 8, 8 },
                                          new int[] { 9, 9 } };

            solution.TestIntersection().Should().BeFalse();
        }

        [Fact]
        public void InterceptionTest7()
        {
            GeneticComputation computation = new GeneticComputation(100, 1, 500, FIGURES_AMOUNT, FIGURES_SIZES, new CancellationTokenSource().Token);
            Candidate solution = new Candidate(computation);
            solution.gens = new int[][] { new int[] { 13, 13 },
                                          new int[] { 12, 14 },
                                          new int[] { 15, 12 },
                                          new int[] { 16, 15 },
                                          new int[] { 16, 12 } };

            solution.TestIntersection().Should().BeTrue();
        }

        [Fact]
        public void InterceptionTest8()
        {
            GeneticComputation computation = new GeneticComputation(100, 1, 500, FIGURES_AMOUNT, FIGURES_SIZES, new CancellationTokenSource().Token);
            Candidate solution = new Candidate(computation);
            solution.gens = new int[][] { new int[] { 45, 60 },
                                          new int[] { 46, 59 },
                                          new int[] { 42, 61 },
                                          new int[] { 42, 59 },
                                          new int[] { 44, 59 } };

            solution.TestIntersection().Should().BeTrue();
        }

        [Fact]
        public void InterceptionTest9()
        {
            GeneticComputation computation = new GeneticComputation(100, 1, 500, FIGURES_AMOUNT, FIGURES_SIZES, new CancellationTokenSource().Token);
            Candidate solution = new Candidate(computation);
            solution.gens = new int[][] { new int[] { 45, 40 },
                                          new int[] { 43, 41 },
                                          new int[] { 43, 43 },
                                          new int[] { 43, 40 },
                                          new int[] { 43, 40 } };

            solution.TestIntersection().Should().BeTrue();
        }

    }
}

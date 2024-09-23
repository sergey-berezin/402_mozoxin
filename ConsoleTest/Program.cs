using GeneticPackaging;

namespace ConsoleTest
{
    class MainClass
    {
        static void Main(string[] args)
        {
            GeneticComputation computation = new GeneticComputation(1000, 5, 500);

            computation.CalculateMetric();
            Console.WriteLine(computation.bestSolution.metric);
            computation.NextIteration();

            while (computation.bestSolution.metric > 20)
            {
                computation.ReproducePopulation();
                computation.Mutate();
                computation.CalculateMetric();
                Console.WriteLine(computation.bestSolution.metric);
                computation.NextIteration();
            }

        }
    }
}
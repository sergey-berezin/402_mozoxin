using GeneticPackaging;
using System.Xml.XPath;

namespace ConsoleTest
{
    class MainClass
    {
        static Candidate best;
        static int bestMetric = -1;
        static bool working = true;
        static bool stop = false;

        static int FIGURES_AMOUNT = 5;
        static int[] FIGURES_SIZES = { 3, 2, 2, 1, 1 };

        static void Main(string[] args)
        {
            
            GeneticComputation computation = new GeneticComputation(100, 1, 500, FIGURES_AMOUNT, FIGURES_SIZES);

            computation.CalculateMetric();
            Console.WriteLine(computation.bestSolution.metric);
            computation.NextIteration();

            int i = 0;

            Console.CancelKeyPress += new ConsoleCancelEventHandler(interrupted);

            while (working)
            {
                computation.ReproducePopulation();
                computation.Mutate();
                computation.CalculateMetric();
                Console.WriteLine("Итерация " + i + ". Метрика наилучшего экземпляра: " + computation.bestSolution.metric);
                if (bestMetric == -1 || bestMetric > computation.bestSolution.metric)
                {
                    best = computation.bestSolution;
                    bestMetric = computation.bestSolution.metric;
                }
                
                computation.NextIteration();

                i++;
            }
            while (!stop) { }
        }

        protected static void interrupted(object sender, ConsoleCancelEventArgs args)
        {
            args.Cancel = true;
            working = false;

            Console.WriteLine();
            Console.WriteLine("Метрика лучшего решения: " + bestMetric);
            
            for (int i = 0; i < best.gens.Length; i++)
            {
                Console.WriteLine("(" + best.gens[i][0] + "," + best.gens[i][1] 
                                 + ") - квадрат со стороной " + FIGURES_SIZES[i]);
            }
            stop = true;
        }
    }
}
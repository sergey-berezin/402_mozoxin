using GeneticPackaging;
using System.Xml.XPath;

namespace ConsoleTest
{
    class MainClass
    {
        static Candidate best = new Candidate();
        static int bestMetric = -1;
        static bool working = true;
        static bool stop = false;

        static void Main(string[] args)
        {
            GeneticComputation computation = new GeneticComputation(100, 2, 500);

            computation.CalculateMetric();
            Console.WriteLine(computation.bestSolution.metric);
            computation.NextIteration();

            ConsoleKeyInfo cki;

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
                                 + ") - квадрат со стороной " + Candidate.FIGURES_SIZES[i]);
            }
            stop = true;
        }
    }
}
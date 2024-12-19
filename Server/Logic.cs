using GeneticPackaging;
using Newtonsoft.Json;

namespace Server
{
    public class Logic
    {
        
        public static async Task<GeneticComputation> StartCalculation(int childrenSize, int mutationSize, int populaionSize, int figuresAmount, int[] figuresSizes)
        {
            CancellationTokenSource ctf = new CancellationTokenSource();

            GeneticComputation computation = new GeneticComputation(childrenSize, mutationSize, populaionSize, figuresAmount, figuresSizes, ctf.Token);
            await computation.CalculateMetric();
            computation.NextIteration();
            return computation;
        }

        public static async Task<GeneticComputation> Iterate(int iterationsCompleted, int childrenSize, int mutationSize, int populationSize, int figuresAmount, int[] figuresSizes, List<Creature> populationFromClient)
        {

            var population = new List<Candidate>();

            var ctf = new CancellationTokenSource();
            var computation = new GeneticComputation(childrenSize, mutationSize, populationSize, figuresAmount, figuresSizes, ctf.Token);

            foreach (var elem in populationFromClient)
            {
                population.Add(new Candidate(computation, elem.gens));
            }
            computation.population = population;
            computation.iterationsCompleted = iterationsCompleted;

            await computation.ReproducePopulation();
            await computation.Mutate();
            await computation.CalculateMetric();
            computation.NextIteration();

            return computation;
            
        }
    }
}

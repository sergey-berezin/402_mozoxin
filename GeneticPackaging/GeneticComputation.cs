using System;
using System.Collections.Generic;

namespace GeneticPackaging
{
    public class GeneticComputation
    {
        public int iterationsCompleted { get; set; }
        public int populationSize { get; set; }
        public int mutationSize { get; set; }
        public int bestSize { get; set; }
        public List<Candidate> population { get; set; }
        public Candidate bestSolution { get; set; }

        private Random random;

        public int FIGURES_AMOUNT;
        public int[] FIGURES_SIZES;
        private CancellationToken token;

        public GeneticComputation(int populationSize, int mutationSize, int bestSize, int figures_amount, int[] figures_sizes, CancellationToken token)
        {
            iterationsCompleted = 0;
            this.populationSize = populationSize;
            this.mutationSize = mutationSize;
            this.bestSize = bestSize;

            FIGURES_AMOUNT = figures_amount;
            FIGURES_SIZES = figures_sizes;

            population = new List<Candidate>();

            for (int i = 0; i < this.bestSize; i++)
            {
                Candidate candidate = new Candidate(this);
                while (candidate.TestIntersection())
                {
                    candidate = new Candidate(this);
                }
                population.Add(candidate);
            }

            random = new Random();
            this.bestSize = bestSize;
            this.token = token;
        }

        public async Task ReproducePopulation()
        {
            if (iterationsCompleted == 0)
            {
                return;
            }

            var tasks = new Task<Candidate>[populationSize];


            for (int i = 0; i < populationSize; i++)
            {
                tasks[i] = Task.Factory.StartNew(() =>
                {
                    token.ThrowIfCancellationRequested();
                    Candidate candidate = new Candidate(this);
                    bool created = false;

                    while (!created || candidate.TestIntersection())
                    {
                        int first = random.Next(0, population.Count);
                        int second = first;
                        while (second == first)
                        {
                            second = random.Next(0, population.Count);
                        }

                        candidate = new Candidate(this, population[first], population[second]);
                        created = true;
                    }
                    return candidate;
                });
            }

            var results = await Task.WhenAll(tasks);

            for (int i = 0; i < populationSize; i++)
            {
                population.Add(results[i]);
            }
        }

        public async Task Mutate()
        {
            int[] indexes = new int[mutationSize];
            for (int i = 0; i < mutationSize; i++) 
            {
                int index = random.Next(0, population.Count);
                while (indexes.Contains(index))
                {
                    index = random.Next(0, population.Count);
                }
                indexes[i] = index;
            }

            var tasks = new Task<Candidate>[indexes.Length];
            int taskIndex = 0;

            foreach (int i in indexes)
            {
                tasks[taskIndex++] = Task.Factory.StartNew(item =>
                {
                    token.ThrowIfCancellationRequested();
                    int add;
                    if (random.Next(0, 2) == 0)
                        add = 1;
                    else
                        add = -1;
                    int[][] gens = new int[((Candidate) item).gens.Length][];
                    for (int j = 0; j < gens.Length; j++)
                    {
                        gens[j] = (int[])((Candidate) item).gens[j].Clone();
                    }
                    gens[random.Next(0, FIGURES_AMOUNT)][random.Next(0, 2)] += add;

                    return new Candidate(this, gens);
                }, population[i], token);
            }

            var results = await Task.WhenAll(tasks);

            for (int i = 0; i < taskIndex; i++)
            {
                population.Add(results[i]);
            }

            population.RemoveAll(cand => cand.TestIntersection() == true);
        }

        public async Task CalculateMetric()
        {
            int bestMetric = int.MaxValue;

            var tasks = new Task[population.Count];

            for (int i = 0; i < population.Count; i++)
            {
                tasks[i] = Task.Factory.StartNew(index =>
                {
                    token.ThrowIfCancellationRequested();
                    population[(int) index].CalculateMetric();
                }, i, token);
            }
            await Task.WhenAll(tasks);
            for (int i = 0; i < population.Count; i++)
            {
                if (population[i].metric < bestMetric)
                {
                    bestMetric = population[i].metric;
                    bestSolution = population[i];
                }
            }
            
        }

        public void NextIteration()
        {
            List<Candidate> newPopulation = new List<Candidate>();

            newPopulation = population.OrderBy(o => o.metric).ToList();

            population = newPopulation.GetRange(0, bestSize);
            iterationsCompleted++;
        }
        
    }
}

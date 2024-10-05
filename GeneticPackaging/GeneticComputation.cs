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
        
        public GeneticComputation(int populationSize, int mutationSize, int bestSize, int figures_amount, int[] figures_sizes)
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
        }

        public void ReproducePopulation()
        {
            if (iterationsCompleted == 0)
            {
                return;
            }

            for (int i = 0; i < populationSize; i++)
            {
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
                population.Add(candidate);
            }
        }

        public void Mutate()
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

            foreach (int i in indexes)
            {
                int add;
                if (random.Next(0, 2) == 0)
                    add = 1;
                else
                    add = -1;
                int[][] gens = new int[population[i].gens.Length][];
                for (int j = 0; j < gens.Length; j++)
                {
                    gens[j] = (int[]) population[i].gens[j].Clone();
                }
                gens[random.Next(0, 5)][random.Next(0, 2)] += add;
                population.Add(new Candidate(this, gens));
            }

            population.RemoveAll(cand => cand.TestIntersection() == true);
        }

        public void CalculateMetric()
        {
            int bestMetric = int.MaxValue;

            for (int i = 0; i < population.Count; i++)
            {
                population[i].CalculateMetric();
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

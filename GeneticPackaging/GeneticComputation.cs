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

        public GeneticComputation(int populationSize, int mutationSize, int bestSize)
        {
            iterationsCompleted = 0;
            this.populationSize = populationSize;
            this.mutationSize = mutationSize;
            this.bestSize = bestSize;
            population = new List<Candidate>();

            for (int i = 0; i < this.populationSize; i++)
            {
                Candidate candidate = new Candidate();
                while (candidate.TestIntersection())
                {
                    candidate = new Candidate();
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

            List<Candidate> newPopulation = new List<Candidate>();

            for (int i = 0; i < populationSize; i++)
            {
                Candidate candidate = new Candidate();
                bool created = false;

                while (!created || candidate.TestIntersection())
                {
                    int first = random.Next(0, population.Count);
                    int second = first;
                    while (second == first)
                    {
                        second = random.Next(0, population.Count);
                    }

                    candidate = new Candidate(population[first], population[second]);
                    created = true;
                }
                newPopulation.Add(candidate);
            }

            population = newPopulation;
        }

        public void Mutate()
        {
            int[] indexes = new int[mutationSize];
            for (int i = 0; i < mutationSize; i++) 
            {
                int index = random.Next(0, populationSize);
                while (indexes.Contains(index))
                {
                    index = random.Next(0, populationSize);
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
                population[i].gens[random.Next(0, 5)][random.Next(0, 2)] += add;
            }
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
            double metricSum = 0;
            List<Candidate> newPopulation = new List<Candidate>();

            for (int i = 0; i < population.Count; i++)
            {
                if (population[i].TestIntersection())
                {
                    population.RemoveAt(i);
                    i--;
                    continue;
                }
                metricSum += (double) 1 / population[i].metric;
            }

            List<double> probabilities = new List<double>();

            foreach (Candidate candidate in population)
            {
                probabilities.Add((double) 1 / candidate.metric / metricSum);
            }

            double interval = 1;

            for (int i = 0; i < bestSize; i++)
            {
                double p = random.NextDouble() * interval;
                double s = 0;

                for (int j = 0; j < probabilities.Count; j++)
                {
                    s += probabilities[j];
                    if (s > p)
                    {
                        newPopulation.Add(population[j]);
                        interval = (metricSum - 1 / population[j].metric) / metricSum;
                        metricSum -= 1 / population[j].metric;
                        probabilities.RemoveAt(j);
                        break;
                    }
                }
            }

            population = newPopulation;
            iterationsCompleted++;
        }
        
    }
}

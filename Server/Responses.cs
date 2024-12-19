using GeneticPackaging;

namespace Server
{
    public class PostBody
    {
        public int childrenSize { get; set; }
        public int mutationSize { get; set; }
        public int populationSize { get; set; }
        public int figuresAmount { get; set; }
        public int[] figuresSizes { get; set; }
        public List<Creature> population { get; set; }
        public int iterationsCompleted { get; set; }
    }

    public class Creature
    {
        public int[][] gens { get; set; }
    }
}
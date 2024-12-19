namespace GeneticPackaging
{
    public class Candidate
    {
        public int[][] gens { get; set; }
        public int metric { get; set; }
        private GeneticComputation context;
        public int[][] bounds { get; set; }
        private int FIELD_MIN = 0;
        private int FIELD_MAX;

        public Candidate(GeneticComputation context)
        {
            gens = new int[context.FIGURES_AMOUNT][];

            FIELD_MAX = context.FIGURES_SIZES.Sum() * 10;

            Random random = new Random();
            for (int i = 0; i < context.FIGURES_AMOUNT; i++)
            {
                gens[i] = new int[] { random.Next(FIELD_MIN, FIELD_MAX), random.Next(FIELD_MIN, FIELD_MAX) };
            }

            this.context = context;
        }

        public Candidate(GeneticComputation context, int[][] gens)
        {
            FIELD_MAX = context.FIGURES_SIZES.Sum() * 10;
            this.gens = (int[][]) gens.Clone();
            this.context = context;
        }

        public Candidate(GeneticComputation context, Candidate mother, Candidate father)
        {
            Random random = new Random();
            gens = new int[context.FIGURES_AMOUNT][];

            FIELD_MAX = context.FIGURES_SIZES.Sum() * 10;

            for (int i = 0; i < context.FIGURES_AMOUNT; i++)
            {
                if (random.Next(0, 2) % 2 == 0)
                {
                    gens[i] = mother.gens[i];
                } 
                else
                {
                    gens[i] = father.gens[i];
                }
            }
            this.context = context;
        }

        public int[][] CalculateBounds()
        {
            int[] max = { 0, 0 };
            int[] min = { FIELD_MAX + context.FIGURES_SIZES.Max(), FIELD_MAX + context.FIGURES_SIZES.Max() };

            for (int i = 0; i < gens.Length; i++)
            {
                for (int j = 0; j < min.Length; j++)
                {
                    if (min[j] > gens[i][j])
                        min[j] = gens[i][j];

                    if (max[j] < gens[i][j] + context.FIGURES_SIZES[i])
                        max[j] = gens[i][j] + context.FIGURES_SIZES[i];
                }
            }

            int[][] res = new int[2][];
            res[0] = min;
            res[1] = max;
            return res;
        }
        
        public void CalculateMetric()
        {
            var res = CalculateBounds();
            bounds = res;
            metric = (res[1][0] - res[0][0]) * (res[1][1] - res[0][1]);
        }

        public bool TestIntersection()
        {
            for (int i = 0; i < context.FIGURES_AMOUNT; i++)
            {
                
                for (int j = 0; j < context.FIGURES_AMOUNT; j++)
                {
                        
                    if (i != j &&
                        Math.Max(gens[i][0], gens[j][0]) - Math.Min(gens[i][0] + context.FIGURES_SIZES[i], gens[j][0] + context.FIGURES_SIZES[j]) < 0 &&
                        Math.Max(gens[i][1], gens[j][1]) - Math.Min(gens[i][1] + context.FIGURES_SIZES[i], gens[j][1] + context.FIGURES_SIZES[j]) < 0)
                    {
                        return true;
                    }
                        
                }
                
            }
            return false;
        }
    }
}

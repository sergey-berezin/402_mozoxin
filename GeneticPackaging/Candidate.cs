namespace GeneticPackaging
{
    public class Candidate
    {
        public int[][] gens { get; set; }
        public int metric { get; set; }

        public static int FIGURES_AMOUNT = 5;
        public static int[] FIGURES_SIZES = { 3, 2, 2, 1, 1 };
        public static int FIELD_MIN = 0;
        public static int FIELD_MAX = 100;

        public Candidate()
        {
            gens = new int[5][];

            Random random = new Random();
            for (int i = 0; i < FIGURES_AMOUNT; i++)
            {
                gens[i] = new int[] { random.Next(FIELD_MIN, FIELD_MAX), random.Next(FIELD_MIN, FIELD_MAX)};
            }
        }

        public Candidate(int[][] gens)
        {
            this.gens = (int[][]) gens.Clone();
        }

        public Candidate(Candidate mother, Candidate father)
        {
            Random random = new Random();
            gens = new int[FIGURES_AMOUNT][];

            for (int i = 0; i < FIGURES_AMOUNT; i++)
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

            
        }

        public void CalculateMetric()
        {
            int[] max = { 0, 0 };
            int[] min = { FIELD_MAX + FIGURES_SIZES.Max(), FIELD_MAX + FIGURES_SIZES.Max() };

            for (int i = 0; i < gens.Length; i++)
            {
                for (int j = 0; j < min.Length; j++)
                {
                    if (min[j] > gens[i][j]) 
                        min[j] = gens[i][j];

                    if (max[j] < gens[i][j] + FIGURES_SIZES[i]) 
                        max[j] = gens[i][j] + FIGURES_SIZES[i];
                }
            }

            metric = (max[0] - min[0]) * (max[1] - min[1]);
        }

        public bool TestIntersection()
        {
            for (int i = 0; i < FIGURES_AMOUNT; i++)
            {
                if (FIGURES_SIZES[i] > 1)
                {
                    for (int j = 0; j < FIGURES_AMOUNT; j++)
                    {
                        
                        if (i != j && 
                            gens[i][0] <= gens[j][0] && gens[i][0] + FIGURES_SIZES[i] > gens[j][0] &&
                            gens[i][1] <= gens[j][1] && gens[i][1] + FIGURES_SIZES[i] > gens[j][1])
                        {
                            return true;
                        }
                        
                    }
                }
            }
            return false;
        }
    }
}

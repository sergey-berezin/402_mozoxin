using Microsoft.EntityFrameworkCore;

namespace GeneticApp
{
    public class Experiment
    {
        public int id { get; set; }
        public string name { get; set; }
        public int iterations { get; set; }
        public int childrenSize { get; set; }
        public int mutationSize { get; set; }
        public int populationSize { get; set; }
        public int figuresAmount { get; set; }
        public string figuresSizes { get; set; }
        virtual public ICollection<Figure> population { get; set; }
    }
    public class Figure
    {
        public int id { get; set; }
        public string gens { get; set; }

    }

    class LibraryContext : DbContext
    {
        public LibraryContext() 
        {
            Database.EnsureCreated();
        }

        public DbSet<Experiment> Experiments { get; set; }
        public DbSet<Figure> Figures { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder o)
            => o.UseLazyLoadingProxies().UseSqlite("Data Source=experiments.db");
    }
}

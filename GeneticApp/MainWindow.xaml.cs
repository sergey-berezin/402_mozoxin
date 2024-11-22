using System.Collections.ObjectModel;
using System.Drawing;
using System.Security.Policy;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

using GeneticPackaging;

namespace GeneticApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public int populationSize { get; set; }
        public int mutationSize { get; set; }
        public int childrenSize { get; set; }
        public string figuresSizesText { get; set; }
        public int[] figuresSizes { get; set; }
        public int bestMetric { get; set; }
        public string experimentName { get; set; }
        public ObservableCollection<string> experiments_names { get; set; }

        private bool working;
        private GeneticComputation computation;
        private int iterations;
        private List<Candidate> population;
        private bool loaded = false;
        private CancellationTokenSource ctf;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            childrenSize = 100;
            mutationSize = 1;
            populationSize = 500;
            figuresSizesText = "3, 2, 2, 1, 1";
            Stop_Button.IsEnabled = false;
            Save_Button.IsEnabled = false;
            experimentName = "";
            experiments_names= new ObservableCollection<string>();
        }


        private async void StartCalculation(object sender, RoutedEventArgs e)
        {
            Runs_Listbox.Visibility = Visibility.Collapsed;
            Canvas_Grid.Visibility = Visibility.Visible;
            Start_Button.IsEnabled = false;
            Save_Button.IsEnabled = false;
            Load_Button.IsEnabled = false;
            working = true;
            bestMetric = -1;

            Stop_Button.IsEnabled = true;

            string[] sizes = figuresSizesText.Split(", ");
            figuresSizes = new int[sizes.Length];
            int index = 0;
            foreach (string s in sizes)
            {
                figuresSizes[index++] = int.Parse(s);
            }

            ctf = new CancellationTokenSource();

            Task<GeneticComputation> result_task = await Task.Factory.StartNew(async () =>
            {
                ctf.Token.ThrowIfCancellationRequested();
                if (!loaded)
                    computation = new GeneticComputation(childrenSize, mutationSize, populationSize, sizes.Length, figuresSizes, ctf.Token);

                await computation.CalculateMetric();
                computation.NextIteration();

                int i = 0;

                while (working)
                {
                    await computation.ReproducePopulation();
                    await computation.Mutate();
                    await computation.CalculateMetric();

                    if (bestMetric == -1 || bestMetric > computation.bestSolution.metric)
                    {
                        bestMetric = computation.bestSolution.metric;

                        await Dispatcher.BeginInvoke(new Action(() =>
                        {
                            Solution_Canvas.Children.Clear();
                            Metric_TextBlock.Text = "Метрика лучшего результата: " + bestMetric;

                            var bestSolution = computation.bestSolution;

                            var bounds = bestSolution.CalculateBounds();

                            double sizeX = Math.Abs(bounds[0][0] - bounds[1][0]);
                            double sizeY = Math.Abs(bounds[0][1] - bounds[1][1]);
                            if (sizeX > sizeY)
                            {
                                sizeY = sizeX;
                            }
                            else
                            {
                                sizeX = sizeY;
                            }
                            var height = Canvas_Grid.ActualHeight;
                            var width = Canvas_Grid.ActualWidth;

                            for (int i = 0; i < computation.FIGURES_AMOUNT; i++)
                            {
                                var rectangle = new System.Windows.Shapes.Rectangle
                                {
                                    Height = computation.FIGURES_SIZES[i] / sizeY * height,
                                    Width = computation.FIGURES_SIZES[i] / sizeX * width
                                };
                                Canvas.SetLeft(rectangle, (bestSolution.gens[i][0] - bounds[0][0]) / sizeX * width);
                                Canvas.SetTop(rectangle, (bestSolution.gens[i][1] - bounds[0][1]) / sizeY * height);

                                rectangle.Fill = new SolidColorBrush(Colors.Black);
                                rectangle.Stroke = new SolidColorBrush(Colors.Red);
                                Solution_Canvas.Children.Add(rectangle);
                            }
                        }));
                    }

                    computation.NextIteration();

                    i++;
                }

                return computation;
            }, ctf.Token);

            loaded = false;

            var result = await result_task;
            Start_Button.IsEnabled = true;
            Load_Button.IsEnabled = true;
            Save_Button.IsEnabled = true;

        }

        private void StopCalculation(object sender, RoutedEventArgs e)
        {
            working = false;
            Stop_Button.IsEnabled = false;
            Save_Button.IsEnabled = false;
        }

        private void SaveCalculation(object sender, RoutedEventArgs e)
        {
            working = false;
            using (var db = new LibraryContext())
            {
                var experiment = new Experiment
                {
                    name = experimentName,
                    iterations = computation.iterationsCompleted,
                    childrenSize = computation.populationSize,
                    mutationSize = computation.mutationSize,
                    populationSize = computation.bestSize,
                    figuresAmount = computation.FIGURES_AMOUNT,
                    figuresSizes = string.Join(",", computation.FIGURES_SIZES)
                };
                var population = new List<Figure>();
                foreach (var cand in computation.population)
                {
                    population.Add(new Figure
                    {
                        gens = string.Join(";", computation.bestSolution.gens.Select(inner => string.Join(",", inner)))
                    });
                }

                experiment.population = population;
                db.Add(experiment);
                db.SaveChanges();
            }
            MessageBox.Show("Эксперимент сохранен успешно!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LoadCalculation(object sender, RoutedEventArgs e)
        {
            Canvas_Grid.Visibility = Visibility.Collapsed;
            Runs_Listbox.Visibility = Visibility.Visible;

            experiments_names.Clear();
            using (var db = new LibraryContext())
            {
                foreach (var exp in db.Experiments)
                {
                    experiments_names.Add(exp.name + " (id = " + exp.id + ")");
                   
                }
            }
            

        }

        private void SelectedIndex(object sender, System.EventArgs e)
        {
            if (Runs_Listbox.SelectedIndex != -1)
            {
                Runs_Listbox.Visibility = Visibility.Collapsed;
                Canvas_Grid.Visibility = Visibility.Visible;
                using (var db = new LibraryContext())
                {
                    var exp = db.Experiments.Where(e => e.id == Runs_Listbox.SelectedIndex + 1).First();

                    iterations = exp.iterations;
                    childrenSize = exp.childrenSize;
                    mutationSize = exp.mutationSize;
                    populationSize = exp.populationSize;

                    string[] sizes = exp.figuresSizes.Split(",");
                    figuresSizes = new int[sizes.Length];
                    int index = 0;
                    foreach (string s in sizes)
                    {
                        figuresSizes[index++] = int.Parse(s);
                    }

                    population = new List<Candidate>();

                    ctf = new CancellationTokenSource();
                    computation = new GeneticComputation(childrenSize, mutationSize, populationSize, sizes.Length, figuresSizes, ctf.Token);

                    foreach (var elem in exp.population)
                    {
                        int[][] gens = new int[sizes.Length][];
                        int i = 0;
                        foreach (string pair in elem.gens.Split(";"))
                        {
                            var coord = pair.Split(",");
                            gens[i++] = [int.Parse(coord[0]), int.Parse(coord[1])];
                        }
                        population.Add(new Candidate(computation, gens));
                    }
                    computation.population = population;
                    computation.iterationsCompleted = iterations;
                    //Metric_TextBlock.Text = "Метрика лучшего результата: " + ;
                }

                loaded = true;
                MessageBox.Show("Эксперимент загружен успешно! Для запуска нажмите \"Запустить расчет\"", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
using System.Drawing;
using System.Security.Policy;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
        public int populationSize {  get; set; }
        public int mutationSize { get; set; }
        public int childrenSize { get; set; }

        public int figuresAmount { get; set; }
        public string figuresSizesText { get; set; }
        public int[] figuresSizes { get; set; }
        public int bestMetric { get; set; }

        private bool working;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            childrenSize = 100;
            mutationSize = 1;
            populationSize = 500;
            figuresSizesText = "3, 2, 2, 1, 1";
            Stop_Button.IsEnabled = false;
        }


        private async void StartCalculation(object sender, RoutedEventArgs e)
        {
            Start_Button.IsEnabled = false;
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

            CancellationTokenSource ctf = new CancellationTokenSource();

            Task<GeneticComputation> result_task = await Task.Factory.StartNew(async () =>
            {
                ctf.Token.ThrowIfCancellationRequested();
                GeneticComputation computation = new GeneticComputation(childrenSize, mutationSize, populationSize, sizes.Length, figuresSizes, ctf.Token);

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

            var result = await result_task;
            Start_Button.IsEnabled = true;

        }

        private void StopCalculation(object sender, RoutedEventArgs e)
        {
            working = false;
            Stop_Button.IsEnabled = false;
        }
    }
}
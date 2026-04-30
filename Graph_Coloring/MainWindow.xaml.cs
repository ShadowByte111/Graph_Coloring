//using Graph_Coloring.Graph_Coloring;
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
using System.Collections.Generic;


namespace Graph_Coloring
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        private Graph _graph;
        private Dictionary<Node, Ellipse> _nodeVisuals;

        // Змінна для малювання ребер (запам'ятовує перший клік по вершині)
        private Node _firstNodeSelected = null;

        private Brush[] _palette = new Brush[]
        {
            Brushes.White, Brushes.Tomato, Brushes.MediumSeaGreen, Brushes.DodgerBlue, Brushes.Gold, Brushes.BlueViolet, Brushes.Orange
        };

        public MainWindow()
        {
            InitializeComponent();
            _graph = new Graph();
            _nodeVisuals = new Dictionary<Node, Ellipse>();
        }

        // --- МАЛЮВАННЯ ГРАФА МИШКОЮ ---

        // 1. Клік по порожньому полотну створює вершину
        private void GraphCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point clickPosition = e.GetPosition(GraphCanvas);

            // Додаємо в логіку графа
            _graph.AddNode(clickPosition.X, clickPosition.Y);

            // Малюємо на екрані (беремо останню додану вершину)
            DrawNode(_graph.Nodes[_graph.Nodes.Count - 1]);
        }

        // --- КНОПКА: Запуск алгоритму ---
        private void BtnSolve_Click(object sender, RoutedEventArgs e)
        {
            if (_graph.Nodes.Count == 0) return;

            // Скидаємо кольори перед новим запуском
            _graph.ClearColors();
            foreach (var ellipse in _nodeVisuals.Values)
            {
                ellipse.Fill = _palette[0];
            }

            ISolver solver = new HillClimbingSolver();
            // Дамо алгоритму більше кольорів для складних графів
            int steps = solver.Solve(_graph, 5);

            foreach (var node in _graph.Nodes)
            {
                if (_nodeVisuals.ContainsKey(node))
                {
                    _nodeVisuals[node].Fill = _palette[node.Color];
                }
            }

            MessageBox.Show($"Алгоритм завершив роботу за {steps} ітерацій!\nЗалишилось конфліктів: {_graph.CalculateConflicts()}", "Результат");
        }

        // --- КНОПКА 3: Запуск Емуляції відпалу ---
        private void BtnSolveAnnealing_Click(object sender, RoutedEventArgs e)
        {
            if (_graph.Nodes.Count == 0) return;

            // Скидаємо кольори перед запуском
            _graph.ClearColors();
            foreach (var ellipse in _nodeVisuals.Values)
            {
                ellipse.Fill = _palette[0];
            }

            // Використовуємо новий алгоритм!
            ISolver solver = new SimulatedAnnealingSolver();
            int steps = solver.Solve(_graph, 3); // Дамо 3 кольори для цікавості

            // Оновлюємо картинку
            foreach (var node in _graph.Nodes)
            {
                if (_nodeVisuals.ContainsKey(node))
                {
                    _nodeVisuals[node].Fill = _palette[node.Color];
                }
            }

            MessageBox.Show($"Емуляція відпалу завершила роботу за {steps} ітерацій!\nЗалишилось конфліктів: {_graph.CalculateConflicts()}", "Результат Відпалу");
        }

        // --- КНОПКА 4: Запуск Променевого пошуку ---
        private void BtnSolveBeam_Click(object sender, RoutedEventArgs e)
        {
            if (_graph.Nodes.Count == 0) return;

            // Скидаємо кольори
            _graph.ClearColors();
            foreach (var ellipse in _nodeVisuals.Values)
            {
                ellipse.Fill = _palette[0];
            }

            ISolver solver = new BeamSearchSolver();
            int steps = solver.Solve(_graph, 3); // Нехай спробує розфарбувати в 3 кольори

            // Малюємо результат
            foreach (var node in _graph.Nodes)
            {
                if (_nodeVisuals.ContainsKey(node))
                {
                    _nodeVisuals[node].Fill = _palette[node.Color];
                }
            }

            MessageBox.Show($"Променевий пошук завершив роботу за {steps} ітерацій!\nЗалишилось конфліктів: {_graph.CalculateConflicts()}", "Результат Променевого пошуку");
        }

        // --- КНОПКА: Очистити все (переробили кнопку генерації) ---
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            GraphCanvas.Children.Clear();
            _graph = new Graph();
            _nodeVisuals.Clear();
            _firstNodeSelected = null;
        }


        // --- Допоміжні методи для малювання ---
        private void DrawEdge(Node a, Node b)
        {
            Line line = new Line
            {
                X1 = a.X,
                Y1 = a.Y,
                X2 = b.X,
                Y2 = b.Y,
                Stroke = Brushes.Gray,
                StrokeThickness = 3
            };

            // Додаємо лінію на задній план (щоб вона не перекривала кружечки)
            GraphCanvas.Children.Insert(0, line);
        }

        private void DrawNode(Node node)
        {
            Ellipse ellipse = new Ellipse
            {
                Width = 40,
                Height = 40,
                Fill = _palette[node.Color],
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };

            Canvas.SetLeft(ellipse, node.X - 20);
            Canvas.SetTop(ellipse, node.Y - 20);

            // 2. Логіка кліку по САМІЙ ВЕРШИНІ (для з'єднання ребрами)
            ellipse.MouseLeftButtonDown += (s, e) =>
            {
                if (_firstNodeSelected == null)
                {
                    // Це перший клік - виділяємо вершину червоним контуром
                    _firstNodeSelected = node;
                    ellipse.Stroke = Brushes.Red;
                    ellipse.StrokeThickness = 4;
                }
                else
                {
                    // Це другий клік - з'єднуємо їх, якщо це різні вершини
                    if (_firstNodeSelected != node)
                    {
                        _graph.AddEdge(_firstNodeSelected, node);
                        DrawEdge(_firstNodeSelected, node);
                    }

                    // Знімаємо виділення з першої вершини
                    _nodeVisuals[_firstNodeSelected].Stroke = Brushes.Black;
                    _nodeVisuals[_firstNodeSelected].StrokeThickness = 2;
                    _firstNodeSelected = null;
                }

                // Зупиняємо клік, щоб він не пішов далі на Canvas (щоб не створилася нова вершина під цією)
                e.Handled = true;
            };

            GraphCanvas.Children.Add(ellipse);
            _nodeVisuals.Add(node, ellipse);
        }
    }
}
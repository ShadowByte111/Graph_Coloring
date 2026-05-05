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
using Graph_Coloring.Models;
using Graph_Coloring.Algorithms;

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
        private Node? _firstNodeSelected = null;

        private Brush[] _palette = new Brush[]
        {
            Brushes.White, Brushes.Tomato, Brushes.MediumSeaGreen, Brushes.DodgerBlue,
            Brushes.Gold, Brushes.BlueViolet, Brushes.Orange, Brushes.Orchid,
            Brushes.Purple, Brushes.PaleTurquoise, Brushes.Olive, Brushes.Aqua,
            Brushes.BlanchedAlmond, Brushes.Coral, Brushes.DarkBlue, Brushes.ForestGreen,
            Brushes.Honeydew, Brushes.Yellow, Brushes.Orange, Brushes.OldLace, Brushes.SteelBlue
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
            if (_graph.Nodes.Count >= 20)
            {
                MessageBox.Show("Досягнуто ліміт: максимум 20 вершин!", "Увага");
                return;
            }

            Point clickPosition = e.GetPosition(GraphCanvas);

            foreach (var node in _graph.Nodes)
            {
                double dx = node.X - clickPosition.X;
                double dy = node.Y - clickPosition.Y;
                double distance = Math.Sqrt(dx * dx + dy * dy);

                // 40 - це діаметр нашого кружечка. Якщо відстань менша, значить вони перетнуться
                if (distance < 40)
                {
                    return; // Просто ігноруємо клік, нічого не малюємо
                }
            }

            // Додаємо в логіку графа
            _graph.AddNode(clickPosition.X, clickPosition.Y);
            // Малюємо на екрані (беремо останню додану вершину)
            DrawNode(_graph.Nodes[_graph.Nodes.Count - 1]);
        }

        // --- КНОПКА: Очистити все (переробили кнопку генерації) ---
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            GraphCanvas.Children.Clear();
            _graph = new Graph();
            _nodeVisuals.Clear();
            _firstNodeSelected = null;
        }

        // --- КНОПКА: Запуск алгоритму ---
        private void BtnSolveHill_Click(object sender, RoutedEventArgs e)
        {
            if (_graph.Nodes.Count == 0)
            {
                return;
            }

            if (!ValidateColorInput(out int colorCount))
            {
                return;
            }

            // Скидаємо кольори перед новим запуском
            _graph.ClearColors();
            foreach (var ellipse in _nodeVisuals.Values)
            {
                ellipse.Fill = _palette[0];
            }

            ISolver solver = new HillClimbingSolver();
            // Дамо алгоритму більше кольорів для складних графів
            int steps = solver.Solve(_graph, colorCount);

            foreach (var node in _graph.Nodes)
            {
                if (_nodeVisuals.ContainsKey(node))
                {
                    _nodeVisuals[node].Fill = _palette[node.Color];
                }
            }

            ResultTextBlock.Text =
                $"Метод: Hill Climbing\n" +
                $"Ітерацій: {steps}\n" +
                $"Конфліктів: {_graph.CalculateConflicts()}";
        }

        // --- КНОПКА 3: Запуск Емуляції відпалу ---
        private void BtnSolveAnnealing_Click(object sender, RoutedEventArgs e)
        {
            if (_graph.Nodes.Count == 0)
            {
                return;
            }

            if (!ValidateColorInput(out int colorCount))
            {
                return;
            }

            // Скидаємо кольори перед запуском
            _graph.ClearColors();
            foreach (var ellipse in _nodeVisuals.Values)
            {
                ellipse.Fill = _palette[0];
            }

            // Використовуємо новий алгоритм!
            ISolver solver = new SimulatedAnnealingSolver();
            int steps = solver.Solve(_graph, colorCount); // Дамо 3 кольори для цікавості

            // Оновлюємо картинку
            foreach (var node in _graph.Nodes)
            {
                if (_nodeVisuals.ContainsKey(node))
                {
                    _nodeVisuals[node].Fill = _palette[node.Color];
                }
            }

            //MessageBox.Show($"Емуляція відпалу завершила роботу за {steps} ітерацій!\nЗалишилось конфліктів: {_graph.CalculateConflicts()}", "Результат Відпалу");
            ResultTextBlock.Text =
                $"Метод: Simulated Annealing\n" +
                $"Ітерацій: {steps}\n" +
                $"Конфліктів: {_graph.CalculateConflicts()}";
        }

        // --- КНОПКА 4: Запуск Променевого пошуку ---
        private void BtnSolveBeam_Click(object sender, RoutedEventArgs e)
        {
            if (_graph.Nodes.Count == 0)
            {
                return;
            }

            if (!ValidateColorInput(out int colorCount))
            {
                return;
            }

            // Скидаємо кольори
            _graph.ClearColors();
            foreach (var ellipse in _nodeVisuals.Values)
            {
                ellipse.Fill = _palette[0];
            }

            ISolver solver = new BeamSearchSolver();
            int steps = solver.Solve(_graph, colorCount); // Нехай спробує розфарбувати в 3 кольори

            // Малюємо результат
            foreach (var node in _graph.Nodes)
            {
                if (_nodeVisuals.ContainsKey(node))
                {
                    _nodeVisuals[node].Fill = _palette[node.Color];
                }
            }

            ResultTextBlock.Text =
                $"Метод: Beam Search\n" +
                $"Ітерацій: {steps}\n" +
                $"Конфліктів: {_graph.CalculateConflicts()}";
        }

  

        // --- МЕТОД ВАЛІДАЦІЇ ---
        private bool ValidateColorInput(out int colorCount)
        {
            colorCount = 0;

            // 1. Перевірка: чи намальований граф взагалі
            if (_graph.Nodes.Count == 0)
            {
                MessageBox.Show("Спочатку намалюйте граф (додайте хоча б одну вершину)!", "Помилка");
                return false;
            }

            // 2. Перевірка: чи це взагалі ціле число
            if (!int.TryParse(ColorCountInput.Text, out colorCount))
            {
                MessageBox.Show("Будь ласка, введіть ціле число у поле кількості кольорів!", "Помилка вводу");
                return false;
            }

            // 3. Перевірка: чи число не менше 1
            if (colorCount < 1)
            {
                MessageBox.Show("Кількість кольорів має бути не менше 1!", "Помилка вводу");
                return false;
            }

            // 4. Перевірка: чи кольорів не більше, ніж вершин
            if (colorCount > _graph.Nodes.Count)
            {
                MessageBox.Show($"Кількість кольорів не може перевищувати кількість вершин!\nМаксимум для цього графа: {_graph.Nodes.Count}", "Помилка вводу");
                return false;
            }

            // Якщо всі перевірки пройдені успішно
            return true;
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
                        if (!_firstNodeSelected.Neighbors.Contains(node))
                        {
                            _graph.AddEdge(_firstNodeSelected, node);
                            DrawEdge(_firstNodeSelected, node);
                        }
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
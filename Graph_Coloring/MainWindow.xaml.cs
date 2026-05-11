// ==========================================================================================
// Файл: MainWindow.xaml.cs
// Призначення: Головне вікно програми. Відповідає за логіку взаємодії користувача з 
// графічним інтерфейсом (WPF), малювання графа на полотні та виклик алгоритмів розфарбовування.
// Використовувані бібліотеки:
// - System.Windows.* : для роботи з елементами керування WPF, подіями миші та візуальними формами.
// - System.Windows.Shapes : для малювання геометричних фігур (Лінії, Еліпси).
// - System.Windows.Media : для роботи з пензлями (Brushes) та кольорами.
// - System.IO, System.Text, Microsoft.Win32 : для форматування тексту та збереження звіту у файл.
// - Graph_Coloring.Models, Graph_Coloring.Algorithms : власні простори імен для роботи з логікою.
// ==========================================================================================
using System.IO;        
using System.Text;      
using Microsoft.Win32;  
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
        // Поле для зберігання абстрактної математичної моделі графа (тип Graph)
        private Graph _graph;

        // Словник для зв'язування логічної вершини з її візуальним відображенням на формі (тип Dictionary<Node, Ellipse>)
        private Dictionary<Node, Ellipse> _nodeVisuals;

        // Поле для відстеження першої виділеної вершини при створенні ребра (тип Node, може бути null)
        private Node? _firstNodeSelected = null;

        // Масив пензлів, що представляє палітру кольорів для розфарбовування (тип Brush[])
        // Індекс 0 (Brushes.White) використовується як стан "без кольору"
        private Brush[] _palette = new Brush[]
        {
            Brushes.White, Brushes.Tomato, Brushes.MediumSeaGreen, Brushes.DodgerBlue,
            Brushes.Gold, Brushes.BlueViolet, Brushes.Orange, Brushes.Orchid,
            Brushes.Purple, Brushes.PaleTurquoise, Brushes.Olive, Brushes.Aqua,
            Brushes.BlanchedAlmond, Brushes.Coral, Brushes.DarkBlue, Brushes.ForestGreen,
            Brushes.Honeydew, Brushes.Yellow, Brushes.Orange, Brushes.OldLace, Brushes.SteelBlue
        };

        /// <summary>
        /// Конструктор головного вікна. Ініціалізує компоненти WPF та внутрішні колекції.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            _graph = new Graph();
            _nodeVisuals = new Dictionary<Node, Ellipse>();
        }

        /// <summary>
        /// Обробник події натискання лівої кнопки миші по полотну (Canvas) для створення нової вершини.
        /// </summary>
        /// <param name="sender">Об'єкт, що викликав подію (тип object).</param>
        /// <param name="e">Аргументи події миші, що містять координати кліку (тип MouseButtonEventArgs).</param>
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

                if (distance < 40)
                {
                    return;
                }
            }

            _graph.AddNode(clickPosition.X, clickPosition.Y);
            DrawNode(_graph.Nodes[_graph.Nodes.Count - 1]);
        }

        /// <summary>
        /// Обробник події натискання кнопки "Очистити". Скидає граф та очищає інтерфейс.
        /// </summary>
        /// <param name="sender">Об'єкт кнопки (тип object).</param>
        /// <param name="e">Аргументи події маршрутизації (тип RoutedEventArgs).</param>
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            GraphCanvas.Children.Clear();
            _graph = new Graph();
            _nodeVisuals.Clear();
            _firstNodeSelected = null;

            ResultTextBlock.Text = "Результати з'являться тут...";
            ColorCountInput.Text = "0";
        }

        /// <summary>
        /// Обробник натискання кнопки для запуску методу "Сходження на гору".
        /// </summary>
        /// <param name="sender">Об'єкт кнопки (тип object).</param>
        /// <param name="e">Аргументи події (тип RoutedEventArgs).</param>
        private void BtnSolveHill_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateColorInput(out int colorCount))
            {
                return;
            }

            _graph.ClearColors();
            foreach (var ellipse in _nodeVisuals.Values)
            {
                ellipse.Fill = _palette[0];
            }

            ISolver solver = new HillClimbingSolver();
            
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

        /// <summary>
        /// Обробник натискання кнопки для запуску методу "Імітація відпалу".
        /// </summary>
        /// <param name="sender">Об'єкт кнопки (тип object).</param>
        /// <param name="e">Аргументи події (тип RoutedEventArgs).</param>  
        private void BtnSolveAnnealing_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateColorInput(out int colorCount))
            {
                return;
            }

            _graph.ClearColors();
            foreach (var ellipse in _nodeVisuals.Values)
            {
                ellipse.Fill = _palette[0];
            }

            ISolver solver = new SimulatedAnnealingSolver();

            int steps = solver.Solve(_graph, colorCount);

            foreach (var node in _graph.Nodes)
            {
                if (_nodeVisuals.ContainsKey(node))
                {
                    _nodeVisuals[node].Fill = _palette[node.Color];
                }
            }

            ResultTextBlock.Text =
                $"Метод: Simulated Annealing\n" +
                $"Ітерацій: {steps}\n" +
                $"Конфліктів: {_graph.CalculateConflicts()}";
        }

        /// <summary>
        /// Обробник натискання кнопки для запуску методу "Променевий пошук".
        /// </summary>
        /// <param name="sender">Об'єкт кнопки (тип object).</param>
        /// <param name="e">Аргументи події (тип RoutedEventArgs).</param>
        private void BtnSolveBeam_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateColorInput(out int colorCount))
            {
                return;
            }

            _graph.ClearColors();
            foreach (var ellipse in _nodeVisuals.Values)
            {
                ellipse.Fill = _palette[0];
            }

            ISolver solver = new BeamSearchSolver();
            int steps = solver.Solve(_graph, colorCount);

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


        /// <summary>
        /// Допоміжний метод для перевірки коректності вводу кількості кольорів у текстове поле.
        /// </summary>
        /// <param name="colorCount">Вихідний параметр, що повертає зчитану кількість кольорів (тип out int).</param>
        /// <returns>True, якщо дані валідні; інакше False (тип bool).</returns>
        private bool ValidateColorInput(out int colorCount)
        {
            colorCount = 0;

            if (_graph.Nodes.Count == 0)
            {
                MessageBox.Show("Спочатку намалюйте граф (додайте хоча б одну вершину)!", "Помилка");
                return false;
            }

            if (!int.TryParse(ColorCountInput.Text, out colorCount))
            {
                MessageBox.Show("Будь ласка, введіть ціле число у поле кількості кольорів!", "Помилка вводу");
                return false;
            }

            if (colorCount < 1)
            {
                MessageBox.Show("Кількість кольорів має бути не менше 1!", "Помилка вводу");
                return false;
            }

            if (colorCount > _graph.Nodes.Count)
            {
                MessageBox.Show($"Кількість кольорів не може перевищувати кількість вершин!\nМаксимум для цього графа: {_graph.Nodes.Count}", "Помилка вводу");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Обробник натискання кнопки "Зберегти результати". Генерує звіт та зберігає його у .txt файл.
        /// </summary>
        /// <param name="sender">Об'єкт кнопки (тип object).</param>
        /// <param name="e">Аргументи події (тип RoutedEventArgs).</param>
        private void BtnSaveFile_Click(object sender, RoutedEventArgs e)
        {
            if (_graph.Nodes.Count == 0)
            {
                MessageBox.Show("Немає графа для збереження. Спочатку намалюйте його!", "Помилка");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Текстові файли (*.txt)|*.txt|Всі файли (*.*)|*.*"; // лише .txt
            saveFileDialog.Title = "Зберегти звіт розфарбовування";
            saveFileDialog.FileName = "Graph_Report.txt"; // Назва файлу за замовчуванням

            if (saveFileDialog.ShowDialog() == true)
            {
                StringBuilder report = new StringBuilder();
                report.AppendLine("========================================");
                report.AppendLine("   ЗВІТ: РОЗФАРБОВУВАННЯ ГРАФА");
                report.AppendLine("========================================");
                report.AppendLine($"Дата та час: {DateTime.Now}");
                report.AppendLine($"Загальна кількість вершин: {_graph.Nodes.Count}");

                report.AppendLine("\n--- Останній запущений алгоритм ---");
                report.AppendLine(ResultTextBlock.Text);

                report.AppendLine("\n--- Деталізація по вершинах ---");
                for (int i = 0; i < _graph.Nodes.Count; i++)
                {
                    var currentNode = _graph.Nodes[i];

                    List<int> neighborIndices = new List<int>();
                    foreach (var neighbor in currentNode.Neighbors)
                    {
                        neighborIndices.Add(_graph.Nodes.IndexOf(neighbor) + 1);
                    }

                    string neighborsText = neighborIndices.Count > 0
                        ? string.Join(", ", neighborIndices)
                        : "немає";

                    report.AppendLine($"Вершина {i + 1}: Колір №{currentNode.Color} | Зв'язки з: [{neighborsText}]");
                }
                report.AppendLine("========================================");

                try
                {
                    File.WriteAllText(saveFileDialog.FileName, report.ToString());
                    MessageBox.Show("Звіт успішно збережено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Виникла помилка при збереженні файлу: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Допоміжний метод для візуального малювання ребра (лінії) між двома вершинами.
        /// </summary>
        /// <param name="a">Перша логічна вершина (тип Node).</param>
        /// <param name="b">Друга логічна вершина (тип Node).</param> 
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

            GraphCanvas.Children.Insert(0, line);
        }

        /// <summary>
        /// Допоміжний метод для візуального малювання нової вершини та підписки її на події кліку.
        /// </summary>
        /// <param name="node">Логічна модель вершини, яку треба відобразити (тип Node).</param>
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

            ellipse.MouseLeftButtonDown += (s, e) =>
            {
                if (_firstNodeSelected == null)
                {
                    _firstNodeSelected = node;
                    ellipse.Stroke = Brushes.Red;
                    ellipse.StrokeThickness = 4;
                }
                else
                {
                    if (_firstNodeSelected != node)
                    {
                        if (!_firstNodeSelected.Neighbors.Contains(node))
                        {
                            _graph.AddEdge(_firstNodeSelected, node);
                            DrawEdge(_firstNodeSelected, node);
                        }
                    }

                    _nodeVisuals[_firstNodeSelected].Stroke = Brushes.Black;
                    _nodeVisuals[_firstNodeSelected].StrokeThickness = 2;
                    _firstNodeSelected = null;
                }

                e.Handled = true;
            };

            GraphCanvas.Children.Add(ellipse);
            _nodeVisuals.Add(node, ellipse);

            TextBlock nodeNumber = new TextBlock
            {
                Text = (_graph.Nodes.Count - 1).ToString(),
                Width = 40,                           
                Height = 40,                          
                TextAlignment = TextAlignment.Center, 
                Padding = new Thickness(0, 10, 0, 0), 
                Foreground = Brushes.Black,           
                FontWeight = FontWeights.Bold,        
                FontSize = 14,                        
                IsHitTestVisible = false
            };

            Canvas.SetLeft(nodeNumber, node.X - 20);
            Canvas.SetTop(nodeNumber, node.Y - 20);

            GraphCanvas.Children.Add(nodeNumber);
        }
    }
}
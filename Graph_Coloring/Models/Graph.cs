// ==========================================================================================
// Файл: Graph.cs
// Призначення: Містить клас моделі графа, який керує колекцією вершин та зв'язками між ними.
// Використовувані бібліотеки: 
// - System.Collections.ObjectModel: для використання ObservableCollection (колекція, що сповіщає WPF про зміни).
// ==========================================================================================
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph_Coloring.Models
{
    /// <summary>
    /// Клас моделі графа. Зберігає всі вузли (вершини) та надає методи для управління ними.
    /// </summary>
    public class Graph
    {
        /// <summary>
        /// Колекція вершин графа. Використовується ObservableCollection для автоматичного 
        /// оновлення графічного інтерфейсу (WPF) при додаванні нових вершин.
        /// </summary>
        public ObservableCollection<Node> Nodes { get; private set; }

        /// <summary>
        /// Конструктор класу Graph. Ініціалізує порожню колекцію вершин.
        /// </summary>
        public Graph()
        {
            Nodes = new ObservableCollection<Node>();
        }

        /// <summary>
        /// Метод для додавання нової вершини до графа на заданих координатах.
        /// </summary>
        /// <param name="x">Координата X нової вершини на графічному полотні (тип double).</param>
        /// <param name="y">Координата Y нової вершини на графічному полотні (тип double).</param>
        public void AddNode(double x, double y)
        {
            int newId = Nodes.Count + 1;
            Nodes.Add(new Node(newId, x, y));
        }

        /// <summary>
        /// Метод для створення неорієнтованого ребра (зв'язку) між двома існуючими вершинами.
        /// </summary>
        /// <param name="source">Перша вершина (початкова), яку потрібно з'єднати (тип Node).</param>
        /// <param name="target">Друга вершина (кінцева), яку потрібно з'єднати (тип Node).</param>
        public void AddEdge(Node source, Node target)
        {
            if (source != null && target != null && source != target)
            {
                source.AddNeighbor(target);
            }
        }

        /// <summary>
        /// Метод для підрахунку загальної кількості конфліктів у розфарбуванні графа.
        /// Конфлікт виникає, коли дві суміжні вершини мають однаковий колір.
        /// </summary>
        /// <returns>Цілочисельне значення загальної кількості конфліктів (тип int).</returns>
        public int CalculateConflicts()
        {
            int totalConflicts = 0;

            foreach (var node in Nodes)
            {
                if (node.Color == 0)
                {
                    continue;
                }

                foreach (var neighbor in node.Neighbors)
                {
                    if (node.Color == neighbor.Color)
                    {
                        totalConflicts++;
                    }
                }
            }

            return totalConflicts / 2;
        }

        /// <summary>
        /// Метод для очищення розфарбування графа. Встановлює колір усіх вершин у 0 (незабарвлений стан).
        /// </summary>
        public void ClearColors()
        {
            foreach (var node in Nodes)
            {
                node.Color = 0;
            }
        }
    }
}

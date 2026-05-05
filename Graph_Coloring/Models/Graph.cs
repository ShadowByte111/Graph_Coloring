using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph_Coloring.Models
{
    public class Graph
    {
        // Список вузлів. ObservableCollection автоматично оновлює UI при додаванні/видаленні вузлів
        public ObservableCollection<Node> Nodes { get; private set; }

        public Graph()
        {
            Nodes = new ObservableCollection<Node>();
        }

        // Метод для створення нової вершини
        public void AddNode(double x, double y)
        {
            // Генеруємо ID на основі поточної кількості вузлів
            int newId = Nodes.Count + 1;
            Nodes.Add(new Node(newId, x, y));
        }

        // Метод для створення ребра між двома вузлами
        public void AddEdge(Node source, Node target)
        {
            if (source != null && target != null && source != target)
            {
                source.AddNeighbor(target);
            }
        }

        //[cite_start]
        // Цільова функція (Objective Function) - критично важлива для алгоритмів [cite: 669]
        // Рахує кількість ребер, де вузли мають однаковий колір
        public int CalculateConflicts()
        {
            int totalConflicts = 0;

            foreach (var node in Nodes)
            {
                // Якщо вузол не розфарбований, він не створює конфлікту за правилами
                if (node.Color == 0) continue;

                foreach (var neighbor in node.Neighbors)
                {
                    if (node.Color == neighbor.Color)
                    {
                        totalConflicts++;
                    }
                }
            }

            // Кожен конфлікт порахований двічі (для обох кінців ребра), тому ділимо на 2
            return totalConflicts / 2;
        }

        // Очищення кольорів перед запуском нового алгоритму
        public void ClearColors()
        {
            foreach (var node in Nodes)
            {
                node.Color = 0;
            }
        }
    }
}

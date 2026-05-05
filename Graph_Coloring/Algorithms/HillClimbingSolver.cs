using Graph_Coloring.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph_Coloring.Algorithms
{
    using System;
    using System.Linq;

    public class HillClimbingSolver : ISolver
    {
        private Random _random = new Random();

        public int Solve(Graph graph, int colorCount)
        {
            if (colorCount == 1)
            {
                foreach (var node in graph.Nodes) 
                { 
                    node.Color = 1; 
                }

                return 0; // 0 ітерацій, бо ми нічого не шукали
            }

            int iterations = 0;
            int maxIterations = 10000; // Щоб алгоритм не працював вічно

            // 1. Початковий стан: фарбуємо все випадково
            foreach (var node in graph.Nodes)
            {
                node.Color = _random.Next(1, colorCount + 1);
            }

            int currentConflicts = graph.CalculateConflicts();

            // 2. Основний цикл покращення
            while (currentConflicts > 0 && iterations < maxIterations)
            {
                iterations++;

                // Вибираємо випадкову ноду
                var randomNode = graph.Nodes[_random.Next(graph.Nodes.Count)];
                int oldColor = randomNode.Color;

                // Пробуємо змінити її колір на інший випадковий
                int newColor = _random.Next(1, colorCount + 1);
                if (newColor == oldColor)
                {
                    continue;
                }

                randomNode.Color = newColor;
                int newConflicts = graph.CalculateConflicts();

                // Якщо стало краще або так само (для виходу з плато) — залишаємо
                if (newConflicts <= currentConflicts)
                {
                    currentConflicts = newConflicts;
                }
                else
                {
                    // Якщо стало гірше — повертаємо старий колір
                    randomNode.Color = oldColor;
                }
            }

            return iterations;
        }
    }
}

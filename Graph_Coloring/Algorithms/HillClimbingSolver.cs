// ==========================================================================================
// Файл: HillClimbingSolver.cs
// Призначення: Містить реалізацію алгоритму "Сходження на гору" (Hill Climbing).
// Використовує жадібний підхід для локального пошуку оптимального розфарбування.
// ==========================================================================================
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

    /// <summary>
    /// Клас, що реалізує метод локального пошуку "Сходження на гору".
    /// </summary>
    public class HillClimbingSolver : ISolver
    {
        private Random _random = new Random();

        /// <summary>
        /// Метод розв'язання задачі за допомогою жадібного локального пошуку.
        /// </summary>
        /// <param name="graph">Екземпляр графа для розфарбовування (тип Graph).</param>
        /// <param name="colorCount">Кількість доступних кольорів з палітри (тип int).</param>
        /// <returns>Кількість виконаних ітерацій алгоритму (тип int).</returns>
        public int Solve(Graph graph, int colorCount)
        {
            int iterations = 0;
            int maxIterations = 10000;

            foreach (var node in graph.Nodes)
            {
                node.Color = _random.Next(1, colorCount + 1);
            }

            int currentConflicts = graph.CalculateConflicts();

            while (currentConflicts > 0 && iterations < maxIterations)
            {
                iterations++;

                var randomNode = graph.Nodes[_random.Next(graph.Nodes.Count)];
                int oldColor = randomNode.Color;

                int newColor = _random.Next(1, colorCount + 1);
                if (newColor == oldColor)
                {
                    continue;
                }

                randomNode.Color = newColor;
                int newConflicts = graph.CalculateConflicts();

                if (newConflicts <= currentConflicts)
                {
                    currentConflicts = newConflicts;
                }
                else
                {
                    randomNode.Color = oldColor;
                }
            }

            return iterations;
        }
    }
}

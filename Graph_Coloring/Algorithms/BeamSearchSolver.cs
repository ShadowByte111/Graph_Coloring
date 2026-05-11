// ==========================================================================================
// Файл: BeamSearchSolver.cs
// Призначення: Містить реалізацію алгоритму "Локальний променевий пошук" (Local Beam Search).
// Використовує бібліотеку System.Linq для сортування та маніпуляцій з колекціями станів.
// ==========================================================================================
using Graph_Coloring.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph_Coloring.Algorithms
{
    /// <summary>
    /// Клас, що реалізує метод "Локальний променевий пошук".
    /// Досліджує кілька найкращих станів паралельно.
    /// </summary>
    public class BeamSearchSolver : ISolver
    {
        // Генератор псевдовипадкових чисел (тип Random)
        private Random _random = new Random();

        // Ширина променя - кількість найперспективніших станів, що зберігаються на кожному кроці (тип int)
        private int _beamWidth = 5;

        /// <summary>
        /// Метод розв'язання задачі за допомогою променевого пошуку.
        /// </summary>
        /// <param name="graph">Екземпляр графа для розфарбовування (тип Graph).</param>
        /// <param name="colorCount">Кількість доступних кольорів (тип int).</param>
        /// <returns>Кількість виконаних ітерацій (тип int).</returns>
        public int Solve(Graph graph, int colorCount)
        {
            int n = graph.Nodes.Count;
            if (n == 0)
            {
                return 0;
            }

            int iterations = 0;
            int maxIterations = 500;

            List<int[]> beam = new List<int[]>();

            for (int i = 0; i < _beamWidth; i++)
            {
                int[] state = new int[n];
                for (int j = 0; j < n; j++)
                {
                    state[j] = _random.Next(1, colorCount + 1);
                }
                beam.Add(state);
            }

            while (iterations < maxIterations)
            {
                iterations++;
                List<int[]> allNeighbors = new List<int[]>();

                foreach (var state in beam)
                {
                    for (int i = 0; i < n; i++)
                    {
                        for (int c = 1; c <= colorCount; c++)
                        {
                            if (state[i] == c) continue;

                            int[] neighbor = (int[])state.Clone();
                            neighbor[i] = c;
                            allNeighbors.Add(neighbor);
                        }
                    }
                }

                allNeighbors.AddRange(beam);

                beam = allNeighbors
                    .OrderBy(state => CalculateStateConflicts(graph, state))
                    .Take(_beamWidth)
                    .ToList();

                if (CalculateStateConflicts(graph, beam.First()) == 0)
                {
                    break;
                }
            }

            int[] bestState = beam.First();
            for (int i = 0; i < n; i++)
            {
                graph.Nodes[i].Color = bestState[i];
            }

            return iterations;
        }

        /// <summary>
        /// Допоміжний метод для підрахунку конфліктів конкретного абстрактного стану (масиву кольорів).
        /// Оцінює стан без його застосування до візуального графа.
        /// </summary>
        /// <param name="graph">Екземпляр графа для зчитування топології зв'язків (тип Graph).</param>
        /// <param name="stateColors">Масив цілих чисел, що представляє тестові кольори вершин (тип int[]).</param>
        /// <returns>Загальна кількість конфліктів у заданому стані (тип int).</returns>
        private int CalculateStateConflicts(Graph graph, int[] stateColors)
        {
            int conflicts = 0;
            for (int i = 0; i < graph.Nodes.Count; i++)
            {
                var node = graph.Nodes[i];
                foreach (var neighbor in node.Neighbors)
                {
                    int neighborIndex = graph.Nodes.IndexOf(neighbor);
                    if (stateColors[i] == stateColors[neighborIndex])
                    {
                        conflicts++;
                    }
                }
            }
            return conflicts / 2;
        }
    }
}

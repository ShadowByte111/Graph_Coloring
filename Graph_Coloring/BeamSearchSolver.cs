using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph_Coloring
{
    public class BeamSearchSolver : ISolver
    {
        private Random _random = new Random();
        private int _beamWidth = 5; // Кількість "променів" (k)

        public int Solve(Graph graph, int colorCount)
        {
            int n = graph.Nodes.Count;
            if (n == 0) return 0;

            int iterations = 0;
            int maxIterations = 500;

            // Зберігаємо стани як масиви кольорів (це працює блискавично швидко)
            List<int[]> beam = new List<int[]>();

            // 1. СТАРТ: Генеруємо k початкових випадкових станів
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

                // 2. ГЕНЕРАЦІЯ: Створюємо сусідів для кожного стану в промені
                foreach (var state in beam)
                {
                    // Пробуємо змінити колір кожної вершини
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

                // Додаємо поточні промені до списку кандидатів (раптом вони кращі за сусідів)
                allNeighbors.AddRange(beam);

                // 3. ВІДБІР: Залишаємо лише k найкращих станів
                beam = allNeighbors
                    .OrderBy(state => CalculateStateConflicts(graph, state))
                    .Take(_beamWidth)
                    .ToList();

                // 4. ЗУПИНКА: Якщо найкращий промінь має 0 конфліктів — ми перемогли!
                if (CalculateStateConflicts(graph, beam.First()) == 0)
                {
                    break;
                }
            }

            // 5. ЗАВЕРШЕННЯ: Застосовуємо найкращий знайдений стан до нашого реального графа
            int[] bestState = beam.First();
            for (int i = 0; i < n; i++)
            {
                graph.Nodes[i].Color = bestState[i];
            }

            return iterations;
        }

        // Спеціальний швидкий метод підрахунку конфліктів для масиву
        private int CalculateStateConflicts(Graph graph, int[] stateColors)
        {
            int conflicts = 0;
            for (int i = 0; i < graph.Nodes.Count; i++)
            {
                var node = graph.Nodes[i];
                foreach (var neighbor in node.Neighbors)
                {
                    // Отримуємо порядковий номер сусіда, щоб знайти його колір у масиві
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

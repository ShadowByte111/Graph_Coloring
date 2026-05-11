// ==========================================================================================
// Файл: SimulatedAnnealingSolver.cs
// Призначення: Містить реалізацію метаевристичного алгоритму "Імітація відпалу" (Simulated Annealing).
// Дозволяє уникати локальних мінімумів за допомогою ймовірнісного прийняття гірших станів.
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
    /// Клас, що реалізує стохастичний метод "Імітація відпалу".
    /// </summary>
    public class SimulatedAnnealingSolver : ISolver
    {
        private Random _random = new Random();

        public int Solve(Graph graph, int colorCount)
        {

            foreach (var node in graph.Nodes)
            {
                node.Color = _random.Next(1, colorCount + 1);
            }

            int currentConflicts = graph.CalculateConflicts();
            int iterations = 0;

            double temperature = 100.0;
            double coolingRate = 0.99;
            double absoluteTemperature = 0.00001;

            while (temperature > absoluteTemperature && currentConflicts > 0)
            {
                iterations++;

                var randomNode = graph.Nodes[_random.Next(graph.Nodes.Count)];
                int oldColor = randomNode.Color;
                int newColor = _random.Next(1, colorCount + 1);

                if (newColor != oldColor)
                {
                    randomNode.Color = newColor;
                    int newConflicts = graph.CalculateConflicts();

                    int deltaE = newConflicts - currentConflicts;

                    if (deltaE <= 0)
                    {
                        currentConflicts = newConflicts;
                    }
                    else
                    {
                        double probability = Math.Exp(-deltaE / temperature);
                        double randomValue = _random.NextDouble();

                        if (randomValue < probability)
                        {
                            currentConflicts = newConflicts;
                        }
                        else
                        {
                            randomNode.Color = oldColor;
                        }
                    }
                }

                temperature *= coolingRate;
            }

            return iterations;
        }
    }
}

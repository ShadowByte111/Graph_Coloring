using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph_Coloring
{
    public class SimulatedAnnealingSolver : ISolver
    {
        private Random _random = new Random();

        public int Solve(Graph graph, int colorCount)
        {
            // 1. Початковий стан: випадкове розфарбування
            foreach (var node in graph.Nodes)
            {
                node.Color = _random.Next(1, colorCount + 1);
            }

            int currentConflicts = graph.CalculateConflicts();
            int iterations = 0;

            // Налаштування "відпалу"
            double temperature = 100.0;     // Початкова температура
            double coolingRate = 0.99;      // Швидкість охолодження (чим ближче до 1, тим довше шукає)
            double absoluteTemperature = 0.00001; // Температура зупинки

            while (temperature > absoluteTemperature && currentConflicts > 0)
            {
                iterations++;

                // Вибираємо випадкову ноду і новий випадковий колір
                var randomNode = graph.Nodes[_random.Next(graph.Nodes.Count)];
                int oldColor = randomNode.Color;
                int newColor = _random.Next(1, colorCount + 1);

                if (newColor == oldColor) continue;

                // Робимо тестовий крок
                randomNode.Color = newColor;
                int newConflicts = graph.CalculateConflicts();

                // Різниця між новим станом і старим
                int deltaE = newConflicts - currentConflicts;

                // Якщо стало краще (deltaE < 0) або так само — ПРИЙМАЄМО 100%
                if (deltaE <= 0)
                {
                    currentConflicts = newConflicts;
                }
                else
                {
                    // Якщо стало гірше — приймаємо з певною ймовірністю, яка залежить від температури
                    double probability = Math.Exp(-deltaE / temperature);
                    double randomValue = _random.NextDouble(); // Випадкове число від 0.0 до 1.0

                    if (randomValue < probability)
                    {
                        // Ризикуємо і приймаємо гірший крок!
                        currentConflicts = newConflicts;
                    }
                    else
                    {
                        // Не пощастило, відхиляємо і повертаємо старий колір
                        randomNode.Color = oldColor;
                    }
                }

                // Охолоджуємо систему
                temperature *= coolingRate;
            }

            return iterations;
        }
    }
}

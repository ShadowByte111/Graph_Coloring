using Graph_Coloring.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph_Coloring.Algorithms
{
    public class BeamSearchSolver : ISolver
    {
        private Random _random = new Random();
        private int _beamWidth = 5;

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

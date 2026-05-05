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
        public ObservableCollection<Node> Nodes { get; private set; }

        public Graph()
        {
            Nodes = new ObservableCollection<Node>();
        }

        public void AddNode(double x, double y)
        {
            int newId = Nodes.Count + 1;
            Nodes.Add(new Node(newId, x, y));
        }

        public void AddEdge(Node source, Node target)
        {
            if (source != null && target != null && source != target)
            {
                source.AddNeighbor(target);
            }
        }

        public int CalculateConflicts()
        {
            int totalConflicts = 0;

            foreach (var node in Nodes)
            {
                if (node.Color == 0) continue;

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

        public void ClearColors()
        {
            foreach (var node in Nodes)
            {
                node.Color = 0;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph_Coloring
{
    internal interface ISolver
    {
        int Solve(Graph graph, int colorCount);
    }
}

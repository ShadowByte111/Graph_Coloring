// ==========================================================================================
// Файл: ISolver.cs
// Призначення: Визначає загальний інтерфейс для всіх алгоритмів розв'язання задачі розфарбовування графа.
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
    /// Загальний інтерфейс для евристичних алгоритмів розфарбовування.
    /// Забезпечує поліморфізм для виклику різних методів з головного вікна.
    /// </summary>
    internal interface ISolver
    {
        /// <summary>
        /// Головний метод для запуску алгоритму розфарбовування.
        /// </summary>
        /// <param name="graph">Об'єкт графа, який необхідно розфарбувати (тип Graph).</param>
        /// <param name="colorCount">Максимальна допустима кількість кольорів (тип int).</param>
        /// <returns>Кількість виконаних ітерацій алгоритму (тип int).</returns>
        int Solve(Graph graph, int colorCount);
    }
}

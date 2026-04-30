using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Graph_Coloring
{
    public class Node : INotifyPropertyChanged
    {
        // Приватні поля (зберігають фактичні значення)
        private int _color;
        private double _x;
        private double _y;

        // Унікальний номер вершини (ID не змінюється в процесі роботи, тому він простий)
        public int Id { get; set; }

        // Список сусідів. Завдяки йому алгоритмам не треба перебирати весь граф
        public List<Node> Neighbors { get; private set; }

        // Конструктор: те, що викликається при створенні нової вершини
        public Node(int id, double x, double y)
        {
            Id = id;
            X = x; // Використовуємо властивості з великої літери
            Y = y;
            Color = 0; // 0 - вершина ще не розфарбована
            Neighbors = new List<Node>();
        }

        // Властивість Колір. 
        // Коли алгоритм робить Node.Color = 1, спрацьовує блок "set", 
        // який змінює значення і подає сигнал інтерфейсу "Перемалюй мене!"
        public int Color
        {
            get { return _color; }
            set
            {
                if (_color != value)
                {
                    _color = value;
                    OnPropertyChanged(); // Той самий сигнал
                }
            }
        }

        // Координата X для малювання на полотні Canvas
        public double X
        {
            get { return _x; }
            set
            {
                if (_x != value)
                {
                    _x = value;
                    OnPropertyChanged();
                }
            }
        }

        // Координата Y для малювання на полотні Canvas
        public double Y
        {
            get { return _y; }
            set
            {
                if (_y != value)
                {
                    _y = value;
                    OnPropertyChanged();
                }
            }
        }

        // Метод додавання сусіда (відразу в обидва боки, бо граф неорієнтований)
        public void AddNeighbor(Node neighbor)
        {
            // Перевіряємо, чи немає вже такого сусіда, щоб не було дублікатів
            if (!Neighbors.Contains(neighbor))
            {
                Neighbors.Add(neighbor);
                neighbor.Neighbors.Add(this);
            }
        }


        // --- МАГІЯ WPF (Реалізація INotifyPropertyChanged) ---

        public event PropertyChangedEventHandler? PropertyChanged;

        // [CallerMemberName] автоматично підставляє ім'я властивості, яка змінилася 
        // (наприклад, "Color" або "X"), щоб WPF знав, що саме треба оновити на екрані.
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

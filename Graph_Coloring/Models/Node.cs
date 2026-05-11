// ==========================================================================================
// Файл: Node.cs
// Призначення: Представляє окрему вершину (вузол) графа. Містить інформацію про її колір, координати та сусідів.
// Використовувані бібліотеки:
// - System.Collections.Generic: для використання узагальненої колекції List.
// - System.ComponentModel: для використання інтерфейсу INotifyPropertyChanged (оновлення UI).
// - System.Runtime.CompilerServices: для використання атрибуту CallerMemberName.
// ==========================================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Graph_Coloring.Models
{
    /// <summary>
    /// Клас моделі вершини графа. Реалізує інтерфейс INotifyPropertyChanged для 
    /// динамічного оновлення властивостей на формі WPF.
    /// </summary>
    public class Node : INotifyPropertyChanged
    {
        // Приватне поле для зберігання поточного індексу кольору вершини (тип int)
        private int _color;

        // Приватне поле для зберігання координати X вершини (тип double)
        private double _x;

        // Приватне поле для зберігання координати Y вершини (тип double)
        private double _y;

        /// <summary>
        /// Унікальний ідентифікатор вершини (тип int).
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Список суміжних вершин (сусідів), з якими ця вершина з'єднана ребрами (тип List<Node>).
        /// </summary>
        public List<Node> Neighbors { get; private set; }

        /// <summary>
        /// Конструктор класу Node. Ініціалізує нову вершину із заданими параметрами.
        /// </summary>
        /// <param name="id">Унікальний цілочисельний ідентифікатор вершини.</param>
        /// <param name="x">Початкова координата X на полотні (тип double).</param>
        /// <param name="y">Початкова координата Y на полотні (тип double).</param>
        public Node(int id, double x, double y)
        {
            Id = id;
            X = x;
            Y = y;
            Color = 0;
            Neighbors = new List<Node>();
        }

        /// <summary>
        /// Властивість доступу до кольору вершини. 
        /// При зміні значення викликає подію OnPropertyChanged для оновлення UI.
        /// </summary>
        public int Color
        {
            get { return _color; }
            set
            {
                if (_color != value)
                {
                    _color = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Властивість доступу до координати X вершини.
        /// При зміні значення викликає подію OnPropertyChanged для оновлення UI.
        /// </summary>
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

        /// <summary>
        /// Властивість доступу до координати Y вершини.
        /// При зміні значення викликає подію OnPropertyChanged для оновлення UI.
        /// </summary>
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

        /// <summary>
        /// Метод для встановлення двостороннього зв'язку (ребра) із сусідньою вершиною.
        /// </summary>
        /// <param name="neighbor">Об'єкт цільової суміжної вершини (тип Node).</param>
        public void AddNeighbor(Node neighbor)
        {
            if (!Neighbors.Contains(neighbor))
            {
                Neighbors.Add(neighbor);
                neighbor.Neighbors.Add(this);
            }
        }


        // WPF INotifyPropertyChanged

        /// <summary>
        /// Подія, що виникає при зміні значення будь-якої властивості класу.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Допоміжний метод для виклику події PropertyChanged.
        /// </summary>
        /// <param name="propertyName">Ім'я властивості, що змінилася (автоматично підставляється компілятором).</param>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

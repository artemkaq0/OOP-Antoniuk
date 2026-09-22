using System;

namespace Lab5
{
    public class IntArray
    {
        private readonly int[] _data;

        public int Length => _data.Length;

        public IntArray(int size)
        {
            if (size < 0)
                throw new ArgumentOutOfRangeException(nameof(size), "Розмір масиву не може бути від'ємним.");

            _data = new int[size];
        }

        public IntArray(params int[] elements)
        {
            _data = elements != null ? (int[])elements.Clone() : Array.Empty<int>();
        }

        public int this[int index]
        {
            get
            {
                ValidateIndex(index);
                return _data[index];
            }
            set
            {
                ValidateIndex(index);
                _data[index] = value;
            }
        }

        private void ValidateIndex(int index)
        {
            if (index < 0 || index >= _data.Length)
            {
                throw new IndexOutOfRangeException($"Індекс {index} виходить за межі масиву розмірністю {_data.Length}.");
            }
        }

        public static IntArray operator +(IntArray a, IntArray b)
        {
            if (a is null || b is null)
                throw new ArgumentNullException(a is null ? nameof(a) : nameof(b));

            int minLength = Math.Min(a.Length, b.Length);
            int maxLength = Math.Max(a.Length, b.Length);

            int[] result = new int[maxLength];

            for (int i = 0; i < minLength; i++)
            {
                result[i] = a[i] + b[i];
            }

            IntArray longer = a.Length > b.Length ? a : b;
            for (int i = minLength; i < maxLength; i++)
            {
                result[i] = longer[i];
            }

            return new IntArray(result);
        }

        public static bool operator ==(IntArray? a, IntArray? b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;
            return a.Equals(b);
        }

        public static bool operator !=(IntArray? a, IntArray? b)
        {
            return !(a == b);
        }

        public override bool Equals(object? obj)
        {
            if (obj is IntArray other)
            {
                if (Length != other.Length) return false;
                for (int i = 0; i < Length; i++)
                {
                    if (_data[i] != other._data[i]) return false;
                }
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            foreach (var item in _data)
            {
                hash.Add(item);
            }
            return hash.ToHashCode();
        }

        public override string ToString()
        {
            return $"[{string.Join(", ", _data)}]";
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Демонстрація роботи класу IntArray (Варіант 1)\n");

            IntArray array1 = new IntArray(10, 20, 30, 40);
            IntArray array2 = new IntArray(1, 2, 3, 4);
            IntArray array3 = new IntArray(10, 20, 30, 40);

            Console.WriteLine($"Масив 1 (array1): {array1}");
            Console.WriteLine($"Масив 2 (array2): {array2}");
            Console.WriteLine($"Масив 3 (array3): {array3}\n");

            Console.WriteLine("1. Тестування індексатора");
            Console.WriteLine($"Читання елемента array1[2]: {array1[2]}");

            array1[2] = 99;
            Console.WriteLine($"Запис значення 99 в array1[2]. Новий array1: {array1}");

            array1[2] = 30;
            Console.WriteLine($"Відновлення array1[2] = 30: {array1}\n");

            Console.WriteLine("2. Перевантажений оператор + (поелементне додавання)");
            IntArray sumArray = array1 + array2;
            Console.WriteLine($"array1 + array2 = {sumArray}\n");

            Console.WriteLine("3. Перевантажені оператори == та !=");
            Console.WriteLine($"array1 == array3: {array1 == array3} (Очікується True)");
            Console.WriteLine($"array1 == array2: {array1 == array2} (Очікується False)");
            Console.WriteLine($"array1 != array2: {array1 != array2} (Очікується True)");
            Console.WriteLine($"array1.Equals(array3): {array1.Equals(array3)} (Очікується True)\n");

            Console.WriteLine("4. Перевірка виходу за межі масиву");
            try
            {
                int invalidElement = array1[10];
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine($"Спіймано виключення: {ex.Message}");
            }
        }
    }
}
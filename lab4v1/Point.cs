using System;

namespace GeometryApp
{
    public class Point
    {
        private int _x;
        private int _y;

        public static Point Origin => new Point(0, 0);

        public int X
        {
            get => _x;
            set
            {
                if (value < -1000 || value > 1000)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Координата X повинна бути в діапазоні від -1000 до 1000.");
                }
                _x = value;
            }
        }

        public int Y
        {
            get => _y;
            set
            {
                if (value < -1000 || value > 1000)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Координата Y повинна бути в діапазоні від -1000 до 1000.");
                }
                _y = value;
            }
        }

        public Point(int x = 0, int y = 0)
        {
            X = x;
            Y = y;
        }

        public int this[int index]
        {
            get
            {
                return index switch
                {
                    0 => X,
                    1 => Y,
                    _ => throw new IndexOutOfRangeException("Індекс повинен бути 0 (для X) або 1 (для Y).")
                };
            }
            set
            {
                switch (index)
                {
                    case 0:
                        X = value;
                        break;
                    case 1:
                        Y = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Індекс повинен бути 0 (для X) або 1 (для Y).");
                }
            }
        }

        public static Point operator +(Point p1, Point p2)
        {
            return new Point(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static Point operator *(Point p, int scalar)
        {
            return new Point(p.X * scalar, p.Y * scalar);
        }

        public static Point operator *(int scalar, Point p)
        {
            return p * scalar;
        }

        public static bool operator ==(Point? p1, Point? p2)
        {
            if (ReferenceEquals(p1, p2)) return true;
            if (p1 is null || p2 is null) return false;
            return p1.Equals(p2);
        }

        public static bool operator !=(Point? p1, Point? p2) => !(p1 == p2);

        public override bool Equals(object? obj)
        {
            if (obj is Point p)
            {
                return X == p.X && Y == p.Y;
            }
            return false;
        }

        public override int GetHashCode() => HashCode.Combine(X, Y);

        public override string ToString() => $"({X}, {Y})";
    }
}
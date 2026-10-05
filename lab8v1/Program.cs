using System;
using System.Collections.Generic;

namespace lab8v1
{
    class Shape
    {
        public string Color { get; set; }

        public Shape(string color)
        {
            Color = color;
        }

        public virtual double GetArea()
        {
            return 0;
        }

        public virtual double GetPerimeter()
        {
            return 0;
        }
    }

    class Circle : Shape
    {
        public double Radius { get; set; }

        public Circle(string color, double radius) : base(color)
        {
            Radius = radius;
        }

        public override double GetArea()
        {
            return Math.PI * Radius * Radius;
        }

        public override double GetPerimeter()
        {
            return 2 * Math.PI * Radius;
        }
    }

    class Rectangle : Shape
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public Rectangle(string color, double width, double height) : base(color)
        {
            Width = width;
            Height = height;
        }

        public override double GetArea()
        {
            return Width * Height;
        }

        public override double GetPerimeter()
        {
            return 2 * (Width + Height);
        }
    }

    class Triangle : Shape
    {
        public double SideA { get; set; }
        public double SideB { get; set; }
        public double SideC { get; set; }

        public Triangle(string color, double sideA, double sideB, double sideC) : base(color)
        {
            SideA = sideA;
            SideB = sideB;
            SideC = sideC;
        }

        public override double GetPerimeter()
        {
            return SideA + SideB + SideC;
        }

        public override double GetArea()
        {
            double p = GetPerimeter() / 2;
            return Math.Sqrt(p * (p - SideA) * (p - SideB) * (p - SideC));
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            List<Shape> shapes = new List<Shape>
            {
                new Circle("Червоний", 5.0),
                new Rectangle("Синій", 4.0, 6.0),
                new Triangle("Зелений", 3.0, 4.0, 5.0)
            };

            double totalArea = 0;
            double totalPerimeter = 0;

            Console.WriteLine("Демонстрація поліморфізму");
            foreach (var shape in shapes)
            {
                double area = shape.GetArea();
                double perimeter = shape.GetPerimeter();

                totalArea += area;
                totalPerimeter += perimeter;

                Console.WriteLine($"Фігура: {shape.GetType().Name} ({shape.Color})");
                Console.WriteLine($"  Площа: {area:F2}");
                Console.WriteLine($"  Периметр: {perimeter:F2}");
            }

            Console.WriteLine("\nРезультати агрегації");
            Console.WriteLine($"Загальна площа всіх фігур: {totalArea:F2}");
            Console.WriteLine($"Загальний периметр всіх фігур: {totalPerimeter:F2}");

            Console.ReadLine();
        }
    }
}
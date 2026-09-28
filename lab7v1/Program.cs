using System;

namespace lab7v1
{
    class Vehicle
    {
        public virtual void Move()
        {
            Console.WriteLine("Транспортний засіб якось рухається");
        }
    }

    class Car : Vehicle
    {
        public override void Move()
        {
            Console.WriteLine("Їде по дорозі");
        }
    }

    class Bicycle : Vehicle
    {
        public new void Move()
        {
            Console.WriteLine("Крутить педалі по стежці");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Vehicle myCar = new Car();
            Vehicle myBicycle = new Bicycle();

            Console.WriteLine("Використання посилання базового класу (Upcasting)");
            Console.Write("myCar.Move() -> ");
            myCar.Move(); 
            
            Console.Write("myBicycle.Move() -> ");
            myBicycle.Move(); 

            Console.WriteLine("\nВикористання посилання похідного класу (Downcasting)");
            Console.Write("((Car)myCar).Move() -> ");
            ((Car)myCar).Move(); 
            
            Console.Write("((Bicycle)myBicycle).Move() -> ");
            ((Bicycle)myBicycle).Move(); 

            Console.ReadLine();
        }
    }
}
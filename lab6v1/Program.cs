using System;

namespace Lab6
{
    // Базовий клас
    class Employee
    {
        public string Name { get; set; }
        public decimal Salary { get; set; }

        public Employee(string name, decimal salary)
        {
            Name = name;
            Salary = salary;
        }

        public virtual void DisplayInfo()
        {
            Console.WriteLine($"[Employee] Ім'я: {Name}, Зарплата: {Salary:C}");
        }

        public string GetRole()
        {
            return "Employee";
        }
    }

    class Manager : Employee
    {
        public string Department { get; set; }

        public Manager(string name, decimal salary, string department) : base(name, salary)
        {
            Department = department;
        }

        public override void DisplayInfo()
        {
            Console.WriteLine($"[Manager] Ім'я: {Name}, Зарплата: {Salary:C}, Відділ: {Department}");
        }

        public void ManageTeam()
        {
            Console.WriteLine($"{Name} керує командою відділу {Department}.");
        }

        public new string GetRole()
        {
            return "Manager";
        }
    }

    class Director : Manager
    {
        public decimal Bonus { get; set; }

        public Director(string name, decimal salary, string department, decimal bonus) 
            : base(name, salary, department)
        {
            Bonus = bonus;
        }

        public override void DisplayInfo()
        {
            Console.WriteLine($"[Director] Ім'я: {Name}, Зарплата: {Salary:C}, Відділ: {Department}, Бонус: {Bonus:C}");
        }

        public void LeadCompany()
        {
            Console.WriteLine($"{Name} керує всією компанією.");
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Employee emp = new Employee("Іван", 15000);
            Manager mgr = new Manager("Олена", 25000, "IT");
            Director dir = new Director("Петро", 50000, "Опції та Стратегія", 100000);

            Console.WriteLine("Демонстрація поліморфізму (virtual / override)");
            Employee[] staff = { emp, mgr, dir };
            
            foreach (var person in staff)
            {
                person.DisplayInfo();
            }

            Console.WriteLine("\nУнікальні методи похідних класів");
            mgr.ManageTeam();
            dir.LeadCompany();

            Console.WriteLine("\nДемонстрація різниці між override та new");
            Employee empRefToManager = new Manager("Марія", 28000, "HR");

            Console.Write("Виклик DisplayInfo() через Employee-посилання: ");
            empRefToManager.DisplayInfo(); 

            Console.WriteLine($"\nВиклик GetRole() через Employee-посилання: {empRefToManager.GetRole()}");
            
            Console.WriteLine($"Виклик GetRole() через Manager-посилання: {mgr.GetRole()}");
        }
    }
}
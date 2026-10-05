using System;

namespace IndependentWork1
{
    public class FitnessTracker
    {
        private string _ownerName;
        private int _stepsToday;
        private int _dailyStepGoal;

        public string OwnerName
        {
            get { return _ownerName; }
        }

        public int StepsToday
        {
            get { return _stepsToday; }
            set { _stepsToday = value; }
        }

        public FitnessTracker(string ownerName, int dailyStepGoal)
        {
            _ownerName = ownerName;
            _dailyStepGoal = dailyStepGoal;
            _stepsToday = 0;
        }

        public double GetGoalProgressPercentage()
        {
            if (_dailyStepGoal == 0) return 0;
            
            double progress = ((double)_stepsToday / _dailyStepGoal) * 100;
            return progress;
        }

        public void AddSteps(int steps)
        {
            if (steps > 0)
            {
                _stepsToday += steps;
            }
        }
    }

    public class CoffeeMachine
    {
        private string _brand;
        private double _waterLevelLiters;

        public string Brand
        {
            get { return _brand; }
        }

        public double WaterLevelLiters
        {
            get { return _waterLevelLiters; }
            set { _waterLevelLiters = value; }
        }

        public CoffeeMachine(string brand, double initialWaterLiters)
        {
            _brand = brand;
            _waterLevelLiters = initialWaterLiters;
        }

        public bool MakeEspresso()
        {
            double espressoWaterNeeded = 0.05;

            if (_waterLevelLiters >= espressoWaterNeeded)
            {
                _waterLevelLiters -= espressoWaterNeeded;
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Демонстрація FitnessTracker");
            
            FitnessTracker myTracker = new FitnessTracker("Артем Антонюк", 10000);
            
            myTracker.AddSteps(4500);
            double progress = myTracker.GetGoalProgressPercentage();
            
            Console.WriteLine($"Власник: {myTracker.OwnerName}");
            Console.WriteLine($"Пройдено кроків: {myTracker.StepsToday}");
            Console.WriteLine($"Прогрес досягнення мети: {progress:F1}%\n");

            
            Console.WriteLine("Демонстрація CoffeeMachine");
            
            CoffeeMachine officeMachine = new CoffeeMachine("DeLonghi", 0.08);
            
            Console.WriteLine($"Кавомашина: {officeMachine.Brand}");
            Console.WriteLine($"Початковий рівень води: {officeMachine.WaterLevelLiters} л.");
            
            bool isFirstCoffeeMade = officeMachine.MakeEspresso();
            Console.WriteLine($"Перше еспресо зроблено? {isFirstCoffeeMade}");
            Console.WriteLine($"Залишок води: {officeMachine.WaterLevelLiters:F2} л.");
            
            bool isSecondCoffeeMade = officeMachine.MakeEspresso();
            Console.WriteLine($"Друге еспресо зроблено? {isSecondCoffeeMade}");
            Console.WriteLine($"Залишок води: {officeMachine.WaterLevelLiters:F2} л.");
        }
    }
}
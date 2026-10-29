using System;
using System.Collections.Generic;

namespace lab14v1
{
    class MyTask
    {
        public string Title;
        public HashSet<string> Tags;

        public MyTask(string title)
        {
            Title = title;
            Tags = new HashSet<string>();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            MyTask t1 = new MyTask("Зробити лабу з ООП");
            t1.Tags.Add("навчання");
            t1.Tags.Add("програмування");
            t1.Tags.Add("навчання");

            MyTask t2 = new MyTask("Сходити в магазин");
            t2.Tags.Add("покупки");
            t2.Tags.Add("їжа");

            MyTask t3 = new MyTask("Поприбирати");
            t3.Tags.Add("дім");

            Console.WriteLine("Завдання: " + t1.Title);
            Console.WriteLine("Теги: " + string.Join(", ", t1.Tags));
            Console.WriteLine("Чи є тег 'навчання'? " + t1.Tags.Contains("навчання"));
            Console.WriteLine();

            Queue<MyTask> queue = new Queue<MyTask>();
            queue.Enqueue(t1);
            queue.Enqueue(t2);
            queue.Enqueue(t3);

            Console.WriteLine("Завдань у черзі: " + queue.Count);
            Console.WriteLine("Перше в черзі: " + queue.Peek().Title);
            Console.WriteLine();

            Stack<MyTask> history = new Stack<MyTask>();

            Console.WriteLine("Виконання завдань");
            while (queue.Count > 0)
            {
                MyTask current = queue.Dequeue();
                Console.WriteLine("Виконую: " + current.Title);
                
                history.Push(current);
            }

            Console.WriteLine("\nСтан після виконання:");
            Console.WriteLine("В черзі: " + queue.Count);
            Console.WriteLine("В історії (стеку): " + history.Count);
            Console.WriteLine();

            Console.WriteLine("Скасування останньої дії (Undo)");
            if (history.Count > 0)
            {
                MyTask undoneTask = history.Pop();
                Console.WriteLine("Скасовано: " + undoneTask.Title);
                
                queue.Enqueue(undoneTask);
            }

            Console.WriteLine("\nФінальний стан:");
            Console.WriteLine("В історії залишилось: " + history.Count);
            Console.WriteLine("В чергу повернулось: " + queue.Count);
            
            Console.ReadLine();
        }
    }
}
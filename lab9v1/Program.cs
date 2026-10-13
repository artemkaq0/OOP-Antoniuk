using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Lab9V1
{
    public class NotificationException : Exception
    {
        public NotificationException(string message) : base(message) { }
    }

    public abstract class Notification
    {
        public string Recipient { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        public abstract void Validate();

        public abstract void Send();
    }

    public class EmailNotification : Notification
    {
        public string Subject { get; set; } = string.Empty;

        public override void Validate()
        {
            if (string.IsNullOrWhiteSpace(Recipient))
                throw new NotificationException("Email отримувача не може бути порожнім.");

            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(Recipient, emailPattern))
                throw new NotificationException($"Некоректний Email формат: '{Recipient}'");

            if (string.IsNullOrWhiteSpace(Message))
                throw new NotificationException("Текст Email-повідомлення порожній.");
        }

        public override void Send()
        {
            Validate();
            Console.WriteLine($"[EMAIL] Тема: '{Subject}' | Отримувач: {Recipient} | Повідомлення: \"{Message}\"");
        }
    }

    public class SMSNotification : Notification
    {
        public override void Validate()
        {
            if (string.IsNullOrWhiteSpace(Recipient))
                throw new NotificationException("Номер телефону не може бути порожнім.");

            string phonePattern = @"^\+?[0-9]{10,14}$";
            if (!Regex.IsMatch(Recipient, phonePattern))
                throw new NotificationException($"Некоректний номер телефону: '{Recipient}' (очікується формат на зразок +380501234567)");

            if (string.IsNullOrWhiteSpace(Message))
                throw new NotificationException("Текст SMS порожній.");

            if (Message.Length > 160)
                throw new NotificationException($"Текст SMS занадто довгий ({Message.Length} симв. Максимум: 160).");
        }

        public override void Send()
        {
            Validate();
            Console.WriteLine($"[SMS] Номер: {Recipient} | Повідомлення: \"{Message}\"");
        }
    }

    public class PushNotification : Notification
    {
        public string DeviceId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;

        public override void Validate()
        {
            if (string.IsNullOrWhiteSpace(DeviceId))
                throw new NotificationException("Device ID пристрою не може бути порожнім.");

            if (DeviceId.Length < 8)
                throw new NotificationException($"Некоректний Device ID: '{DeviceId}' (має бути не менше 8 символів)");

            if (string.IsNullOrWhiteSpace(Message))
                throw new NotificationException("Текст Push-сповіщення порожній.");
        }

        public override void Send()
        {
            Validate();
            Console.WriteLine($"[PUSH] Пристрій ID: {DeviceId} | Заголовок: '{Title}' | Повідомлення: \"{Message}\"");
        }
    }

    public class NotificationService
    {
        public void SendAll(List<Notification> notifications)
        {
            Console.WriteLine("       ЗАПУСК СЕРВІСУ ВІДПРАВКИ СПОВІЩЕНЬ         ");

            int successCount = 0;
            int errorCount = 0;

            for (int i = 0; i < notifications.Count; i++)
            {
                var notification = notifications[i];
                Console.WriteLine($"[{i + 1}] Канал: {notification.GetType().Name}");

                try
                {
                    notification.Send();

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("    Статус: УСПІШНО ВІДПРАВЛЕНО");
                    Console.ResetColor();
                    successCount++;
                }
                catch (NotificationException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"   ! ПОМИЛКА ВАЛІДАЦІЇ: {ex.Message}");
                    Console.ResetColor();
                    errorCount++;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine($"   ! КРИТИЧНА ПОМИЛКА: {ex.Message}");
                    Console.ResetColor();
                    errorCount++;
                }

                Console.WriteLine(new string('-', 50));
            }

            Console.WriteLine($"ПІДСУМОК: Успішно: {successCount} | З помилками: {errorCount}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            List<Notification> notifications = new List<Notification>
            {
                new EmailNotification
                {
                    Recipient = "student@college.edu.ua",
                    Subject = "Оцінка за Лабораторну №9",
                    Message = "Вашу роботу перевірено і зараховано!"
                },

                new SMSNotification
                {
                    Recipient = "+380501234567",
                    Message = "Код авторизації: 8842"
                },

                new PushNotification
                {
                    DeviceId = "DEV-ANDROID-98123",
                    Title = "Нове повідомлення",
                    Message = "У вас 1 нове сповіщення у додатку."
                },

                new EmailNotification
                {
                    Recipient = "user-without-at-sign",
                    Subject = "Тест",
                    Message = "Привіт!"
                },

                new SMSNotification
                {
                    Recipient = "123",
                    Message = "Тестовий SMS"
                },

                new PushNotification
                {
                    DeviceId = "ID123",
                    Title = "Тест",
                    Message = "Помилка пристрою"
                }
            };

            NotificationService service = new NotificationService();
            service.SendAll(notifications);

            Console.WriteLine("\nНатисніть Enter для завершення");
            Console.ReadLine();
        }
    }
}
// Подключаем необходимые пространства имен
using System;
using System.Diagnostics; // Для работы с процессами
using System.IO; // Для работы с файловой системой (логирование)
using System.Linq; // Для сортировки процессов

class Program
{
    // Точка входа в программу
    static void Main(string[] args)
    {
        // Бесконечный цикл для отображения меню
        while (true)
        {
            // Очищаем консоль для обновления информации
            Console.Clear();

            // Выводим меню для пользователя
            Console.WriteLine("1. Показать запущенные процессы");
            Console.WriteLine("2. Завершить процесс по ID");
            Console.WriteLine("3. Запустить новый процесс");
            Console.WriteLine("4. Выйти");

            // Получаем выбор пользователя
            Console.Write("Выберите действие: ");
            string choice = Console.ReadLine();

            // В зависимости от выбора пользователя выполняем соответствующее действие
            switch (choice)
            {
                case "1":
                    // Показать список процессов
                    ShowProcesses();
                    break;
                case "2":
                    // Завершить процесс по ID
                    EndProcess();
                    break;
                case "3":
                    // Запустить новый процесс
                    StartNewProcess();
                    break;
                case "4":
                    // Выход из программы
                    return;
                default:
                    // Сообщение о неверном выборе
                    Console.WriteLine("Неверный выбор, попробуйте снова.");
                    break;
            }

            // Ожидание нажатия клавиши для продолжения
            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }
    }

    // Метод для отображения списка запущенных процессов
    static void ShowProcesses()
    {
        // Получаем список всех процессов
        var processes = Process.GetProcesses();

        // Заголовок таблицы процессов
        Console.WriteLine("{0,-40} {1,-10} {2,10} {3,15}", "Имя процесса", "ID", "Память (MB)", "Статус");

        // Перебираем каждый процесс и выводим информацию о нём
        foreach (var process in processes.OrderBy(p => p.ProcessName)) // Сортируем по имени процесса
        {
            // Определяем статус процесса (запущен или не отвечает)
            string status = process.Responding ? "Запущен" : "Не отвечает";

            // Выводим информацию о процессе: имя, ID, используемая память, статус
            Console.WriteLine("{0,-40} {1,-10} {2,10:N2} {3,15}",
                process.ProcessName, process.Id, process.WorkingSet64 / 1024.0 / 1024.0, status); // Память в мегабайтах
        }
    }

    // Метод для завершения процесса по ID
    static void EndProcess()
    {
        // Просим пользователя ввести ID процесса для завершения
        Console.Write("Введите ID процесса для завершения: ");

        // Проверяем, корректен ли ввод
        if (int.TryParse(Console.ReadLine(), out int processId))
        {
            try
            {
                // Получаем процесс по ID
                var process = Process.GetProcessById(processId);

                // Завершаем процесс
                process.Kill();

                // Логируем действие
                LogAction($"Процесс {process.ProcessName} (ID: {processId}) был завершен.");

                // Уведомляем пользователя об успешном завершении
                Console.WriteLine("Процесс успешно завершен.");
            }
            catch (Exception ex)
            {
                // В случае ошибки выводим сообщение
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
        else
        {
            // Если ID введён некорректно, выводим сообщение об ошибке
            Console.WriteLine("Некорректный ID.");
        }
    }
    // Метод для запуска нового процесса
    static void StartNewProcess()
    {
        // Просим пользователя ввести имя программы или путь к исполняемому файлу
        Console.Write("Введите имя программы или путь для запуска: ");
        string processName = Console.ReadLine();

        try
        {
            // Пытаемся запустить процесс
            Process.Start(processName);

            // Логируем действие
            LogAction($"Процесс {processName} был запущен.");

            // Уведомляем пользователя об успешном запуске
            Console.WriteLine("Процесс успешно запущен.");
        }
        catch (Exception ex)
        {
            // В случае ошибки выводим сообщение
            Console.WriteLine($"Ошибка при запуске процесса: {ex.Message}");
        }
    }

    // Метод для логирования действий (запуск и завершение процессов)
    static void LogAction(string action)
    {
        // Путь к файлу для логов
        string logPath = "process_log.txt";

        // Открываем файл для записи (если файла нет, он создается)
        using (StreamWriter writer = new StreamWriter(logPath, true))
        {
            // Записываем в файл текущее время и действие
            writer.WriteLine($"{DateTime.Now}: {action}");
        }
    }
}

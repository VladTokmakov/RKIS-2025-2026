using System.Text.RegularExpressions;

namespace Todolist
{
    class Program
    {
        private static Person user;
        private static string[] todos = new string[2];
        private static bool[] statuses = new bool[2];
        private static DateTime[] dates = new DateTime[2];
        private static int taskCount = 0;

        static void Main(string[] args)
        {
            Console.WriteLine("Работу выполнили Токмаков и Неофидис");
            SetDataUser();

            while (true)
            {
                Console.Write("Введите команду: ");
                string fullInput = Console.ReadLine().Trim();
                string[] partInput = fullInput.Split(' ', 2);
                string command = partInput[0].ToLower();
                switch (command)
                {
                    case "help":
                        Help();
                        break;

                    case "profile":
                        Profile();
                        break;

                    case "add_user":
                        SetDataUser();
                        break;

                    case "add":
                        if (partInput.Length > 1)
                        {
                            AddTask(partInput[1]);
                        }
                        else
                        {
                            Console.WriteLine("Неправильный формат, должен быть: add \"текст задачи\" или add текст задачи");
                        }
                        break;

                    
                    case "read":
                        if (partInput.Length > 1)
                        {
                            ReadTask(partInput[1]);
                        }
                        else
                        {
                            Console.WriteLine("Неправильный формат, должен быть: read номер_задачи");
                        }
                        break;

                    case "view":
                        string flags;
                        if (partInput.Length > 1)
                        {
                            flags = partInput[1];
                        }
                        else
                        {
                            flags = "";
                        }
                        TasksView(flags);
                        break;

                    case "done":
                        if (partInput.Length > 1)
                        {
                            MarkTaskDone(partInput[1]);
                        }
                        else
                        {
                            Console.WriteLine("Неправильный формат, должен быть: done номер_задачи");
                        }
                        break;

                    case "delete":
                        if (partInput.Length > 1)
                        {
                            DeleteTask(partInput[1]);
                        }
                        else
                        {
                            Console.WriteLine("Неправильный формат, должен быть: delete номер_задачи");
                        }
                        break;

                    case "update":
                        if (partInput.Length > 1)
                        {
                            UpdateTask(partInput[1]);
                        }
                        else
                        {
                            Console.WriteLine("Неправильный формат, должен быть: update номер_задачи \"новый_текст\" или update номер_задачи новый_текст");
                        }
                        break;

                    case "exit":
                        return;

                    default:
                        Console.WriteLine($"Неизвестная команда: {command}");
                        Console.WriteLine("Введите 'help' для просмотра доступных команд");
                        break;
                }
            }
        }
        static void MarkTaskDone(string taskID)
        {
            if (Regex.IsMatch(taskID, @"^\d+$"))
            {
                int taskNumber = int.Parse(taskID);
                if (taskNumber > 0 && taskNumber <= taskCount)
                {
                    int taskIndex = taskNumber - 1;
                    statuses[taskIndex] = true;
                    dates[taskIndex] = DateTime.Now;
                    Console.WriteLine($"Задача №{taskNumber} отмечена как выполненная");
                }
                else
                {
                    Console.WriteLine($"Неверный номер задачи. Должен быть от 1 до {taskCount}");
                }

            }
            else
            {
                Console.WriteLine("Номер задачи должен быть числом");
            }
        }

        static void DeleteTask(string taskID)
        {
            if (Regex.IsMatch(taskID, @"^\d+$"))
            {
                int taskNumber = int.Parse(taskID);
                if (taskNumber > 0 && taskNumber <= taskCount)
                {
                    int taskIndex = taskNumber - 1;
                    string deletedTask = todos[taskIndex];
                    
                    for (int i = taskIndex; i < taskCount - 1; i++)
                    {
                        todos[i] = todos[i + 1];
                        statuses[i] = statuses[i + 1];
                        dates[i] = dates[i + 1];
                    }
                    
                    todos[taskCount - 1] = null;
                    statuses[taskCount - 1] = false;
                    dates[taskCount - 1] = DateTime.MinValue;
                    
                    taskCount--;
                    Console.WriteLine($"Задача №{taskNumber} '{deletedTask}' удалена");
                }
                else
                {
                    Console.WriteLine($"Неверный номер задачи. Должен быть от 1 до {taskCount}");
                }
            }
            else
            {
                Console.WriteLine("Номер задачи должен быть числом");
            }
        }

        static void UpdateTask(string taskID)
        {
            string[] parts = taskID.Split(' ', 2);
            
            if (parts.Length < 2)
            {
                Console.WriteLine("Не указан новый текст задачи");
                return;
            }

            if (Regex.IsMatch(parts[0], @"^\d+$"))
            {
                int taskNumber = int.Parse(parts[0]);
                if (taskNumber > 0 && taskNumber <= taskCount)
                {
                    int taskIndex = taskNumber - 1;
                    string newText = parts[1];
                    
                    if (newText.StartsWith("\"") && newText.EndsWith("\""))
                    {
                        newText = newText.Substring(1, newText.Length - 2);
                    }
                    
                    string oldText = todos[taskIndex];
                    todos[taskIndex] = newText;
                    dates[taskIndex] = DateTime.Now;
                    
                    Console.WriteLine($"Обновил задачу: \nБыло: Задача №{taskNumber} \"{oldText}\" \nСтало: Задача №{taskNumber} \"{newText}\"");
                }
                else
                {
                    Console.WriteLine($"Неверный номер задачи. Должен быть от 1 до {taskCount}");
                }
            }
            else
            {
                Console.WriteLine("Номер задачи должен быть числом");
            }
        }


        static void SetDataUser()
        {
            Console.Write("Введите ваше имя: ");
            string firstName = Console.ReadLine();

            Console.Write("Введите вашу фамилию: ");
            string lastName = Console.ReadLine();

            Console.Write("Введите ваш год рождения: ");
            int yearBirth = int.Parse(Console.ReadLine());

            int age = DateTime.Now.Year - yearBirth;

            user = new Person(firstName, lastName, yearBirth, age);

            Console.WriteLine($"Добавлен пользователь {firstName} {lastName}, Год рождения: {yearBirth}, возраст – {age}");
            Console.WriteLine();

        }
        static void Help()
        {
            Console.WriteLine("Доступные команды:");
            Console.WriteLine("help    - выводит список всех доступных команд с кратким описанием");
            Console.WriteLine("profile - выводит данные пользователя");
            Console.WriteLine("add     - добавляет новую задачу");
            Console.WriteLine("read    - полный просмотр задачи");
            Console.WriteLine("view    - выводит все задачи из массива");
            Console.WriteLine("done    - отмечает задачу выполненной");
            Console.WriteLine("delete  - удаляет задачу по индексу");
            Console.WriteLine("update  - обновляет текст задачи");
            Console.WriteLine("exit    - завершает программу");
            Console.WriteLine();
            Console.WriteLine("Флаги для команды 'view':");
            Console.WriteLine("  -i, --index       - показывать индекс задачи");
            Console.WriteLine("  -s, --status      - показывать статус задачи");
            Console.WriteLine("  -d, --update-date - показывать дату изменения");
            Console.WriteLine("  -a, --all         - показывать все данные");
        }

        static void Profile()
        {
            if (user != null)
            {
                Console.WriteLine($"Пользователь: {user.FirstName} {user.LastName}");
                Console.WriteLine($"Год рождения: {user.YearBirth}");
                Console.WriteLine($"Возраст: {user.Age}");
            }
            else
            {
                Console.WriteLine("Данные пользователя не найдены");
            }
        }

        static void AddTask(string taskID)
        {
            if (taskID == "--multiline" || taskID == "-m")
            {
                Console.WriteLine("Многострочный режим. Введите задачи (для завершения введите 'lend'):");
                string multilineText = "";

                string line = Console.ReadLine();
                while (line.ToLower() != "!end")
                {
                    multilineText += line + "\n";
                    line = Console.ReadLine();
                }

                if (taskCount >= todos.Length)
                {
                    ExpandAllArrays();
                }

                todos[taskCount] = multilineText.Trim();
                statuses[taskCount] = false;
                dates[taskCount] = DateTime.Now;
                taskCount++;
                Console.WriteLine($"Добавлена задача №{taskCount}: {multilineText.Trim()}");
            }
            else
            {
                if (taskID.StartsWith("\"") && taskID.EndsWith("\""))
                {
                    taskID = taskID.Substring(1, taskID.Length - 2);
                }

                if (taskCount >= todos.Length)
                {
                    ExpandAllArrays();
                }

                todos[taskCount] = taskID;
                statuses[taskCount] = false;
                dates[taskCount] = DateTime.Now;
                taskCount++;
                Console.WriteLine($"Добавлена задача №{taskCount}: {taskID}");
            }
        }

        static void ExpandAllArrays()
        {
            int newSize = todos.Length * 2;

            string[] newTodos = new string[newSize];
            bool[] newStatuses = new bool[newSize];
            DateTime[] newDates = new DateTime[newSize];

            for (int i = 0; i < todos.Length; i++)
            {
                newTodos[i] = todos[i];
                newStatuses[i] = statuses[i];
                newDates[i] = dates[i];
            }

            todos = newTodos;
            statuses = newStatuses;
            dates = newDates;

            Console.WriteLine($"Массивы увеличены до {newSize} элементов");
        }


        static void ReadTask(string taskID)
        {
            if (Regex.IsMatch(taskID, @"^\d+$"))
            {
                int taskNumber = int.Parse(taskID);
                if (taskNumber > 0 && taskNumber <= taskCount)
                {
                    int taskIndex = taskNumber - 1;
                    Console.WriteLine($"Задача №{taskNumber}:");
                    Console.WriteLine($"Текст: {todos[taskIndex]}");
                    Console.WriteLine($"Статус: {(statuses[taskIndex] ? "Выполнена" : "Не выполнена")}");
                    Console.WriteLine($"Дата изменения: {dates[taskIndex]}");
                }
                else
                {
                    Console.WriteLine($"Неверный номер задачи. Должен быть от 1 до {taskCount}");
                }
            }
            else
            {
                Console.WriteLine("Номер задачи должен быть числом");
            }
        }

        static void TasksView(string flags)
        {
            if (taskCount == 0)
            {
                Console.WriteLine("Задачи отсутствуют");
                return;
            }

            bool showIndex = flags.Contains("-i") || flags.Contains("--index");
            bool showStatus = flags.Contains("-s") || flags.Contains("--status");
            bool showDate = flags.Contains("-d") || flags.Contains("--update-date");
            bool showAll = flags.Contains("-a") || flags.Contains("--all");

            if (showAll) showIndex = showStatus = showDate = true;

            string header = "";
            if (showIndex) header += "№       ";
            if (showStatus) header += "Статус     ";
            header += "Задача                            ";
            if (showDate) header += "Дата изменения  ";

            Console.WriteLine(header);
            Console.WriteLine(new string('-', header.Length));

            for (int i = 0; i < taskCount; i++)
            {
                string row = "";

                if (showIndex) row += $"{i + 1}       ".Substring(0, 8);

                if (showStatus)
                {
                    string statusText = statuses[i] ? "Сделано    " : "Не сделано ";
                    row += statusText;
                }

                string taskText = todos[i];
                if (taskText.Length > 34)
                    taskText = taskText.Substring(0, 30) + "... ";
                row += taskText + new string(' ', 34 - taskText.Length);

                if (showDate) row += $"{dates[i]} ";

                Console.WriteLine(row);
            }
        }

    }

    class Person
    {
        public string FirstName { get; }
        public string LastName { get; }
        public int YearBirth { get; }
        public int Age { get; }

        public Person(string firstName, string lastName, int yearBirth, int age)
        {
            FirstName = firstName;
            LastName = lastName;
            YearBirth = yearBirth;
            Age = age;
        }

    }
}
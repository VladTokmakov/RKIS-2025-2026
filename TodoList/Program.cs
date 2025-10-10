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

                    case "view":
                        TasksView();
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
        static void MarkTaskDone(string taskText)
        {
            if (Regex.IsMatch(taskText, @"^\d+$"))
            {
                int taskNumber = int.Parse(taskText);
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

        static void DeleteTask(string taskText)
        {
            if (Regex.IsMatch(taskText, @"^\d+$"))
            {
                int taskNumber = int.Parse(taskText);
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

        static void UpdateTask(string taskText)
        {
            string[] parts = taskText.Split(' ', 2);
            
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
            Console.WriteLine("view    - выводит все задачи из массива");
            Console.WriteLine("exit    - завершает программу");
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

        static void AddTask(string taskText)
        {
            if (taskText.StartsWith("\"") && taskText.EndsWith("\""))
            {
                taskText = taskText.Substring(1, taskText.Length - 2);
            }

            if (taskCount >= todos.Length)
            {
                ExpandAllArrays();
            }

            todos[taskCount] = taskText;
            statuses[taskCount] = false;
            dates[taskCount] = DateTime.Now;
            taskCount++;
            Console.WriteLine($"Добавлена задача №{taskCount}: {taskText}");
        }

        static void ExpandAllArrays()
        {
            int newSize = todos.Length * 2;

            string[] newTodos = new string[newSize];
            for (int i = 0; i < todos.Length; i++)
            {
                newTodos[i] = todos[i];
            }
            todos = newTodos;

            bool[] newStatuses = new bool[newSize];
            for (int i = 0; i < statuses.Length; i++)
            {
                newStatuses[i] = statuses[i];
            }
            statuses = newStatuses;

            DateTime[] newDates = new DateTime[newSize];
            for (int i = 0; i < dates.Length; i++)
            {
                newDates[i] = dates[i];
            }
            dates = newDates;

            Console.WriteLine($"Массивы увеличены до {newSize} элементов");
        }

        static void TasksView()
        {
            if (taskCount == 0)
            {
                Console.WriteLine("Задачи отсутствуют");
            }
            else
            {
                Console.WriteLine("Список задач:");
                for (int i = 0; i < taskCount; i++)
                {
                    string statusText = statuses[i] ? "Сделано" : "Не сделано";
                    string dateText = dates[i].ToString();
                    Console.WriteLine($"{i + 1}. {todos[i]} [{statusText}] {dateText}");
                }
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
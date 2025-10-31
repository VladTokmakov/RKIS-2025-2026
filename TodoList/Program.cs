using System.Text.RegularExpressions;

namespace Todolist
{
    class Program
    {
        private static Profile user;
        private static Todolist todoList = new Todolist();

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
                        if (partInput.Length > 1) AddTask(partInput[1]);
                        else Console.WriteLine("Неправильный формат, должен быть: add \"текст задачи\" или add текст задачи");
                        break;

                    
                    case "read":
                        if (partInput.Length > 1) ReadTask(partInput[1]);
                        else Console.WriteLine("Неправильный формат, должен быть: read номер_задачи");
                        break;

                    case "view":
                        string flags;
                        if (partInput.Length > 1) flags = partInput[1];
                        else flags = "";
                        TasksView(flags);
                        break;

                    case "done":
                        if (partInput.Length > 1) TaskID(partInput[1]);
                        else Console.WriteLine("Неправильный формат, должен быть: done номер_задачи");
                        break;

                    case "delete":
                        if (partInput.Length > 1) DeleteTask(partInput[1]);
                        else Console.WriteLine("Неправильный формат, должен быть: delete номер_задачи");
                        break;

                    case "update":
                        if (partInput.Length > 1) UpdateTask(partInput[1]);
                        else Console.WriteLine("Неправильный формат, должен быть: update номер_задачи \"новый_текст\" или update номер_задачи новый_текст");
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
        static void TaskID(string taskText)
        {
            if (ValidationNumber(taskText, out TodoItem item, out int taskNumber))
            {
                item.MarkDone();
                Console.WriteLine($"Задача №{taskNumber} отмечена как выполненная");
            }
        }

        static void DeleteTask(string taskText)
        {
            if (ValidationNumber(taskText, out TodoItem item, out int taskNumber))
            {
                todoList.Delete(taskNumber - 1);
                Console.WriteLine($"Задача №{taskNumber} '{item.Text}' удалена");
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

            if (ValidationNumber(parts[0], out TodoItem item, out int taskNumber))
            {
                string newText = parts[1];

                if (newText.StartsWith("\"") && newText.EndsWith("\""))
                {
                    newText = newText.Substring(1, newText.Length - 2);
                }

                string oldText = item.Text;
                item.UpdateText(newText);

                Console.WriteLine($"Обновил задачу: \nБыло: Задача №{taskNumber} \"{oldText}\" \nСтало: Задача №{taskNumber} \"{newText}\"");
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

            user = new Profile(firstName, lastName, yearBirth);

            Console.WriteLine($"Добавлен пользователь: {user.GetInfo()}");
            Console.WriteLine();

        }
        static void Help()
        {
            Console.WriteLine("""
            Доступные команды:
            help    - выводит список всех доступных команд с кратким описанием
            profile - выводит данные пользователя
            add     - добавляет новую задачу
            read    - полный просмотр задачи
            view    - выводит все задачи из массива
            done    - отмечает задачу выполненной
            delete  - удаляет задачу по индексу
            update  - обновляет текст задачи
            exit    - завершает программу

            Флаги для команды 'view':
            -i, --index       - показывать индекс задачи
            -s, --status      - показывать статус задачи
            -d, --update-date - показывать дату изменения
            -a, --all         - показывать все данные
            """);
        }

        static void Profile()
        {
            if (user != null)
            {
                Console.WriteLine(user.GetInfo());
            }
            else
            {
                Console.WriteLine("Данные пользователя не найдены");
            }
        }

        static void AddTask(string taskText)
        {
            if (taskText == "--multiline" || taskText == "-m")
            {
                Console.WriteLine("Многострочный режим. Введите задачи (для завершения введите '!end'):");
                string multilineText = "";

                Console.Write("> ");
                string line = Console.ReadLine();
                while (line.ToLower() != "!end")
                {
                    Console.Write("> ");
                    multilineText += line + "\n";
                    line = Console.ReadLine();
                }

                TodoItem newItem = new TodoItem(multilineText.Trim());
                todoList.Add(newItem);
                Console.WriteLine($"Добавлена задача №{todoList.GetCount()}: {multilineText.Trim()}");
            }
            else
            {
                if (taskText.StartsWith("\"") && taskText.EndsWith("\""))
                {
                    taskText = taskText.Substring(1, taskText.Length - 2);
                }

                TodoItem newItem = new TodoItem(taskText);
                todoList.Add(newItem);
                Console.WriteLine($"Добавлена задача №{todoList.GetCount()}: {taskText}");
            }
        }

        static void ReadTask(string taskText)
        {
            if (ValidationNumber(taskText, out TodoItem item, out int taskNumber))
            {
                Console.WriteLine(item.GetFullInfo());
            }
        }

        static void TasksView(string flags)
        {
            if (todoList.GetCount() == 0)
            {
                Console.WriteLine("Задачи отсутствуют");
                return;
            }
            bool showIndex = flags.Contains("-i") || flags.Contains("--index") || flags.Contains("i");
            bool showStatus = flags.Contains("-s") || flags.Contains("--status") || flags.Contains("s");
            bool showDate = flags.Contains("-d") || flags.Contains("--update-date") || flags.Contains("d");
            bool showAll = flags.Contains("-a") || flags.Contains("--all");

            if (showAll) showIndex = showStatus = showDate = true;

            todoList.View(showIndex, showStatus, showDate);
        }

        private static bool ValidationNumber(string taskText, out TodoItem item, out int taskNumber)
        {
            item = null;
            taskNumber = 0;
            
            if (!Regex.IsMatch(taskText, @"^\d+$"))
            {
                Console.WriteLine("Номер задачи должен быть числом");
                return false;
            }
            
            taskNumber = int.Parse(taskText);
            if (taskNumber <= 0 || taskNumber > todoList.GetCount())
            {
                Console.WriteLine($"Неверный номер задачи. Должен быть от 1 до {todoList.GetCount()}");
                return false;
            }
            
            item = todoList.GetItem(taskNumber - 1);
            return true;
        }
    }
}
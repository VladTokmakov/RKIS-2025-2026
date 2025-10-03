namespace Todolist
{
    class Program
    {
        private static Person user;
        private static string[] todos = new string[2];
        private static int taskCount = 0;

        static void Main(string[] args)
        {
            Console.WriteLine("Работу выполнили Токмаков и Неофидис");

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

                    case "adduser":
                        GetDataUser();
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

                    case "exit":
                        return;
                }

            }
        }
        static void GetDataUser()
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
                string[] newTodos = new string[todos.Length * 2];

                for (int i = 0; i < todos.Length; i++)
                {
                    newTodos[i] = todos[i];
                }

                todos = newTodos;
                Console.WriteLine($"Массив todos увеличен до {todos.Length} элементов");
            }

            todos[taskCount] = taskText;
            taskCount++;
            Console.WriteLine($"Добавлена задача №{taskCount}: {taskText}");
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
                    Console.WriteLine($"{i + 1}. {todos[i]}");
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
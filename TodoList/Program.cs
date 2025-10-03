namespace Todolist
{
    class Program
    {
        private static Person user;
 
        static void Main(string[] args)
        {
            Console.WriteLine("Работу выполнили Токмаков и Неофидис");

            //GetDataUser();

            string[] todos = new string[10];

            while (true)
            {
                Console.Write("Введите команду: ");
                string command = Console.ReadLine().Trim().ToLower();

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
                        break;

                    case "view":
                        break;

                    case "exit":
                        break;
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
            //Console.ReadLine();
        }
        static void Help()
        {
            Console.WriteLine("Доступные команды:");
            Console.WriteLine("help    - выводит список всех доступных команд с кратким описанием");
            Console.WriteLine("profile - выводит данные пользователя");
            Console.WriteLine("add     - добавляет новую задачу");
            Console.WriteLine("view    - выводит все задачи из массива");
            Console.WriteLine("exit    - завершает программу");
            Console.ReadLine();
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
            Console.ReadLine();
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
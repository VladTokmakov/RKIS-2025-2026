namespace Todolist
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Работу выполнили Токмаков и Неофидис");

            /*Console.Write("Введите ваше имя: ");
            string firstName = Console.ReadLine();

            Console.Write("Введите вашу фамилию: ");
            string lastName = Console.ReadLine();

            Console.Write("Введите ваш год рождения: ");
            string yearBirthString = Console.ReadLine();

            int yearBirth = int.Parse(yearBirthString);

            int age = DateTime.Now.Year - yearBirth;

            Console.WriteLine($"Добавлен пользователь {firstName} {lastName}, возраст – {age}");
            */

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

    }
}
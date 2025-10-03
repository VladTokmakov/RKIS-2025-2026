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
                string command = Console.ReadLine().Trim();

                switch (command)
                {
                    case "help":
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
    }
}
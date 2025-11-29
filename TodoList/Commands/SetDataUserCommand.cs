namespace Todolist
{
    public class SetDataUserCommand : ICommand
    {
        public Profile User { get; private set; }
        private readonly string ProfileFilePath;

        public SetDataUserCommand(string profileFilePath = null)
        {
            ProfileFilePath = profileFilePath;
        }

        public void Execute()
        {
            Console.Write("Введите ваше имя: ");
            string firstName = Console.ReadLine();

            Console.Write("Введите вашу фамилию: ");
            string lastName = Console.ReadLine();

            Console.Write("Введите ваш год рождения: ");
            int yearBirth = int.Parse(Console.ReadLine());

            User = new Profile(firstName, lastName, yearBirth);

            Console.WriteLine($"Добавлен пользователь: {User.GetInfo()}");
            Console.WriteLine();

            if (!string.IsNullOrEmpty(ProfileFilePath)) FileManager.SaveProfile(User, ProfileFilePath);
        }

        public void Unexecute()
        {
        }
    }
}
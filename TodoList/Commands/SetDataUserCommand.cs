using System;

namespace Todolist
{
    public class SetDataUserCommand : IUndo
    {
        public Profile User { get; private set; }
        private readonly string ProfileFilePath;
        private Profile _oldProfile;

        public SetDataUserCommand(string profileFilePath = null)
        {
            ProfileFilePath = profileFilePath;
        }

        public void Execute()
        {
            _oldProfile = AppInfo.CurrentProfile;
            Console.Write("Введите ваше имя: ");
            string firstName = Console.ReadLine();
            Console.Write("Введите вашу фамилию: ");
            string lastName = Console.ReadLine();
            Console.Write("Введите ваш год рождения: ");
            if (!int.TryParse(Console.ReadLine(), out int yearBirth))
            {
                Console.WriteLine("Неверный формат года рождения");
                return;
            }

            User = new Profile(firstName, lastName, yearBirth);
            AppInfo.CurrentProfile = User;
            Console.WriteLine($"Добавлен пользователь: {User.GetInfo()}");
            Console.WriteLine();

            if (!string.IsNullOrEmpty(ProfileFilePath)) 
            {
                FileManager.SaveProfile(User, ProfileFilePath);
            }

            AppInfo.UndoStack.Push(this);
            AppInfo.RedoStack.Clear();
        }

        public void Unexecute()
        {
            AppInfo.CurrentProfile = _oldProfile;
            
            if (_oldProfile != null && !string.IsNullOrEmpty(ProfileFilePath))
            {
                FileManager.SaveProfile(_oldProfile, ProfileFilePath);
            }
            
            Console.WriteLine("Создание профиля отменено");
        }
    }
}
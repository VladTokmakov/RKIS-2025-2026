using System;
using Todolist.Exceptions;

namespace Todolist
{
    public class SetDataUserCommand : IUndo
    {
        public Profile User { get; private set; }

        private readonly string ProfileFilePath;
        private readonly IDataStorage _storage;
        private Profile _oldProfile;

        public SetDataUserCommand(string profileFilePath = null, IDataStorage storage = null)
        {
            ProfileFilePath = profileFilePath;
            _storage = storage;
        }

        public void Execute()
        {
            try
            {
                _oldProfile = AppInfo.CurrentProfile;

                Console.Write("Введите ваше имя: ");
                string firstName = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(firstName))
                    throw new InvalidArgumentException("Имя не может быть пустым.");

                Console.Write("Введите вашу фамилию: ");
                string lastName = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(lastName))
                    throw new InvalidArgumentException("Фамилия не может быть пустой.");

                Console.Write("Введите ваш год рождения: ");
                string yearInput = Console.ReadLine();

                if (!int.TryParse(yearInput, out int yearBirth))
                    throw new InvalidArgumentException("Неверный формат года рождения");

                if (yearBirth < 1900 || yearBirth > DateTime.Now.Year)
                    throw new InvalidArgumentException($"Год рождения должен быть между 1900 и {DateTime.Now.Year}");

                User = new Profile(firstName, lastName, yearBirth);
                AppInfo.CurrentProfile = User;

                Console.WriteLine($"Добавлен пользователь: {User.GetInfo()}");
                Console.WriteLine();

                if (!string.IsNullOrEmpty(ProfileFilePath) && _storage != null)
                {
                    _storage.SaveProfile(User, ProfileFilePath);
                }

                AppInfo.UndoStack.Push(this);
                AppInfo.RedoStack.Clear();
            }
            catch (Exception ex) when (!(ex is InvalidArgumentException))
            {
                throw;
            }
        }

        public void Unexecute()
        {
            try
            {
                AppInfo.CurrentProfile = _oldProfile;

                if (_oldProfile != null && !string.IsNullOrEmpty(ProfileFilePath) && _storage != null)
                {
                    _storage.SaveProfile(_oldProfile, ProfileFilePath);
                }

                Console.WriteLine("Создание профиля отменено");
            }
            catch (Exception ex)
            {
                throw new BusinessLogicException($"Ошибка при отмене создания профиля: {ex.Message}");
            }
        }
    }
}
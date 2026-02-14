using System;
using System.IO;

namespace Todolist
{
    class Program
    {
        private static Profile user;
        private static Todolist todoList = new Todolist();
        private static string dataDirPath = Path.Combine(Directory.GetCurrentDirectory(), "data");
        private static string profileFilePath = Path.Combine(dataDirPath, "profile.txt");
        private static string todoFilePath = Path.Combine(dataDirPath, "todo.csv");

        static void Main(string[] args)
        {
            Console.WriteLine("Работу выполнили Токмаков и Неофидис");
            InitializeFileSystem();
            LoadData();

            while (true)
            {
                if (AppInfo.ShouldLogout)
                {
                    AppInfo.ShouldLogout = false;
                    LoadData();
                }

                Console.Write("Введите команду: ");
                string input = Console.ReadLine().Trim();

                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("Пустая команда.");
                    continue;
                }

                try
                {
                    ICommand command = CommandParser.Parse(input, todoList, user);
                    if (command != null)
                    {
                        command.Execute();
                        
                        if (command is SetDataUserCommand setUserCommand && setUserCommand.User != null)
                        {
                            user = setUserCommand.User;
                            AppInfo.CurrentProfile = user;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
            }
        }

        static void SetDataUserCommand()
        {
            var setDataUserCommand = new SetDataUserCommand(profileFilePath);
            setDataUserCommand.Execute();
            user = setDataUserCommand.User;
            AppInfo.CurrentProfile = user;
        }

        static void InitializeFileSystem()
        {
            FileManager.EnsureDataDirectory(dataDirPath);
            CommandParser.SetFilePaths(todoFilePath, profileFilePath);
        }

        static void LoadData()
        {
            user = FileManager.LoadProfile(profileFilePath);
            AppInfo.CurrentProfile = user;

            if (user != null)
            {
                Console.WriteLine($"Загружен профиль: {user.GetInfo()}");
            }
            else
            {
                SetDataUserCommand();
            }

            todoList = FileManager.LoadTodos(todoFilePath);
            AppInfo.Todos = todoList;
            Console.WriteLine($"Загружено задач: {todoList.GetCount()}");
        }
    }
}
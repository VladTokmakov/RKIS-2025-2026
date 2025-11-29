using System;
using System.IO;

namespace Todolist
{
    class Program
    {
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
                Console.Write("Введите команду: ");
                string input = Console.ReadLine().Trim();

                ICommand command = CommandParser.Parse(input, AppInfo.Todos, AppInfo.CurrentProfile);
                if (command != null)
                {
                    if (!(command is UndoCommand) && !(command is RedoCommand))
                    {
                        AppInfo.UndoStack.Push(command);
                        AppInfo.RedoStack.Clear();
                    }
                    command.Execute();
                    
                    if (command is SetDataUserCommand setUserCommand) 
                        AppInfo.CurrentProfile = setUserCommand.User;
                }
            }
        }

        static void SetDataUserCommand()
        {
            var setDataUserCommand = new SetDataUserCommand(profileFilePath); 
            setDataUserCommand.Execute();
            AppInfo.CurrentProfile = setDataUserCommand.User;
        }

        static void InitializeFileSystem()
        {
            FileManager.EnsureDataDirectory(dataDirPath);
            CommandParser.SetFilePaths(todoFilePath, profileFilePath);
        }

        static void LoadData()
        {
            AppInfo.CurrentProfile = FileManager.LoadProfile(profileFilePath);
            
            if (AppInfo.CurrentProfile != null)
            {
                Console.WriteLine($"Загружен профиль: {AppInfo.CurrentProfile.GetInfo()}");
            }
            else
            {
                SetDataUserCommand();
            }

            AppInfo.Todos = FileManager.LoadTodos(todoFilePath);
            Console.WriteLine($"Загружено задач: {AppInfo.Todos.GetCount()}");
        }
    }
}
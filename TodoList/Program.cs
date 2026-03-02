using System;
using System.IO;
using Todolist.Exceptions;

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
            try
            {
                Console.WriteLine("Работу выполнили Токмаков и Сайтамирова");
                
                InitializeFileSystem();
                LoadData();

                while (true)
                {
                    try
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
                    catch (AuthenticationException ex)
                    {
                        Console.WriteLine($"Ошибка авторизации: {ex.Message}");
                    }
                    catch (InvalidCommandException ex)
                    {
                        Console.WriteLine($"Ошибка команды: {ex.Message}");
                    }
                    catch (InvalidArgumentException ex)
                    {
                        Console.WriteLine($"Ошибка аргументов: {ex.Message}");
                    }
                    catch (TaskNotFoundException ex)
                    {
                        Console.WriteLine($"Ошибка задачи: {ex.Message}");
                    }
                    catch (ProfileNotFoundException ex)
                    {
                        Console.WriteLine($"Ошибка профиля: {ex.Message}");
                    }
                    catch (BusinessLogicException ex)
                    {
                        Console.WriteLine($"Ошибка бизнес-логики: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Неожиданная ошибка: {ex.Message}");
                        Console.WriteLine($"Стек вызовов: {ex.StackTrace}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Критическая ошибка при запуске: {ex.Message}");
                Console.WriteLine($"Стек вызовов: {ex.StackTrace}");
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
            try
            {
                FileManager.EnsureDataDirectory(dataDirPath);
                CommandParser.SetFilePaths(todoFilePath, profileFilePath);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new BusinessLogicException($"Нет прав доступа к файловой системе: {ex.Message}");
            }
            catch (IOException ex)
            {
                throw new BusinessLogicException($"Ошибка ввода-вывода при инициализации: {ex.Message}");
            }
        }

        static void LoadData()
        {
            try
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
            catch (Exception ex) when (ex is FormatException || ex is InvalidOperationException)
            {
                throw new BusinessLogicException($"Ошибка формата данных при загрузке: {ex.Message}");
            }
            catch (IOException ex)
            {
                throw new BusinessLogicException($"Ошибка чтения файлов: {ex.Message}");
            }
        }
    }
}
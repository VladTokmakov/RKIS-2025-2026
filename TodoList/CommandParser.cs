using System;
using System.Text.RegularExpressions;

namespace Todolist
{
    public static class CommandParser
    {
        private static string TodoFilePath;
        private static string ProfileFilePath;

        public static void SetFilePaths(string todoFilePath, string profileFilePath)
        {
            TodoFilePath = todoFilePath;
            ProfileFilePath = profileFilePath;
        }

        public static ICommand Parse(string input, Todolist todoList, Profile profile)
        {
            string[] parts = input.Split(' ', 2);
            string command = parts[0].ToLower();
            string arguments = parts.Length > 1 ? parts[1] : "";

            switch (command)
            {
                case "help":
                    return new HelpCommand();
                
                case "profile":
                    if (arguments == "--out" || arguments == "-o")
                    {
                        AppInfo.ShouldLogout = true;
                        AppInfo.CurrentProfile = null;
                        AppInfo.UndoStack.Clear();
                        AppInfo.RedoStack.Clear();
                        Console.WriteLine("Выход из профиля.");
                        return null;
                    }
                    return new ProfileCommand(profile);
                
                case "add":
                    if (arguments == "--multiline" || arguments == "-m") 
                        return new AddCommand(todoList, "", true, TodoFilePath);
                    else 
                        return new AddCommand(todoList, arguments, false, TodoFilePath);
                
                case "add_user":
                    return new SetDataUserCommand(ProfileFilePath);
                
                case "read":
                    if (ValidationNumber(arguments, todoList, out TodoItem readItem, out int readTaskNumber)) 
                        return new ReadCommand(todoList, readTaskNumber);
                    Console.WriteLine("Неправильный формат, должен быть: read номер_задачи");
                    return null;
                
                case "view":
                    bool showIndex = arguments.Contains("-i") || arguments.Contains("--index");
                    bool showStatus = arguments.Contains("-s") || arguments.Contains("--status");
                    bool showDate = arguments.Contains("-d") || arguments.Contains("--update-date");
                    bool showAll = arguments.Contains("-a") || arguments.Contains("--all");
                    return new ViewCommand(todoList, showIndex, showStatus, showDate, showAll);
                
                case "status":
                    string[] statusParts = arguments.Split(' ');
                    if (statusParts.Length == 2 && ValidationNumber(statusParts[0], todoList, out TodoItem statusItem, out int statusTaskNumber))
                    {
                        if (Enum.TryParse<TodoStatus>(statusParts[1], true, out TodoStatus status))
                        {
                            return new StatusCommand(todoList, statusTaskNumber, status, TodoFilePath);
                        }
                        else
                        {
                            Console.WriteLine($"Неизвестный статус: {statusParts[1]}");
                            Console.WriteLine("Доступные статусы: NotStarted, InProgress, Completed, Postponed, Failed");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Неправильный формат, должен быть: status номер_задачи статус");
                    }
                    return null;

                case "delete":
                    if (ValidationNumber(arguments, todoList, out TodoItem deleteItem, out int deleteTaskNumber)) 
                        return new DeleteCommand(todoList, deleteTaskNumber, TodoFilePath);
                    Console.WriteLine("Неправильный формат, должен быть: delete номер_задачи");
                    return null;

                case "update":
                    string[] updateParts = arguments.Split(' ', 2);
                    if (updateParts.Length == 2 && ValidationNumber(updateParts[0], todoList, out TodoItem updateItem, out int updateTaskNumber)) 
                        return new UpdateCommand(todoList, updateTaskNumber, updateParts[1], TodoFilePath);
                    Console.WriteLine("Неправильный формат, должен быть: update номер_задачи \"новый_текст\" или update номер_задачи новый_текст");
                    return null;

                case "undo":
                    return new UndoCommand();

                case "redo":
                    return new RedoCommand();

                case "search":
                    return ParseSearch(arguments);

                case "exit":
                    return new ExitCommand();

                default:
                    Console.WriteLine($"Неизвестная команда: {command}");
                    Console.WriteLine("Введите 'help' для просмотра доступных команд");
                    return null;
            }
        }

        private static ICommand ParseSearch(string args)
        {
            var flags = new SearchFlags();
            if (string.IsNullOrEmpty(args))
            {
                return new SearchCommand(flags);
            }

            var parts = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < parts.Length; i++)
            {
                switch (parts[i])
                {
                    case "--contains":
                        if (i + 1 < parts.Length)
                        {
                            flags.ContainsText = parts[++i].Trim('"');
                        }
                        break;

                    case "--starts-with":
                        if (i + 1 < parts.Length)
                        {
                            flags.StartsWithText = parts[++i].Trim('"');
                        }
                        break;

                    case "--ends-with":
                        if (i + 1 < parts.Length)
                        {
                            flags.EndsWithText = parts[++i].Trim('"');
                        }
                        break;

                    case "--from":
                        if (i + 1 < parts.Length)
                        {
                            if (DateTime.TryParse(parts[++i], out DateTime from))
                            {
                                flags.FromDate = from;
                            }
                        }
                        break;

                    case "--to":
                        if (i + 1 < parts.Length)
                        {
                            if (DateTime.TryParse(parts[++i], out DateTime to))
                            {
                                flags.ToDate = to;
                            }
                        }
                        break;

                    case "--status":
                        if (i + 1 < parts.Length)
                        {
                            if (Enum.TryParse<TodoStatus>(parts[++i], true, out TodoStatus status))
                            {
                                flags.Status = status;
                            }
                        }
                        break;

                    case "--sort":
                        if (i + 1 < parts.Length)
                        {
                            flags.SortBy = parts[++i].ToLower();
                        }
                        break;

                    case "--desc":
                        flags.Descending = true;
                        break;

                    case "--top":
                        if (i + 1 < parts.Length)
                        {
                            if (int.TryParse(parts[++i], out int top) && top > 0)
                            {
                                flags.TopCount = top;
                            }
                        }
                        break;
                }
            }
            return new SearchCommand(flags);
        }

        private static bool ValidationNumber(string taskText, Todolist todoList, out TodoItem item, out int taskNumber)
        {
            item = null;
            taskNumber = 0;

            if (!int.TryParse(taskText, out taskNumber))
            {
                Console.WriteLine("Номер задачи должен быть числом");
                return false;
            }

            if (taskNumber <= 0 || taskNumber > todoList.GetCount())
            {
                Console.WriteLine($"Неверный номер задачи. Должен быть от 1 до {todoList.GetCount()}");
                return false;
            }

            item = todoList.GetItem(taskNumber - 1);
            return true;
        }
    }
}
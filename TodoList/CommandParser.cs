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
            string arguments;
            if (parts.Length > 1) arguments = parts[1];
            else arguments = "";

            switch (command)
            {
                case "help":
                    return new HelpCommand();

                case "profile":
                    return new ProfileCommand(profile);

                case "add":
                    if (arguments == "--multiline" || arguments == "-m") return new AddCommand(todoList, "", true, TodoFilePath);
                    else return new AddCommand(todoList, arguments, false, TodoFilePath);

                case "add_user":
                    return new SetDataUserCommand(ProfileFilePath);

                case "read":
                    if (ValidationNumber(arguments, todoList, out TodoItem readItem, out int readTaskNumber)) return new ReadCommand(todoList, readTaskNumber);
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
                    if (ValidationNumber(arguments, todoList, out TodoItem deleteItem, out int deleteTaskNumber)) return new DeleteCommand(todoList, deleteTaskNumber, TodoFilePath);
                    Console.WriteLine("Неправильный формат, должен быть: delete номер_задачи");
                    return null;

                case "update":
                    string[] updateParts = arguments.Split(' ', 2);
                    if (updateParts.Length == 2 && ValidationNumber(updateParts[0], todoList, out TodoItem updateItem, out int updateTaskNumber)) return new UpdateCommand(todoList, updateTaskNumber, updateParts[1], TodoFilePath);
                    Console.WriteLine("Неправильный формат, должен быть: update номер_задачи \"новый_текст\" или update номер_задачи новый_текст");
                    return null;

                case "exit":
                    return new ExitCommand();

                default:
                    Console.WriteLine($"Неизвестная команда: {command}");
                    Console.WriteLine("Введите 'help' для просмотра доступных команд");
                    return null;
            }
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
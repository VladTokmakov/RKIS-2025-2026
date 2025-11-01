using System;
namespace Todolist
{
    public static class CommandParser
    {
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
                    if (arguments == "--multiline" || arguments == "-m") return new AddCommand(todoList, "", true);
                    else return new AddCommand(todoList, arguments);

                case "read":
                    if (int.TryParse(arguments, out int readTaskNumber)) return new ReadCommand(todoList, readTaskNumber);
                    Console.WriteLine("Неправильный формат, должен быть: read номер_задачи");
                    return null;

                case "view":
                    bool showIndex = arguments.Contains("-i") || arguments.Contains("--index");
                    bool showStatus = arguments.Contains("-s") || arguments.Contains("--status");
                    bool showDate = arguments.Contains("-d") || arguments.Contains("--update-date");
                    bool showAll = arguments.Contains("-a") || arguments.Contains("--all");
                    return new ViewCommand(todoList, showIndex, showStatus, showDate, showAll);

                case "done":
                    if (int.TryParse(arguments, out int doneTaskNumber)) return new DoneCommand(todoList, doneTaskNumber);
                    Console.WriteLine("Неправильный формат, должен быть: done номер_задачи");
                    return null;

                case "delete":
                    if (int.TryParse(arguments, out int deleteTaskNumber)) return new DeleteCommand(todoList, deleteTaskNumber);
                    Console.WriteLine("Неправильный формат, должен быть: delete номер_задачи");
                    return null;

                case "update":
                    string[] updateParts = arguments.Split(' ', 2);
                    if (updateParts.Length == 2 && int.TryParse(updateParts[0], out int updateTaskNumber)) return new UpdateCommand(todoList, updateTaskNumber, updateParts[1]);
                    Console.WriteLine("Неправильный формат, должен быть: update номер_задачи \"новый_текст\" или update номер_задачи новый_текст");
                    return null;

                default:
                    Console.WriteLine($"Неизвестная команда: {command}");
                    Console.WriteLine("Введите 'help' для просмотра доступных команд");
                    return null;
            }
        }
    }
}
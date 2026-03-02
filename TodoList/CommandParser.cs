using System;
using System.Text.RegularExpressions;
using Todolist.Exceptions;

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
            if (string.IsNullOrWhiteSpace(input))
                throw new InvalidArgumentException("Команда не может быть пустой.");

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
                    
                    if (profile == null)
                        throw new ProfileNotFoundException("Нет активного профиля.");
                    
                    return new ProfileCommand(profile);
                case "add":
                    if (arguments == "--multiline" || arguments == "-m")
                    {
                        if (profile == null)
                            throw new AuthenticationException("Необходимо войти в профиль для добавления задач.");
                        return new AddCommand(todoList, "", true, TodoFilePath);
                    }
                    else
                    {
                        if (profile == null)
                            throw new AuthenticationException("Необходимо войти в профиль для добавления задач.");
                        
                        if (string.IsNullOrWhiteSpace(arguments))
                            throw new InvalidArgumentException("Укажите текст задачи. Использование: add <текст> или add --multiline");
                        
                        return new AddCommand(todoList, arguments, false, TodoFilePath);
                    }
                case "add_user":
                    return new SetDataUserCommand(ProfileFilePath);
                case "read":
                    if (profile == null)
                        throw new AuthenticationException("Необходимо войти в профиль для просмотра задач.");
                    
                    if (ValidationNumber(arguments, todoList, out TodoItem readItem, out int readTaskNumber))
                        return new ReadCommand(todoList, readTaskNumber);
                    
                    throw new InvalidArgumentException("Неправильный формат, должен быть: read номер_задачи");
                case "view":
                    if (profile == null)
                        throw new AuthenticationException("Необходимо войти в профиль для просмотра задач.");
                    
                    bool showIndex = arguments.Contains("-i") || arguments.Contains("--index");
                    bool showStatus = arguments.Contains("-s") || arguments.Contains("--status");
                    bool showDate = arguments.Contains("-d") || arguments.Contains("--update-date");
                    bool showAll = arguments.Contains("-a") || arguments.Contains("--all");
                    
                    return new ViewCommand(todoList, showIndex, showStatus, showDate, showAll);
                case "status":
                    if (profile == null)
                        throw new AuthenticationException("Необходимо войти в профиль для изменения статуса задач.");
                    
                    string[] statusParts = arguments.Split(' ');
                    if (statusParts.Length == 2 && ValidationNumber(statusParts[0], todoList, out TodoItem statusItem, out int statusTaskNumber))
                    {
                        if (Enum.TryParse<TodoStatus>(statusParts[1], true, out TodoStatus status))
                        {
                            return new StatusCommand(todoList, statusTaskNumber, status, TodoFilePath);
                        }
                        else
                        {
                            throw new InvalidArgumentException($"Неизвестный статус: {statusParts[1]}. Доступные статусы: NotStarted, InProgress, Completed, Postponed, Failed");
                        }
                    }
                    else
                    {
                        throw new InvalidArgumentException("Неправильный формат, должен быть: status номер_задачи статус");
                    }
                case "delete":
                    if (profile == null)
                        throw new AuthenticationException("Необходимо войти в профиль для удаления задач.");
                    
                    if (ValidationNumber(arguments, todoList, out TodoItem deleteItem, out int deleteTaskNumber))
                        return new DeleteCommand(todoList, deleteTaskNumber, TodoFilePath);
                    
                    throw new InvalidArgumentException("Неправильный формат, должен быть: delete номер_задачи");
                case "update":
                    if (profile == null)
                        throw new AuthenticationException("Необходимо войти в профиль для обновления задач.");
                    
                    string[] updateParts = arguments.Split(' ', 2);
                    if (updateParts.Length == 2 && ValidationNumber(updateParts[0], todoList, out TodoItem updateItem, out int updateTaskNumber))
                    {
                        if (string.IsNullOrWhiteSpace(updateParts[1]))
                            throw new InvalidArgumentException("Новый текст задачи не может быть пустым.");
                        
                        return new UpdateCommand(todoList, updateTaskNumber, updateParts[1], TodoFilePath);
                    }
                    
                    throw new InvalidArgumentException("Неправильный формат, должен быть: update номер_задачи \"новый_текст\" или update номер_задачи новый_текст");
                case "undo":
                    return new UndoCommand();
                case "redo":
                    return new RedoCommand();
                case "search":
                    return ParseSearch(arguments);
                case "load":
                    return ParseLoad(arguments);
                case "exit":
                    return new ExitCommand();
                default:
                    throw new InvalidCommandException($"Неизвестная команда: {command}. Введите 'help' для просмотра доступных команд");
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
            var recognizedFlags = new HashSet<string> { 
                "--contains", "--starts-with", "--ends-with", 
                "--from", "--to", "--status", "--sort", "--desc", "--top" 
            };

            for (int i = 0; i < parts.Length; i++)
            {
                string currentFlag = parts[i];
                
                if (!currentFlag.StartsWith("--"))
                    continue;

                if (!recognizedFlags.Contains(currentFlag))
                    throw new InvalidArgumentException($"Неизвестный флаг '{currentFlag}' для команды search.");

                switch (currentFlag)
                {
                    case "--contains":
                        if (i + 1 >= parts.Length || parts[i + 1].StartsWith("--"))
                            throw new InvalidArgumentException("Флаг --contains требует значение.");
                        flags.ContainsText = parts[++i].Trim('"');
                        break;
                    case "--starts-with":
                        if (i + 1 >= parts.Length || parts[i + 1].StartsWith("--"))
                            throw new InvalidArgumentException("Флаг --starts-with требует значение.");
                        flags.StartsWithText = parts[++i].Trim('"');
                        break;
                    case "--ends-with":
                        if (i + 1 >= parts.Length || parts[i + 1].StartsWith("--"))
                            throw new InvalidArgumentException("Флаг --ends-with требует значение.");
                        flags.EndsWithText = parts[++i].Trim('"');
                        break;
                    case "--from":
                        if (i + 1 >= parts.Length || parts[i + 1].StartsWith("--"))
                            throw new InvalidArgumentException("Флаг --from требует значение.");
                        
                        string fromStr = parts[++i];
                        if (!DateTime.TryParse(fromStr, out DateTime from))
                            throw new InvalidArgumentException($"Неверный формат даты для --from: '{fromStr}'. Ожидается yyyy-MM-dd.");
                        
                        flags.FromDate = from;
                        break;
                    case "--to":
                        if (i + 1 >= parts.Length || parts[i + 1].StartsWith("--"))
                            throw new InvalidArgumentException("Флаг --to требует значение.");
                        
                        string toStr = parts[++i];
                        if (!DateTime.TryParse(toStr, out DateTime to))
                            throw new InvalidArgumentException($"Неверный формат даты для --to: '{toStr}'. Ожидается yyyy-MM-dd.");
                        
                        flags.ToDate = to;
                        break;
                    case "--status":
                        if (i + 1 >= parts.Length || parts[i + 1].StartsWith("--"))
                            throw new InvalidArgumentException("Флаг --status требует значение.");
                        
                        string statusStr = parts[++i];
                        if (!Enum.TryParse<TodoStatus>(statusStr, true, out TodoStatus status))
                            throw new InvalidArgumentException($"Неверный статус '{statusStr}'. Допустимые значения: NotStarted, InProgress, Completed, Postponed, Failed.");
                        
                        flags.Status = status;
                        break;
                    case "--sort":
                        if (i + 1 >= parts.Length || parts[i + 1].StartsWith("--"))
                            throw new InvalidArgumentException("Флаг --sort требует значение (text или date).");
                        
                        flags.SortBy = parts[++i].ToLower();
                        if (flags.SortBy != "text" && flags.SortBy != "date")
                            throw new InvalidArgumentException("Флаг --sort принимает только значения 'text' или 'date'.");
                        break;
                    case "--desc":
                        flags.Descending = true;
                        break;
                    case "--top":
                        if (i + 1 >= parts.Length || parts[i + 1].StartsWith("--"))
                            throw new InvalidArgumentException("Флаг --top требует положительное целое число.");
                        
                        if (!int.TryParse(parts[++i], out int top) || top <= 0)
                            throw new InvalidArgumentException("Флаг --top требует положительное целое число.");
                        
                        flags.TopCount = top;
                        break;
                }
            }

            return new SearchCommand(flags);
        }

        private static ICommand ParseLoad(string args)
        {
            if (string.IsNullOrWhiteSpace(args))
                throw new InvalidArgumentException("Недостаточно параметров для команды load. Использование: load <количество> <размер>");

            var parts = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
                throw new InvalidArgumentException("Неверное количество параметров. Использование: load <количество> <размер>");

            if (!int.TryParse(parts[0], out int count) || count <= 0)
                throw new InvalidArgumentException("Количество загрузок должно быть положительным целым числом.");

            if (!int.TryParse(parts[1], out int size) || size <= 0)
                throw new InvalidArgumentException("Размер загрузки должен быть положительным целым числом.");

            return new LoadCommand(count, size);
        }

        private static bool ValidationNumber(string taskText, Todolist todoList, out TodoItem item, out int taskNumber)
        {
            item = null;
            taskNumber = 0;

            if (!int.TryParse(taskText, out taskNumber))
                throw new InvalidArgumentException("Номер задачи должен быть числом");

            if (taskNumber <= 0)
                throw new InvalidArgumentException("Номер задачи должен быть положительным числом");

            if (taskNumber > todoList.GetCount())
                throw new TaskNotFoundException($"Задача с номером {taskNumber} не найдена. Всего задач: {todoList.GetCount()}");

            item = todoList.GetItem(taskNumber - 1);
            return true;
        }
    }
}
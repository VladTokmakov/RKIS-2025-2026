using System;
using System.Collections.Generic;
using System.Linq;
using Todolist.Commands;
using Todolist.Exceptions;
using Todolist.Models;

namespace Todolist
{
    public static class CommandParser
    {
        public static ICommand? Parse(string input, TodoList todoList, Profile? currentProfile)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new InvalidArgumentException("Команда не может быть пустой.");

            string[] parts = input.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string command = parts[0].ToLower();

            switch (command)
            {
                case "help":
                    return new HelpCommand();

                case "exit":
                    return new ExitCommand();

                case "profile":
                    if (parts.Length > 1 && (parts[1] == "--out" || parts[1] == "-o"))
                    {
                        AppInfo.ShouldLogout = true;
                        return null;
                    }
                    if (currentProfile == null)
                        throw new ProfileNotFoundException("Профиль не найден. Создайте профиль командой 'add_user'");
                    return new ProfileCommand(currentProfile);

                case "add_user":
                    return new SetDataUserCommand();

                case "add":
                    if (currentProfile == null)
                        throw new AuthenticationException("Необходимо войти в профиль для добавления задач.");
                    
                    bool isMultiline = input.Contains("--multiline") || input.Contains("-m");
                    string taskText = "";
                    
                    if (!isMultiline)
                    {
                        var textParts = parts.Skip(1).ToArray();
                        if (textParts.Length == 0)
                            throw new InvalidArgumentException("Укажите текст задачи. Пример: add \"Купить молоко\"");
                        taskText = string.Join(" ", textParts);
                        if (taskText.StartsWith("\"") && taskText.EndsWith("\""))
                            taskText = taskText.Substring(1, taskText.Length - 2);
                    }
                    
                    return new AddCommand(todoList, taskText, isMultiline);

                case "read":
                    if (currentProfile == null)
                        throw new AuthenticationException("Необходимо войти в профиль для просмотра задач.");
                    
                    if (parts.Length < 2)
                        throw new InvalidArgumentException("Укажите номер задачи. Пример: read 1");
                    
                    if (!int.TryParse(parts[1], out int readIndex) || readIndex < 1)
                        throw new InvalidArgumentException("Номер задачи должен быть положительным числом.");
                    
                    if (readIndex > todoList.GetCount())
                        throw new TaskNotFoundException($"Задача с номером {readIndex} не найдена.");
                    
                    return new ReadCommand(todoList, readIndex);

                case "view":
                    if (currentProfile == null)
                        throw new AuthenticationException("Необходимо войти в профиль для просмотра задач.");
                    
                    bool showIndex = input.Contains("-i") || input.Contains("--index");
                    bool showStatus = input.Contains("-s") || input.Contains("--status");
                    bool showDate = input.Contains("-d") || input.Contains("--update-date");
                    bool showAll = input.Contains("-a") || input.Contains("--all");
                    
                    return new ViewCommand(todoList, showIndex, showStatus, showDate, showAll);

                case "status":
                    if (currentProfile == null)
                        throw new AuthenticationException("Необходимо войти в профиль для изменения статуса.");
                    
                    if (parts.Length < 3)
                        throw new InvalidArgumentException("Укажите номер задачи и статус. Пример: status 1 Completed");
                    
                    if (!int.TryParse(parts[1], out int statusIndex) || statusIndex < 1)
                        throw new InvalidArgumentException("Номер задачи должен быть положительным числом.");
                    
                    if (statusIndex > todoList.GetCount())
                        throw new TaskNotFoundException($"Задача с номером {statusIndex} не найдена.");
                    
                    if (!Enum.TryParse<TodoStatus>(parts[2], true, out TodoStatus newStatus))
                        throw new InvalidArgumentException($"Неизвестный статус. Доступные статусы: {string.Join(", ", Enum.GetNames<TodoStatus>())}");
                    
                    return new StatusCommand(todoList, statusIndex, newStatus);

                case "delete":
                    if (currentProfile == null)
                        throw new AuthenticationException("Необходимо войти в профиль для удаления задач.");
                    
                    if (parts.Length < 2)
                        throw new InvalidArgumentException("Укажите номер задачи. Пример: delete 1");
                    
                    if (!int.TryParse(parts[1], out int deleteIndex) || deleteIndex < 1)
                        throw new InvalidArgumentException("Номер задачи должен быть положительным числом.");
                    
                    if (deleteIndex > todoList.GetCount())
                        throw new TaskNotFoundException($"Задача с номером {deleteIndex} не найдена.");
                    
                    var itemToDelete = todoList.GetItem(deleteIndex);
                    return new DeleteCommand(todoList, deleteIndex, itemToDelete.Id);

                case "update":
                    if (currentProfile == null)
                        throw new AuthenticationException("Необходимо войти в профиль для обновления задач.");
                    
                    if (parts.Length < 3)
                        throw new InvalidArgumentException("Укажите номер задачи и новый текст. Пример: update 1 \"Новый текст\"");
                    
                    if (!int.TryParse(parts[1], out int updateIndex) || updateIndex < 1)
                        throw new InvalidArgumentException("Номер задачи должен быть положительным числом.");
                    
                    if (updateIndex > todoList.GetCount())
                        throw new TaskNotFoundException($"Задача с номером {updateIndex} не найдена.");
                    
                    string newText = string.Join(" ", parts.Skip(2));
                    if (newText.StartsWith("\"") && newText.EndsWith("\""))
                        newText = newText.Substring(1, newText.Length - 2);
                    
                    if (string.IsNullOrWhiteSpace(newText))
                        throw new InvalidArgumentException("Текст задачи не может быть пустым.");
                    
                    return new UpdateCommand(todoList, updateIndex, newText);

                case "undo":
                    return new UndoCommand();

                case "redo":
                    return new RedoCommand();

                case "search":
                    if (currentProfile == null)
                        throw new AuthenticationException("Необходимо войти в профиль для поиска задач.");
                    
                    var searchFlags = ParseSearchFlags(input);
                    return new SearchCommand(searchFlags);

                case "load":
                    if (parts.Length < 3)
                        throw new InvalidArgumentException("Укажите количество загрузок и размер. Пример: load 5 100");
                    
                    if (!int.TryParse(parts[1], out int downloads) || downloads <= 0)
                        throw new InvalidArgumentException("Количество загрузок должно быть положительным числом.");
                    
                    if (!int.TryParse(parts[2], out int size) || size <= 0)
                        throw new InvalidArgumentException("Размер загрузки должен быть положительным числом.");
                    
                    return new LoadCommand(downloads, size);

                case "sync":
                    bool pull = input.Contains("--pull");
                    bool push = input.Contains("--push");
                    
                    if (!pull && !push)
                        throw new InvalidArgumentException("Укажите флаг --pull или --push для синхронизации.");
                    
                    return new SyncCommand(pull, push);

                default:
                    throw new InvalidCommandException($"Неизвестная команда: {command}");
            }
        }

        private static SearchFlags ParseSearchFlags(string input)
        {
            var flags = new SearchFlags();
            var args = input.Substring(input.IndexOf(' ')).Trim();
            var tokens = Tokenize(args);
            
            for (int i = 0; i < tokens.Count; i++)
            {
                string token = tokens[i].ToLower();
                
                switch (token)
                {
                    case "--contains":
                        if (i + 1 < tokens.Count)
                            flags.ContainsText = tokens[++i];
                        break;
                    case "--starts-with":
                        if (i + 1 < tokens.Count)
                            flags.StartsWithText = tokens[++i];
                        break;
                    case "--ends-with":
                        if (i + 1 < tokens.Count)
                            flags.EndsWithText = tokens[++i];
                        break;
                    case "--from":
                        if (i + 1 < tokens.Count && DateTime.TryParse(tokens[i + 1], out DateTime fromDate))
                            flags.FromDate = fromDate;
                        i++;
                        break;
                    case "--to":
                        if (i + 1 < tokens.Count && DateTime.TryParse(tokens[i + 1], out DateTime toDate))
                            flags.ToDate = toDate;
                        i++;
                        break;
                    case "--status":
                        if (i + 1 < tokens.Count && Enum.TryParse<TodoStatus>(tokens[i + 1], true, out TodoStatus status))
                            flags.Status = status;
                        i++;
                        break;
                    case "--sort":
                        if (i + 1 < tokens.Count)
                            flags.SortBy = tokens[++i];
                        break;
                    case "--desc":
                        flags.Descending = true;
                        break;
                    case "--top":
                        if (i + 1 < tokens.Count && int.TryParse(tokens[i + 1], out int top))
                            flags.TopCount = top;
                        i++;
                        break;
                }
            }
            
            return flags;
        }

        private static List<string> Tokenize(string args)
        {
            var tokens = new List<string>();
            var current = "";
            bool inQuotes = false;
            
            for (int i = 0; i < args.Length; i++)
            {
                char c = args[i];
                
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ' ' && !inQuotes)
                {
                    if (!string.IsNullOrWhiteSpace(current))
                    {
                        tokens.Add(current);
                        current = "";
                    }
                }
                else
                {
                    current += c;
                }
            }
            
            if (!string.IsNullOrWhiteSpace(current))
                tokens.Add(current);
            
            return tokens;
        }
    }
}
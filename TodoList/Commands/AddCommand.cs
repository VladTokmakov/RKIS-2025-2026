using System;
using Todolist.Exceptions;

namespace Todolist
{
    public class AddCommand : IUndo
    {
        public bool IsMultiline { get; private set; }
        public string TaskText { get; private set; }
        public TodoList TodoList { get; private set; }
        private readonly string? TodoFilePath;
        private readonly IDataStorage? _storage;
        private TodoItem? _addedItem;

        public AddCommand(TodoList todoList, string taskText, bool isMultiline = false, 
                          string? todoFilePath = null, IDataStorage? storage = null)
        {
            TodoList = todoList;
            TaskText = taskText;
            IsMultiline = isMultiline;
            TodoFilePath = todoFilePath;
            _storage = storage;
        }

        public void Execute()
        {
            try
            {
                if (TodoList == null)
                    throw new BusinessLogicException("Ошибка: нет активного списка задач.");

                string finalText;
                if (IsMultiline)
                {
                    finalText = ReadMultiline();
                }
                else
                {
                    finalText = TaskText;
                    if (finalText.StartsWith("\"") && finalText.EndsWith("\""))
                    {
                        finalText = finalText.Substring(1, finalText.Length - 2);
                    }
                }

                if (string.IsNullOrWhiteSpace(finalText))
                    throw new InvalidArgumentException("Текст задачи не может быть пустым.");

                _addedItem = new TodoItem(finalText);
                TodoList.Add(_addedItem);
                AppInfo.UndoStack.Push(this);
                AppInfo.RedoStack.Clear();
                Console.WriteLine($"Добавлена задача №{TodoList.GetCount()}: {finalText}");

                SaveTodos();
            }
            catch (Exception ex) when (!(ex is InvalidArgumentException || ex is BusinessLogicException))
            {
                throw;
            }
        }

        public void Unexecute()
        {
            try
            {
                if (_addedItem != null && TodoList != null)
                {
                    int lastIndex = TodoList.GetCount();
                    if (lastIndex > 0)
                    {
                        TodoList.Delete(lastIndex);
                        Console.WriteLine("Добавление задачи отменено.");
                        SaveTodos();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicException($"Ошибка при отмене добавления задачи: {ex.Message}");
            }
        }

        private void SaveTodos()
        {
            if (_storage != null && AppInfo.CurrentProfile != null)
            {
                try
                {
                    _storage.SaveTodos(AppInfo.CurrentProfile.Id, TodoList);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Предупреждение: не удалось сохранить задачи - {ex.Message}");
                }
            }
        }

        private string ReadMultiline()
        {
            Console.WriteLine("Многострочный режим. Введите задачи (для завершения введите '!end'):");
            string multilineText = "";
            while (true)
            {
                Console.Write("> ");
                string? line = Console.ReadLine();
                if (line == null)
                    continue;
                if (line.ToLower() == "!end")
                    break;
                multilineText += line + "\n";
            }
            return multilineText.Trim();
        }
    }
}
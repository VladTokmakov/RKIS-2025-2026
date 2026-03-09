using System;
using Todolist.Exceptions;

namespace Todolist
{
    public class AddCommand : IUndo
    {
        public bool IsMultiline { get; private set; }
        public string TaskText { get; private set; }
        public Todolist TodoList { get; private set; }

        private readonly string TodoFilePath;
        private readonly IDataStorage _storage;
        private TodoItem _addedItem;

        public AddCommand(Todolist todoList, string taskText, bool isMultiline = false, 
                          string todoFilePath = null, IDataStorage storage = null)
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

                if (!string.IsNullOrEmpty(TodoFilePath) && _storage != null)
                {
                    try
                    {
                        _storage.SaveTodos(TodoList, TodoFilePath);
                    }
                    catch (Exception ex)
                    {
                        throw new BusinessLogicException($"Ошибка сохранения задачи в файл: {ex.Message}");
                    }
                }
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
                        TodoList.Delete(lastIndex - 1);
                        Console.WriteLine("Добавление задачи отменено.");

                        if (!string.IsNullOrEmpty(TodoFilePath) && _storage != null)
                        {
                            _storage.SaveTodos(TodoList, TodoFilePath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicException($"Ошибка при отмене добавления задачи: {ex.Message}");
            }
        }

        private string ReadMultiline()
        {
            Console.WriteLine("Многострочный режим. Введите задачи (для завершения введите '!end'):");
            string multilineText = "";

            while (true)
            {
                Console.Write("> ");
                string line = Console.ReadLine();

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
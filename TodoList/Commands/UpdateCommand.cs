using System;
using Todolist.Exceptions;

namespace Todolist
{
    public class UpdateCommand : IUndo
    {
        public int TaskNumber { get; private set; }
        public string NewText { get; private set; }
        public Todolist TodoList { get; private set; }

        private readonly string TodoFilePath;
        private readonly IDataStorage _storage;
        private string _oldText;

        public UpdateCommand(Todolist todoList, int taskNumber, string newText, 
                             string todoFilePath = null, IDataStorage storage = null)
        {
            TodoList = todoList;
            TaskNumber = taskNumber;
            NewText = newText;
            TodoFilePath = todoFilePath;
            _storage = storage;
        }

        public void Execute()
        {
            try
            {
                if (TodoList == null)
                    throw new BusinessLogicException("Ошибка: нет активного списка задач.");

                int index = TaskNumber - 1;

                if (index < 0)
                    throw new InvalidArgumentException("Номер задачи должен быть положительным числом.");

                if (index >= TodoList.GetCount())
                    throw new TaskNotFoundException($"Задача с номером {TaskNumber} не найдена.");

                TodoItem item = TodoList.GetItem(index);
                _oldText = item.Text;

                string finalText = NewText;
                if (finalText.StartsWith("\"") && finalText.EndsWith("\""))
                {
                    finalText = finalText.Substring(1, finalText.Length - 2);
                }

                if (string.IsNullOrWhiteSpace(finalText))
                    throw new InvalidArgumentException("Текст задачи не может быть пустым.");

                item.UpdateText(finalText);

                AppInfo.UndoStack.Push(this);
                AppInfo.RedoStack.Clear();

                Console.WriteLine($"Обновлена задача: \nБыло: Задача №{TaskNumber} \"{_oldText}\" \nСтало: Задача №{TaskNumber} \"{finalText}\"");

                if (!string.IsNullOrEmpty(TodoFilePath) && _storage != null)
                {
                    _storage.SaveTodos(TodoList, TodoFilePath);
                }
            }
            catch (Exception ex) when (!(ex is TaskNotFoundException || ex is InvalidArgumentException || ex is BusinessLogicException))
            {
                throw;
            }
        }

        public void Unexecute()
        {
            try
            {
                if (TodoList != null)
                {
                    int index = TaskNumber - 1;
                    if (index >= 0 && index < TodoList.GetCount())
                    {
                        TodoList.GetItem(index).UpdateText(_oldText);
                        Console.WriteLine($"Текст задачи №{TaskNumber} возвращен к предыдущему значению.");

                        if (!string.IsNullOrEmpty(TodoFilePath) && _storage != null)
                        {
                            _storage.SaveTodos(TodoList, TodoFilePath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicException($"Ошибка при отмене обновления: {ex.Message}");
            }
        }
    }
}
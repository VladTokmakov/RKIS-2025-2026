using System;
using System.Collections.Generic;
using Todolist.Exceptions;

namespace Todolist
{
    public class DeleteCommand : IUndo
    {
        public int TaskNumber { get; private set; }
        public TodoList TodoList { get; private set; }
        private readonly string? TodoFilePath;
        private readonly IDataStorage? _storage;
        private TodoItem? _deletedItem;
        private int _actualIndex;

        public DeleteCommand(TodoList todoList, int taskNumber, string? todoFilePath = null, IDataStorage? storage = null)
        {
            TodoList = todoList;
            TaskNumber = taskNumber;
            TodoFilePath = todoFilePath;
            _storage = storage;
        }

        public void Execute()
        {
            try
            {
                if (TodoList == null)
                    throw new BusinessLogicException("Ошибка: нет активного списка задач.");

                _actualIndex = TaskNumber - 1;
                if (_actualIndex < 0)
                    throw new InvalidArgumentException("Номер задачи должен быть положительным числом.");
                if (_actualIndex >= TodoList.GetCount())
                    throw new TaskNotFoundException($"Задача с номером {TaskNumber} не найдена.");

                _deletedItem = TodoList.GetItem(TaskNumber);
                string taskText = _deletedItem.Text;
                TodoList.Delete(TaskNumber);
                
                AppInfo.UndoStack.Push(this);
                AppInfo.RedoStack.Clear();
                Console.WriteLine($"Задача №{TaskNumber} '{taskText}' удалена");

                SaveTodos();
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
                if (_deletedItem != null && TodoList != null)
                {
                    var items = new List<TodoItem>();
                    for (int i = 0; i < TodoList.GetCount(); i++)
                    {
                        items.Add(TodoList.GetItem(i + 1));
                    }
                    if (_actualIndex <= items.Count)
                    {
                        items.Insert(_actualIndex, _deletedItem);
                        while (TodoList.GetCount() > 0)
                        {
                            TodoList.Delete(1);
                        }
                        foreach (var item in items)
                        {
                            TodoList.Add(item);
                        }
                        Console.WriteLine("Удаление задачи отменено.");
                        SaveTodos();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicException($"Ошибка при отмене удаления: {ex.Message}");
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
    }
}
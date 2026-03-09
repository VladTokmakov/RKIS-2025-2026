using System;
using System.Collections.Generic;
using Todolist.Exceptions;

namespace Todolist
{
    public class DeleteCommand : IUndo
    {
        public int TaskNumber { get; private set; }
        public Todolist TodoList { get; private set; }

        private readonly string TodoFilePath;
        private readonly IDataStorage _storage;
        private TodoItem _deletedItem;
        private int _actualIndex;

        public DeleteCommand(Todolist todoList, int taskNumber, string todoFilePath = null, IDataStorage storage = null)
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

                _deletedItem = TodoList.GetItem(_actualIndex);
                string taskText = _deletedItem.Text;

                TodoList.Delete(_actualIndex);

                AppInfo.UndoStack.Push(this);
                AppInfo.RedoStack.Clear();

                Console.WriteLine($"Задача №{TaskNumber} '{taskText}' удалена");

                if (!string.IsNullOrEmpty(TodoFilePath) && _storage != null)
                {
                    try
                    {
                        _storage.SaveTodos(TodoList, TodoFilePath);
                    }
                    catch (Exception ex)
                    {
                        throw new BusinessLogicException($"Ошибка сохранения после удаления: {ex.Message}");
                    }
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
                if (_deletedItem != null && TodoList != null)
                {
                    var items = new System.Collections.Generic.List<TodoItem>();
                    for (int i = 0; i < TodoList.GetCount(); i++)
                    {
                        items.Add(TodoList.GetItem(i));
                    }

                    if (_actualIndex <= items.Count)
                    {
                        items.Insert(_actualIndex, _deletedItem);

                        while (TodoList.GetCount() > 0)
                        {
                            TodoList.Delete(0);
                        }

                        foreach (var item in items)
                        {
                            TodoList.Add(item);
                        }

                        Console.WriteLine("Удаление задачи отменено.");

                        if (!string.IsNullOrEmpty(TodoFilePath) && _storage != null)
                        {
                            _storage.SaveTodos(TodoList, TodoFilePath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicException($"Ошибка при отмене удаления: {ex.Message}");
            }
        }
    }
}
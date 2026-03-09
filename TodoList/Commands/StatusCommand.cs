using System;
using Todolist.Exceptions;

namespace Todolist
{
    public class StatusCommand : IUndo
    {
        public Todolist TodoList { get; private set; }
        public int TaskNumber { get; private set; }
        public TodoStatus Status { get; private set; }

        private readonly string TodoFilePath;
        private readonly IDataStorage _storage;
        private TodoStatus _oldStatus;

        public StatusCommand(Todolist todoList, int taskNumber, TodoStatus status, 
                             string todoFilePath = null, IDataStorage storage = null)
        {
            TodoList = todoList;
            TaskNumber = taskNumber;
            Status = status;
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
                _oldStatus = item.Status;

                TodoList.SetStatus(index, Status);

                AppInfo.UndoStack.Push(this);
                AppInfo.RedoStack.Clear();

                Console.WriteLine($"Задача №{TaskNumber} статус изменен с {_oldStatus} на {Status}");

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
                        TodoList.SetStatus(index, _oldStatus);
                        Console.WriteLine($"Статус задачи №{TaskNumber} возвращен к {_oldStatus}.");

                        if (!string.IsNullOrEmpty(TodoFilePath) && _storage != null)
                        {
                            _storage.SaveTodos(TodoList, TodoFilePath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicException($"Ошибка при отмене изменения статуса: {ex.Message}");
            }
        }
    }
}
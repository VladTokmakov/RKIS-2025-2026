using System;
using Todolist.Exceptions;

namespace Todolist
{
    public class StatusCommand : IUndo
    {
        public TodoList TodoList { get; private set; }
        public int TaskNumber { get; private set; }
        public TodoStatus Status { get; private set; }
        private readonly string? TodoFilePath;
        private readonly IDataStorage? _storage;
        private TodoStatus _oldStatus;

        public StatusCommand(TodoList todoList, int taskNumber, TodoStatus status, 
                              string? todoFilePath = null, IDataStorage? storage = null)
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

                if (TaskNumber < 1)
                    throw new InvalidArgumentException("Номер задачи должен быть положительным числом.");
                if (TaskNumber > TodoList.GetCount())
                    throw new TaskNotFoundException($"Задача с номером {TaskNumber} не найдена.");

                TodoItem item = TodoList.GetItem(TaskNumber);
                _oldStatus = item.Status;
                TodoList.SetStatus(TaskNumber, Status);
                
                AppInfo.UndoStack.Push(this);
                AppInfo.RedoStack.Clear();
                Console.WriteLine($"Задача №{TaskNumber} статус изменен с {_oldStatus} на {Status}");

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
                if (TodoList != null)
                {
                    if (TaskNumber >= 1 && TaskNumber <= TodoList.GetCount())
                    {
                        TodoList.SetStatus(TaskNumber, _oldStatus);
                        Console.WriteLine($"Статус задачи №{TaskNumber} возвращен к {_oldStatus}.");
                        SaveTodos();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicException($"Ошибка при отмене изменения статуса: {ex.Message}");
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
using System;
using Todolist.Exceptions;
using Todolist.Models;

namespace Todolist
{
    public class StatusCommand : IUndo
    {
        public TodoList TodoList { get; private set; }
        public int TaskNumber { get; private set; }
        public TodoStatus Status { get; private set; }
        
        private TodoStatus _oldStatus;
        private Guid _taskId;

        public StatusCommand(TodoList todoList, int taskNumber, TodoStatus status)
        {
            TodoList = todoList;
            TaskNumber = taskNumber;
            Status = status;
        }

        public void Execute()
        {
            try
            {
                if (TodoList == null)
                    throw new BusinessLogicException("Ошибка: нет активного списка задач.");
                
                if (TaskNumber < 1 || TaskNumber > TodoList.GetCount())
                    throw new TaskNotFoundException($"Задача с номером {TaskNumber} не найдена.");
                
                TodoItem item = TodoList.GetItem(TaskNumber);
                _oldStatus = item.Status;
                _taskId = item.Id;
                
                TodoList.SetStatus(TaskNumber, Status);
                AppInfo.TodoRepo.SetStatus(_taskId, Status);
                
                AppInfo.UndoStack.Push(this);
                AppInfo.RedoStack.Clear();
                Console.WriteLine($"Задача №{TaskNumber} статус изменен с {_oldStatus} на {Status}");
            }
            catch (Exception ex) when (!(ex is TaskNotFoundException || ex is InvalidArgumentException))
            {
                throw;
            }
        }

        public void Unexecute()
        {
            try
            {
                if (TodoList != null && TaskNumber >= 1 && TaskNumber <= TodoList.GetCount())
                {
                    TodoList.SetStatus(TaskNumber, _oldStatus);
                    AppInfo.TodoRepo.SetStatus(_taskId, _oldStatus);
                    Console.WriteLine($"Статус задачи №{TaskNumber} возвращен к {_oldStatus}.");
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicException($"Ошибка при отмене изменения статуса: {ex.Message}");
            }
        }
    }
}
using System;
using Todolist.Exceptions;
using Todolist.Models;

namespace Todolist
{
    public class DeleteCommand : IUndo
    {
        public int TaskNumber { get; private set; }
        public Guid TaskId { get; private set; }
        public TodoList TodoList { get; private set; }
        
        private TodoItem? _deletedItem;
        
        public DeleteCommand(TodoList todoList, int taskNumber, Guid taskId)
        {
            TodoList = todoList;
            TaskNumber = taskNumber;
            TaskId = taskId;
        }
        
        public void Execute()
        {
            try
            {
                if (TodoList == null)
                    throw new BusinessLogicException("Ошибка: нет активного списка задач.");
                
                if (TaskNumber < 1 || TaskNumber > TodoList.GetCount())
                    throw new TaskNotFoundException($"Задача с номером {TaskNumber} не найдена.");
                
                _deletedItem = TodoList.GetItem(TaskNumber);
                string taskText = _deletedItem.Text;
                TodoList.Delete(TaskNumber);
                
                AppInfo.TodoRepo.Delete(TaskId);
                
                AppInfo.UndoStack.Push(this);
                AppInfo.RedoStack.Clear();
                Console.WriteLine($"Задача №{TaskNumber} '{taskText}' удалена");
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
                if (_deletedItem != null && TodoList != null)
                {
                    TodoList.Insert(TaskNumber, _deletedItem);
                    AppInfo.TodoRepo.Add(_deletedItem, AppInfo.CurrentProfileId);
                    Console.WriteLine("Удаление задачи отменено.");
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicException($"Ошибка при отмене удаления: {ex.Message}");
            }
        }
    }
}
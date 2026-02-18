using System;

namespace Todolist
{
    public class StatusCommand : IUndo
    {
        public Todolist TodoList { get; private set; }
        public int TaskNumber { get; private set; }
        public TodoStatus Status { get; private set; }
        private readonly string TodoFilePath;
        private TodoStatus _oldStatus;

        public StatusCommand(Todolist todoList, int taskNumber, TodoStatus status, string todoFilePath = null)
        {
            TodoList = todoList;
            TaskNumber = taskNumber;
            Status = status;
            TodoFilePath = todoFilePath;
        }

        public void Execute()
        {
            if (TodoList == null)
            {
                Console.WriteLine("Ошибка: нет активного списка задач.");
                return;
            }

            int index = TaskNumber - 1;
            if (index < 0 || index >= TodoList.GetCount())
            {
                Console.WriteLine("Задача с таким номером не найдена.");
                return;
            }

            TodoItem item = TodoList.GetItem(index);
            _oldStatus = item.Status;
            TodoList.SetStatus(index, Status);
            AppInfo.UndoStack.Push(this);
            AppInfo.RedoStack.Clear();
            Console.WriteLine($"Задача №{TaskNumber} статус изменен с {_oldStatus} на {Status}");

            if (!string.IsNullOrEmpty(TodoFilePath)) 
                FileManager.SaveTodos(TodoList, TodoFilePath);
        }

        public void Unexecute()
        {
            if (TodoList != null)
            {
                int index = TaskNumber - 1;
                if (index >= 0 && index < TodoList.GetCount())
                {
                    TodoList.SetStatus(index, _oldStatus);
                    Console.WriteLine($"Статус задачи №{TaskNumber} возвращен к {_oldStatus}.");
                    
                    if (!string.IsNullOrEmpty(TodoFilePath)) 
                        FileManager.SaveTodos(TodoList, TodoFilePath);
                }
            }
        }
    }
}
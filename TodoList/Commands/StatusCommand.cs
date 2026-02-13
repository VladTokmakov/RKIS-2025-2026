using System;

namespace Todolist
{
    public class StatusCommand : ICommand
    {
        public Todolist TodoList { get; private set; }
        public int TaskNumber { get; private set; }
        public TodoStatus Status { get; private set; }
        private readonly string TodoFilePath;
        private TodoStatus _previousStatus;
        private int _taskIndex;

        public StatusCommand(Todolist todoList, int taskNumber, TodoStatus status, string todoFilePath = null)
        {
            TodoList = todoList;
            TaskNumber = taskNumber;
            Status = status;
            TodoFilePath = todoFilePath;
            _taskIndex = taskNumber - 1;
        }

        public void Execute()
        {
            TodoItem item = TodoList.GetItem(_taskIndex);
            _previousStatus = item.Status;
            TodoList.SetStatus(_taskIndex, Status);
            Console.WriteLine($"Задача №{TaskNumber} статус изменен на {Status}");
            if (!string.IsNullOrEmpty(TodoFilePath)) FileManager.SaveTodos(TodoList, TodoFilePath);
        }
        public void Unexecute()
        {
            TodoList.SetStatus(_taskIndex, _previousStatus);
            Console.WriteLine($"Отменено изменение статуса задачи №{TaskNumber} на {_previousStatus}");
            if (!string.IsNullOrEmpty(TodoFilePath)) FileManager.SaveTodos(TodoList, TodoFilePath);
        }
    }
}
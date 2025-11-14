using System;

namespace Todolist
{
    public class StatusCommand : ICommand
    {
        public Todolist TodoList { get; private set; }
        public int TaskNumber { get; private set; }
        public TodoStatus Status { get; private set; }
        private readonly string TodoFilePath;

        public StatusCommand(Todolist todoList, int taskNumber, TodoStatus status, string todoFilePath = null)
        {
            TodoList = todoList;
            TaskNumber = taskNumber;
            Status = status;
            TodoFilePath = todoFilePath;
        }

        public void Execute()
        {
            TodoItem item = TodoList.GetItem(TaskNumber - 1);
            TodoList.SetStatus(TaskNumber - 1, Status);
            Console.WriteLine($"Задача №{TaskNumber} статус изменен на {Status}");
            if (!string.IsNullOrEmpty(TodoFilePath)) FileManager.SaveTodos(TodoList, TodoFilePath);
        }
    }
}
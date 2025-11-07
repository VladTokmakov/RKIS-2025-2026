using System;
namespace Todolist
{
    public class DoneCommand : ICommand
    {
        public int TaskNumber { get; private set; }
        public Todolist TodoList { get; private set; }
        private readonly string TodoFilePath;

        public DoneCommand(Todolist todoList, int taskNumber, string todoFilePath = null)
        {
            TodoList = todoList;
            TaskNumber = taskNumber;
            TodoFilePath = todoFilePath;
        }

        public void Execute()
        {
            TodoItem item = TodoList.GetItem(TaskNumber - 1);
            item.MarkDone();
            Console.WriteLine($"Задача №{TaskNumber} отмечена как выполненная");

            if (!string.IsNullOrEmpty(TodoFilePath)) FileManager.SaveTodos(TodoList, TodoFilePath);
        }
    }
}
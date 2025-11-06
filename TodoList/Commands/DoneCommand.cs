using System;
namespace Todolist
{
    public class DoneCommand : ICommand
    {
        public int TaskNumber { get; private set; }
        public Todolist TodoList { get; private set; }

        public DoneCommand(Todolist todoList, int taskNumber)
        {
            TodoList = todoList;
            TaskNumber = taskNumber;
        }

        public void Execute()
        {
            TodoItem item = TodoList.GetItem(TaskNumber - 1);
            item.MarkDone();
            Console.WriteLine($"Задача №{TaskNumber} отмечена как выполненная");
        }
    }
}
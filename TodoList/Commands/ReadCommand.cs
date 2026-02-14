using System;
namespace Todolist
{
    public class ReadCommand : ICommand
    {
        public int TaskNumber { get; private set; }
        public Todolist TodoList { get; private set; }

        public ReadCommand(Todolist todoList, int taskNumber)
        {
            TodoList = todoList;
            TaskNumber = taskNumber;
        }

        public void Execute()
        {
            TodoItem item = TodoList.GetItem(TaskNumber - 1);
            Console.WriteLine(item.GetFullInfo());
        }
    }
}
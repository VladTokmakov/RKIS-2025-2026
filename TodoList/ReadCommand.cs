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
            if (TaskNumber <= 0 || TaskNumber > TodoList.GetCount())
            {
                Console.WriteLine($"Неверный номер задачи. Должен быть от 1 до {TodoList.GetCount()}");
                return;
            }

            TodoItem item = TodoList.GetItem(TaskNumber - 1);
            Console.WriteLine(item.GetFullInfo());
        }
    }
}
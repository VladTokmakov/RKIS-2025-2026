using System;
namespace Todolist
{
    public class DeleteCommand : ICommand
    {
        public int TaskNumber { get; private set; }
        public Todolist TodoList { get; private set; }

        public DeleteCommand(Todolist todoList, int taskNumber)
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
            string taskText = item.Text;
            TodoList.Delete(TaskNumber - 1);
            Console.WriteLine($"Задача №{TaskNumber} '{taskText}' удалена");
        }
    }
}
using System;
namespace Todolist
{
    public class DeleteCommand : ICommand
    {
        public int TaskNumber { get; private set; }
        public Todolist TodoList { get; private set; }
        private readonly string TodoFilePath;
        private TodoItem _deletedItem;
        private int _deletedIndex;

        public DeleteCommand(Todolist todoList, int taskNumber, string todoFilePath = null)
        {
            TodoList = todoList;
            TaskNumber = taskNumber;
            TodoFilePath = todoFilePath;
            _deletedIndex = taskNumber - 1;
        }

        public void Execute()
        {
            _deletedItem = TodoList.GetItem(_deletedIndex);
            string taskText = _deletedItem.Text;
            TodoList.Delete(_deletedIndex);
            Console.WriteLine($"Задача №{TaskNumber} '{taskText}' удалена");
        
            if (!string.IsNullOrEmpty(TodoFilePath)) FileManager.SaveTodos(TodoList, TodoFilePath);
        }

        public void Unexecute()
        {
            if (_deletedItem != null)
            {
                TodoList.AddAt(_deletedIndex, _deletedItem);
                Console.WriteLine($"Отменено удаление задачи №{TaskNumber}: {_deletedItem.Text}");
                if (!string.IsNullOrEmpty(TodoFilePath)) FileManager.SaveTodos(TodoList, TodoFilePath);
            }
        }
    }
}
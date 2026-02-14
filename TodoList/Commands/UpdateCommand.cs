using System;
namespace Todolist
{
    public class UpdateCommand : ICommand
    {
        public int TaskNumber { get; private set; }
        public string NewText { get; private set; }
        public Todolist TodoList { get; private set; }
        private readonly string TodoFilePath;

        public UpdateCommand(Todolist todoList, int taskNumber, string newText, string todoFilePath = null)
        {
            TodoList = todoList;
            TaskNumber = taskNumber;
            NewText = newText;
            TodoFilePath = todoFilePath;
        }

        public void Execute()
        {
            TodoItem item = TodoList.GetItem(TaskNumber - 1);
            string oldText = item.Text;

            if (NewText.StartsWith("\"") && NewText.EndsWith("\""))
            {
                NewText = NewText.Substring(1, NewText.Length - 2);
            }

            item.UpdateText(NewText);
            Console.WriteLine($"Обновил задачу: \nБыло: Задача №{TaskNumber} \"{oldText}\" \nСтало: Задача №{TaskNumber} \"{NewText}\"");
        
            if (!string.IsNullOrEmpty(TodoFilePath)) FileManager.SaveTodos(TodoList, TodoFilePath);
        }
    }
}
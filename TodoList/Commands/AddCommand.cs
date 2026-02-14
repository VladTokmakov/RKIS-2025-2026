using System;
namespace Todolist
{
    public class AddCommand : ICommand
    {
        public bool IsMultiline { get; private set; }
        public string TaskText { get; private set; }
        public Todolist TodoList { get; private set; }
        private readonly string TodoFilePath;

        public AddCommand(Todolist todoList, string taskText, bool isMultiline = false, string todoFilePath = null)
        {
            TodoList = todoList;
            TaskText = taskText;
            IsMultiline = isMultiline;
            TodoFilePath = todoFilePath;
        }

        public void Execute()
        {
            if (IsMultiline)
            {
                Console.WriteLine("Многострочный режим. Введите задачи (для завершения введите '!end'):");
                string multilineText = "";

                Console.Write("> ");
                string line = Console.ReadLine();
                while (line.ToLower() != "!end")
                {
                    Console.Write("> ");
                    multilineText += line + "\n";
                    line = Console.ReadLine();
                }

                TodoItem newItem = new TodoItem(multilineText.Trim());
                TodoList.Add(newItem);
                Console.WriteLine($"Добавлена задача №{TodoList.GetCount()}: {multilineText.Trim()}");
            }
            else
            {
                if (TaskText.StartsWith("\"") && TaskText.EndsWith("\""))
                {
                    TaskText = TaskText.Substring(1, TaskText.Length - 2);
                }

                TodoItem newItem = new TodoItem(TaskText);
                TodoList.Add(newItem);
                Console.WriteLine($"Добавлена задача №{TodoList.GetCount()}: {TaskText}");
            }

            if (!string.IsNullOrEmpty(TodoFilePath)) FileManager.SaveTodos(TodoList, TodoFilePath);
        }
    }
}
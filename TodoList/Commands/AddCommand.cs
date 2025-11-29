using System;
using System.Collections.Generic;

namespace Todolist
{
    public class AddCommand : ICommand
    {
        public bool IsMultiline { get; private set; }
        public string TaskText { get; private set; }
        public Todolist TodoList { get; private set; }
        private readonly string TodoFilePath;
        private TodoItem _addedItem;
        private int _addedIndex;

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
                    multilineText += line + "\n";
                    Console.Write("> ");
                    line = Console.ReadLine();
                }

                _addedItem = new TodoItem(multilineText.Trim());
                TodoList.Add(_addedItem);
                _addedIndex = TodoList.GetCount() - 1;
                Console.WriteLine($"Добавлена задача №{TodoList.GetCount()}: {multilineText.Trim()}");
            }
            else
            {
                if (TaskText.StartsWith("\"") && TaskText.EndsWith("\""))
                {
                    TaskText = TaskText.Substring(1, TaskText.Length - 2);
                }

                _addedItem = new TodoItem(TaskText);
                TodoList.Add(_addedItem);
                _addedIndex = TodoList.GetCount() - 1;
                Console.WriteLine($"Добавлена задача №{TodoList.GetCount()}: {TaskText}");
            }

            if (!string.IsNullOrEmpty(TodoFilePath)) FileManager.SaveTodos(TodoList, TodoFilePath);
        }

        public void Unexecute()
        {
            if (_addedItem != null && _addedIndex >= 0 && _addedIndex < TodoList.GetCount())
            {
                TodoList.Delete(_addedIndex);
                Console.WriteLine($"Отменено добавление задачи: {_addedItem.Text}");
                if (!string.IsNullOrEmpty(TodoFilePath)) FileManager.SaveTodos(TodoList, TodoFilePath);
            }
        }
    }
}
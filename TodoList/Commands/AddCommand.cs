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
        private string _addedItemText;

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
                _addedItemText = multilineText.Trim();
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
                _addedItemText = TaskText;
                TodoList.Add(_addedItem);
                _addedIndex = TodoList.GetCount() - 1;
                Console.WriteLine($"Добавлена задача №{TodoList.GetCount()}: {TaskText}");
            }

            if (!string.IsNullOrEmpty(TodoFilePath)) FileManager.SaveTodos(TodoList, TodoFilePath);
        }

        public void Unexecute()
        {
            if (!string.IsNullOrEmpty(_addedItemText))
            {
                for (int i = TodoList.GetCount() - 1; i >= 0; i--)
                {
                    var item = TodoList.GetItem(i);
                    if (item.Text == _addedItemText)
                    {
                        TodoList.Delete(i);
                        Console.WriteLine($"Отменено добавление задачи: {_addedItemText}");
                        if (!string.IsNullOrEmpty(TodoFilePath)) FileManager.SaveTodos(TodoList, TodoFilePath);
                        return;
                    }
                }
                Console.WriteLine("Не удалось найти задачу для отмены добавления");
            }
        }
    }
}
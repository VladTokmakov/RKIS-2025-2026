using System;

namespace Todolist
{
    public class AddCommand : ICommand
    {
        public bool IsMultiline { get; private set; }
        public string TaskText { get; private set; }
        public Todolist TodoList { get; private set; }
        private readonly string TodoFilePath;
        private TodoItem _addedItem;

        public AddCommand(Todolist todoList, string taskText, bool isMultiline = false, string todoFilePath = null)
        {
            TodoList = todoList;
            TaskText = taskText;
            IsMultiline = isMultiline;
            TodoFilePath = todoFilePath;
        }

        public void Execute()
        {
            if (TodoList == null)
            {
                Console.WriteLine("Ошибка: нет активного списка задач.");
                return;
            }

            string finalText;

            if (IsMultiline)
            {
                finalText = ReadMultiline();
            }
            else
            {
                finalText = TaskText;
                if (finalText.StartsWith("\"") && finalText.EndsWith("\""))
                {
                    finalText = finalText.Substring(1, finalText.Length - 2);
                }
            }

            if (string.IsNullOrWhiteSpace(finalText))
            {
                Console.WriteLine("Текст задачи не может быть пустым.");
                return;
            }

            _addedItem = new TodoItem(finalText);
            TodoList.Add(_addedItem);
            AppInfo.UndoStack.Push(this);
            AppInfo.RedoStack.Clear();
            Console.WriteLine($"Добавлена задача №{TodoList.GetCount()}: {finalText}");

            if (!string.IsNullOrEmpty(TodoFilePath)) 
                FileManager.SaveTodos(TodoList, TodoFilePath);
        }

        public void Unexecute()
        {
            if (_addedItem != null && TodoList != null)
            {
                int lastIndex = TodoList.GetCount();

                if (lastIndex > 0)
                {
                    TodoList.Delete(lastIndex - 1);
                    Console.WriteLine("Добавление задачи отменено.");
                    
                    if (!string.IsNullOrEmpty(TodoFilePath)) 
                        FileManager.SaveTodos(TodoList, TodoFilePath);
                }
            }
        }

        private string ReadMultiline()
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
            return multilineText.Trim();
        }
    }
}
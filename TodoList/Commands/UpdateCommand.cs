using System;

namespace Todolist
{
    public class UpdateCommand : ICommand
    {
        public int TaskNumber { get; private set; }
        public string NewText { get; private set; }
        public Todolist TodoList { get; private set; }
        private readonly string TodoFilePath;
        private string _oldText;

        public UpdateCommand(Todolist todoList, int taskNumber, string newText, string todoFilePath = null)
        {
            TodoList = todoList;
            TaskNumber = taskNumber;
            NewText = newText;
            TodoFilePath = todoFilePath;
        }

        public void Execute()
        {
            if (TodoList == null)
            {
                Console.WriteLine("Ошибка: нет активного списка задач.");
                return;
            }

            int index = TaskNumber - 1;
            if (index < 0 || index >= TodoList.GetCount())
            {
                Console.WriteLine("Задача с таким номером не найдена.");
                return;
            }

            TodoItem item = TodoList.GetItem(index);
            _oldText = item.Text;
            string finalText = NewText;
            if (finalText.StartsWith("\"") && finalText.EndsWith("\""))
            {
                finalText = finalText.Substring(1, finalText.Length - 2);
            }

            if (string.IsNullOrWhiteSpace(finalText))
            {
                Console.WriteLine("Текст задачи не может быть пустым.");
                return;
            }

            item.UpdateText(finalText);
            AppInfo.UndoStack.Push(this);
            AppInfo.RedoStack.Clear();
            Console.WriteLine($"Обновил задачу: \nБыло: Задача №{TaskNumber} \"{_oldText}\" \nСтало: Задача №{TaskNumber} \"{finalText}\"");

            if (!string.IsNullOrEmpty(TodoFilePath)) 
                FileManager.SaveTodos(TodoList, TodoFilePath);
        }

        public void Unexecute()
        {
            if (TodoList != null)
            {
                int index = TaskNumber - 1;
                if (index >= 0 && index < TodoList.GetCount())
                {
                    TodoList.GetItem(index).UpdateText(_oldText);
                    Console.WriteLine($"Текст задачи №{TaskNumber} возвращен к предыдущему значению.");
    
                    if (!string.IsNullOrEmpty(TodoFilePath)) 
                        FileManager.SaveTodos(TodoList, TodoFilePath);
                }
            }
        }
    }
}
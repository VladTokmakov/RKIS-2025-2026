using System;

namespace Todolist
{
    public class DeleteCommand : IUndo
    {
        public int TaskNumber { get; private set; }
        public Todolist TodoList { get; private set; }
        private readonly string TodoFilePath;
        private TodoItem _deletedItem;
        private int _actualIndex;

        public DeleteCommand(Todolist todoList, int taskNumber, string todoFilePath = null)
        {
            TodoList = todoList;
            TaskNumber = taskNumber;
            TodoFilePath = todoFilePath;
        }

        public void Execute()
        {
            if (TodoList == null)
            {
                Console.WriteLine("Ошибка: нет активного списка задач.");
                return;
            }

            _actualIndex = TaskNumber - 1;
            if (_actualIndex < 0 || _actualIndex >= TodoList.GetCount())
            {
                Console.WriteLine("Задача с таким номером не найдена.");
                return;
            }

            _deletedItem = TodoList.GetItem(_actualIndex);
            string taskText = _deletedItem.Text;
            TodoList.Delete(_actualIndex);
            AppInfo.UndoStack.Push(this);
            AppInfo.RedoStack.Clear();
            Console.WriteLine($"Задача №{TaskNumber} '{taskText}' удалена");

            if (!string.IsNullOrEmpty(TodoFilePath)) 
                FileManager.SaveTodos(TodoList, TodoFilePath);
        }

        public void Unexecute()
        {
            if (_deletedItem != null && TodoList != null)
            {
                var items = new System.Collections.Generic.List<TodoItem>();
                for (int i = 0; i < TodoList.GetCount(); i++)
                {
                    items.Add(TodoList.GetItem(i));
                }
                
                if (_actualIndex <= items.Count)
                {
                    items.Insert(_actualIndex, _deletedItem);
                    
                    while (TodoList.GetCount() > 0)
                    {
                        TodoList.Delete(0);
                    }
                    
                    foreach (var item in items)
                    {
                        TodoList.Add(item);
                    }
                    
                    Console.WriteLine("Удаление задачи отменено.");
                    
                    if (!string.IsNullOrEmpty(TodoFilePath)) 
                        FileManager.SaveTodos(TodoList, TodoFilePath);
                }
            }
        }
    }
}
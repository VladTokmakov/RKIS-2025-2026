using System;
using Todolist.Exceptions;
using Todolist.Models;

namespace Todolist
{
    public class UpdateCommand : IUndo
    {
        public int TaskNumber { get; private set; }
        public string NewText { get; private set; }
        public TodoList TodoList { get; private set; }
        
        private string _oldText = string.Empty;
        private Guid _taskId;

        public UpdateCommand(TodoList todoList, int taskNumber, string newText)
        {
            TodoList = todoList;
            TaskNumber = taskNumber;
            NewText = newText;
        }

        public void Execute()
        {
            try
            {
                if (TodoList == null)
                    throw new BusinessLogicException("Ошибка: нет активного списка задач.");
                
                if (TaskNumber < 1 || TaskNumber > TodoList.GetCount())
                    throw new TaskNotFoundException($"Задача с номером {TaskNumber} не найдена.");
                
                TodoItem item = TodoList.GetItem(TaskNumber);
                _oldText = item.Text;
                _taskId = item.Id;
                
                string finalText = NewText;
                if (finalText.StartsWith("\"") && finalText.EndsWith("\""))
                    finalText = finalText.Substring(1, finalText.Length - 2);
                
                if (string.IsNullOrWhiteSpace(finalText))
                    throw new InvalidArgumentException("Текст задачи не может быть пустым.");
                
                item.UpdateText(finalText);
                AppInfo.TodoRepo.Update(item);
                
                AppInfo.UndoStack.Push(this);
                AppInfo.RedoStack.Clear();
                Console.WriteLine($"Обновлена задача: \nБыло: Задача №{TaskNumber} \"{_oldText}\" \nСтало: Задача №{TaskNumber} \"{finalText}\"");
            }
            catch (Exception ex) when (!(ex is TaskNotFoundException || ex is InvalidArgumentException))
            {
                throw;
            }
        }

        public void Unexecute()
        {
            try
            {
                if (TodoList != null && TaskNumber >= 1 && TaskNumber <= TodoList.GetCount())
                {
                    TodoList.GetItem(TaskNumber).UpdateText(_oldText);
                    var item = TodoList.GetItem(TaskNumber);
                    item.Text = _oldText;
                    AppInfo.TodoRepo.Update(item);
                    Console.WriteLine($"Текст задачи №{TaskNumber} возвращен к предыдущему значению.");
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicException($"Ошибка при отмене обновления: {ex.Message}");
            }
        }
    }
}
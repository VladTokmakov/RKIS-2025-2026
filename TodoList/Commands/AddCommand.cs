using System;
using Todolist.Exceptions;
using Todolist.Models;

namespace Todolist
{
    public class AddCommand : IUndo
    {
        public bool IsMultiline { get; private set; }
        public string TaskText { get; private set; }
        public TodoList TodoList { get; private set; }
        
        private TodoItem? _addedItem;
        private int _addedIndex;

        public AddCommand(TodoList todoList, string taskText, bool isMultiline = false)
        {
            TodoList = todoList;
            TaskText = taskText;
            IsMultiline = isMultiline;
        }

        public void Execute()
        {
            try
            {
                if (TodoList == null)
                    throw new BusinessLogicException("Ошибка: нет активного списка задач.");

                string finalText;
                if (IsMultiline)
                {
                    finalText = ReadMultiline();
                }
                else
                {
                    finalText = TaskText;
                }

                if (string.IsNullOrWhiteSpace(finalText))
                    throw new InvalidArgumentException("Текст задачи не может быть пустым.");

                _addedItem = new TodoItem(finalText);
                TodoList.Add(_addedItem);
                _addedIndex = TodoList.GetCount();
                
                AppInfo.TodoRepo.Add(_addedItem, AppInfo.CurrentProfileId);
                
                AppInfo.UndoStack.Push(this);
                AppInfo.RedoStack.Clear();
                Console.WriteLine($"Добавлена задача №{_addedIndex}: {finalText}");
            }
            catch (Exception ex) when (!(ex is InvalidArgumentException || ex is BusinessLogicException))
            {
                throw;
            }
        }

        public void Unexecute()
        {
            try
            {
                if (_addedItem != null && TodoList != null)
                {
                    TodoList.Delete(_addedIndex);
                    AppInfo.TodoRepo.Delete(_addedItem.Id);
                    Console.WriteLine("Добавление задачи отменено.");
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicException($"Ошибка при отмене добавления задачи: {ex.Message}");
            }
        }

        private string ReadMultiline()
        {
            Console.WriteLine("Многострочный режим. Введите задачи (для завершения введите '!end'):");
            string multilineText = "";
            while (true)
            {
                Console.Write("> ");
                string? line = Console.ReadLine();
                if (line == null)
                    continue;
                if (line.ToLower() == "!end")
                    break;
                multilineText += line + "\n";
            }
            return multilineText.Trim();
        }
    }
}
using System;
using Todolist.Exceptions;

namespace Todolist
{
    public class ReadCommand : ICommand
    {
        public int TaskNumber { get; private set; }
        public TodoList TodoList { get; private set; }

        public ReadCommand(TodoList todoList, int taskNumber)
        {
            TodoList = todoList;
            TaskNumber = taskNumber;
        }

        public void Execute()
        {
            try
            {
                if (TodoList == null)
                {
                    Console.WriteLine("Ошибка: список задач не инициализирован");
                    return;
                }

                if (TaskNumber < 1)
                {
                    Console.WriteLine("Номер задачи должен быть положительным числом");
                    return;
                }
                                
                if (TaskNumber > TodoList.GetCount())
                {
                    Console.WriteLine($"Задача с номером {TaskNumber} не найдена");
                    return;
                }

                TodoItem item = TodoList.GetItem(TaskNumber);
                Console.WriteLine(item.GetFullInfo());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении задачи: {ex.Message}");
            }
        }
    }
}
using System;

namespace Todolist
{
    public class ViewCommand : ICommand
    {
        public bool ShowIndex { get; private set; }
        public bool ShowStatus { get; private set; }
        public bool ShowDate { get; private set; }
        public bool ShowAll { get; private set; }
        public TodoList TodoList { get; private set; }

        public ViewCommand(TodoList todoList, bool showIndex = false, bool showStatus = false, bool showDate = false, bool showAll = false)
        {
            TodoList = todoList;
            ShowIndex = showIndex;
            ShowStatus = showStatus;
            ShowDate = showDate;
            ShowAll = showAll;
        }

        public void Execute()
        {
            if (TodoList == null)
            {
                Console.WriteLine("Ошибка: список задач не инициализирован");
                return;
            }

            if (TodoList.GetCount() == 0)
            {
                Console.WriteLine("Задачи отсутствуют");
                return;
            }

            bool finalShowIndex = ShowIndex || ShowAll;
            bool finalShowStatus = ShowStatus || ShowAll;
            bool finalShowDate = ShowDate || ShowAll;

            try
            {
                TodoList.View(finalShowIndex, finalShowStatus, finalShowDate);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при отображении задач: {ex.Message}");
            }
        }
    }
}
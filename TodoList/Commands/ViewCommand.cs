using System;
namespace Todolist
{
    public class ViewCommand : ICommand
    {
        public bool ShowIndex { get; private set; }
        public bool ShowStatus { get; private set; }
        public bool ShowDate { get; private set; }
        public bool ShowAll { get; private set; }
        public Todolist TodoList { get; private set; }

        public ViewCommand(Todolist todoList, bool showIndex = false, bool showStatus = false, bool showDate = false, bool showAll = false)
        {
            TodoList = todoList;
            ShowIndex = showIndex;
            ShowStatus = showStatus;
            ShowDate = showDate;
            ShowAll = showAll;
        }

        public void Execute()
        {
            if (TodoList.GetCount() == 0)
            {
                Console.WriteLine("Задачи отсутствуют");
                return;
            }

            bool showIndex = ShowIndex || ShowAll;
            bool showStatus = ShowStatus || ShowAll;
            bool showDate = ShowDate || ShowAll;

            TodoList.View(showIndex, showStatus, showDate);
        }

        public void Unexecute()
        {
        }
    }
}
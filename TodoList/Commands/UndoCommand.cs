using System;

namespace Todolist
{
    public class UndoCommand : ICommand
    {
        public void Execute()
        {
            if (AppInfo.UndoStack.Count > 0)
            {
                var command = AppInfo.UndoStack.Pop();
                command.Unexecute();
                AppInfo.RedoStack.Push(command);
                Console.WriteLine("Отменено последнее действие.");
            }
            else
            {
                Console.WriteLine("Нет действий для отмены.");
            }
        }
    }
}
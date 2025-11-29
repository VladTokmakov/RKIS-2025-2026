using System;

namespace Todolist
{
    public class UndoCommand : ICommand
    {
        public void Execute()
        {
            if (AppInfo.UndoStack.Count > 0)
            {
                ICommand command = AppInfo.UndoStack.Pop();
                command.Unexecute();
                AppInfo.RedoStack.Push(command);
                Console.WriteLine("Команда отменена");
            }
            else
            {
                Console.WriteLine("Нет команд для отмены");
            }
        }
    }
}
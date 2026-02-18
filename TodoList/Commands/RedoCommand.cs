using System;

namespace Todolist
{
    public class RedoCommand : ICommand
    {
        public void Execute()
        {
            if (AppInfo.RedoStack.Count > 0)
            {
                var command = AppInfo.RedoStack.Pop();
                command.Execute();
                Console.WriteLine("Повторено последнее отмененное действие.");
            }
            else
            {
                Console.WriteLine("Нет действий для повтора.");
            }
        }
    }
}
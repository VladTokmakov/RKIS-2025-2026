using System;
namespace Todolist
{
    public class ExitCommand : ICommand
    {
        public void Execute()
        {
            Environment.Exit(0);
        }

        public void Unexecute()
        {
        }
    }
}
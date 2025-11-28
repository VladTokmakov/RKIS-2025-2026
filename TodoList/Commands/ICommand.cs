namespace Todolist
{
    public interface ICommand
    {
        void Execute();
        void Unexecute();
    }
}
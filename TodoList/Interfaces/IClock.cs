using System;

namespace Todolist.Interfaces
{
    public interface IClock
    {
        DateTime Now { get; }
    }
}
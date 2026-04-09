using System;
using Todolist.Interfaces;

namespace Todolist
{
    public class SystemClock : IClock
    {
        public DateTime Now => DateTime.Now;
    }
}
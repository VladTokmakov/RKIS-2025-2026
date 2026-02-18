using System.Collections.Generic;

namespace Todolist
{
    public static class AppInfo
    {
        public static Todolist Todos { get; set; } = new Todolist();
        public static Profile CurrentProfile { get; set; }
        public static Stack<IUndo> UndoStack { get; set; } = new Stack<IUndo>();
        public static Stack<IUndo> RedoStack { get; set; } = new Stack<IUndo>();
        public static bool ShouldLogout { get; set; }
    }
}
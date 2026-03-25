using System;
using System.Collections.Generic;
using Todolist.Commands;

namespace Todolist
{
    public static class AppInfo
    {
        public static IDataStorage? Storage { get; set; }
        public static Dictionary<Guid, TodoList> TodosByProfile { get; } = new Dictionary<Guid, TodoList>();
        
        private static TodoList _currentTodos = new TodoList();
        
        public static TodoList Todos
        {
            get => _currentTodos;
            set
            {
                _currentTodos = value ?? new TodoList();
                if (CurrentProfileId != Guid.Empty)
                {
                    TodosByProfile[CurrentProfileId] = _currentTodos;
                }
            }
        }
        
        public static List<Profile> Profiles { get; set; } = new List<Profile>();
        public static Guid CurrentProfileId { get; set; }
        
        public static Profile? CurrentProfile
        {
            get
            {
                if (CurrentProfileId == Guid.Empty)
                    return null;
                return Profiles.Find(p => p.Id == CurrentProfileId);
            }
            set
            {
                if (value == null) 
                {
                    CurrentProfileId = Guid.Empty;
                    return;
                }
                var existing = Profiles.Find(p => p.Id == value.Id);
                if (existing == null)
                {
                    Profiles.Add(value);
                }
                CurrentProfileId = value.Id;
                if (!TodosByProfile.TryGetValue(CurrentProfileId, out var list))
                {
                    list = new TodoList();
                    TodosByProfile[CurrentProfileId] = list;
                }
                _currentTodos = list;
            }
        }
        
        public static Stack<IUndo> UndoStack { get; set; } = new Stack<IUndo>();
        public static Stack<IUndo> RedoStack { get; set; } = new Stack<IUndo>();
        public static bool ShouldLogout { get; set; }
    }
}
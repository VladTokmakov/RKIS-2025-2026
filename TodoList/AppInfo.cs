using System;
using System.Collections.Generic;
using System.Linq;
using Todolist.Commands;
using Todolist.Models;
using Todolist.Services;

namespace Todolist
{
    public static class AppInfo
    {
        public static IDataStorage? Storage { get; set; }
        
        private static TodoList _currentTodos = new TodoList();
        
        public static TodoList Todos
        {
            get => _currentTodos;
            set => _currentTodos = value ?? new TodoList();
        }
        
        public static List<Profile> Profiles { get; set; } = new List<Profile>();
        public static Guid CurrentProfileId { get; set; }
        
        public static ProfileRepository ProfileRepo { get; set; } = new ProfileRepository();
        public static TodoRepository TodoRepo { get; set; } = new TodoRepository();
        
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
                    _currentTodos = new TodoList();
                    return;
                }
                
                var existing = Profiles.Find(p => p.Id == value.Id);
                if (existing == null)
                {
                    Profiles.Add(value);
                }
                else
                {
                    existing.Login = value.Login;
                    existing.Password = value.Password;
                    existing.FirstName = value.FirstName;
                    existing.LastName = value.LastName;
                    existing.BirthYear = value.BirthYear;
                }
                
                CurrentProfileId = value.Id;
                
                try
                {
                    var todoItems = TodoRepo.GetAll(CurrentProfileId);
                    Console.WriteLine($"[AppInfo] Загружено задач из БД: {todoItems.Count}");
                    _currentTodos = new TodoList(todoItems);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[AppInfo] Ошибка загрузки задач: {ex.Message}");
                    _currentTodos = new TodoList();
                }
            }
        }
        
        public static Stack<IUndo> UndoStack { get; set; } = new Stack<IUndo>();
        public static Stack<IUndo> RedoStack { get; set; } = new Stack<IUndo>();
        public static bool ShouldLogout { get; set; }
        
        public static void SaveCurrentTodos()
        {
            if (CurrentProfileId != Guid.Empty && Todos != null)
            {
                var items = new List<TodoItem>();
                for (int i = 0; i < Todos.GetCount(); i++)
                {
                    items.Add(Todos.GetItem(i + 1));
                }
                Console.WriteLine($"Сохранение задач в БД: {items.Count} задач");
                TodoRepo.ReplaceAll(CurrentProfileId, items);
            }
        }
        
        public static void SaveAllProfiles()
        {
            Console.WriteLine($"Сохранение профилей в БД: {Profiles.Count} профилей");
            foreach (var profile in Profiles)
            {
                var existing = ProfileRepo.GetById(profile.Id);
                if (existing == null)
                    ProfileRepo.Add(profile);
                else
                    ProfileRepo.Update(profile);
            }
        }
        
        public static void ReloadTodos()
        {
            if (CurrentProfileId != Guid.Empty)
            {
                var todoItems = TodoRepo.GetAll(CurrentProfileId);
                Console.WriteLine($"[AppInfo] Перезагрузка задач: {todoItems.Count} задач");
                _currentTodos = new TodoList(todoItems);
            }
        }
    }
}
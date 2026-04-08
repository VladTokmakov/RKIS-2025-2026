using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Todolist.Data;
using Todolist.Models;

namespace Todolist.Services
{
    public class TodoRepository
    {
        public List<TodoItem> GetAll(Guid profileId)
        {
            using var context = new AppDbContext();
            return context.Todos
                .Where(t => t.ProfileId == profileId)
                .OrderBy(t => t.SortOrder)
                .AsNoTracking()
                .ToList();
        }

        public TodoItem? GetById(Guid id)
        {
            using var context = new AppDbContext();
            return context.Todos.AsNoTracking().FirstOrDefault(t => t.Id == id);
        }

        public void Add(TodoItem item, Guid profileId)
        {
            using var context = new AppDbContext();
            
            int maxSortOrder = context.Todos
                .Where(t => t.ProfileId == profileId)
                .Max(t => (int?)t.SortOrder) ?? 0;
            
            item.Id = Guid.NewGuid();
            item.ProfileId = profileId;
            item.SortOrder = maxSortOrder + 1;
            item.LastUpdate = DateTime.Now;
            
            context.Todos.Add(item);
            context.SaveChanges();
        }

        public void Update(TodoItem item)
        {
            using var context = new AppDbContext();
            var existing = context.Todos.FirstOrDefault(t => t.Id == item.Id);
            if (existing != null)
            {
                existing.Text = item.Text;
                existing.Status = item.Status;
                existing.LastUpdate = item.LastUpdate;
                context.SaveChanges();
            }
        }

        public void Delete(Guid id)
        {
            using var context = new AppDbContext();
            var item = context.Todos.FirstOrDefault(t => t.Id == id);
            if (item != null)
            {
                context.Todos.Remove(item);
                context.SaveChanges();
                
                var remainingTodos = context.Todos
                    .Where(t => t.ProfileId == item.ProfileId)
                    .OrderBy(t => t.SortOrder)
                    .ToList();
                
                for (int i = 0; i < remainingTodos.Count; i++)
                {
                    remainingTodos[i].SortOrder = i + 1;
                }
                context.SaveChanges();
            }
        }

        public void SetStatus(Guid id, TodoStatus status)
        {
            using var context = new AppDbContext();
            var item = context.Todos.FirstOrDefault(t => t.Id == id);
            if (item != null)
            {
                item.Status = status;
                item.LastUpdate = DateTime.Now;
                context.SaveChanges();
            }
        }

        public void ReplaceAll(Guid profileId, IEnumerable<TodoItem> todos)
        {
            using var context = new AppDbContext();
            
            var oldTodos = context.Todos.Where(t => t.ProfileId == profileId);
            context.Todos.RemoveRange(oldTodos);
            
            int order = 1;
            foreach (var todo in todos)
            {
                todo.Id = Guid.NewGuid();
                todo.ProfileId = profileId;
                todo.SortOrder = order++;
                context.Todos.Add(todo);
            }
            
            context.SaveChanges();
        }

        public void UpdateSortOrder(Guid profileId, List<Guid> orderedIds)
        {
            using var context = new AppDbContext();
            var todos = context.Todos.Where(t => t.ProfileId == profileId).ToList();
            
            for (int i = 0; i < orderedIds.Count; i++)
            {
                var todo = todos.FirstOrDefault(t => t.Id == orderedIds[i]);
                if (todo != null)
                {
                    todo.SortOrder = i + 1;
                }
            }
            context.SaveChanges();
        }
    }
}
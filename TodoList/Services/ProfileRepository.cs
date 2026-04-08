using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Todolist.Data;
using Todolist.Models;

namespace Todolist.Services
{
    public class ProfileRepository
    {
        public List<Profile> GetAll()
        {
            using var context = new AppDbContext();
            return context.Profiles
                .AsNoTracking()
                .OrderBy(p => p.Login)
                .ToList();
        }

        public Profile? GetById(Guid id)
        {
            using var context = new AppDbContext();
            return context.Profiles.AsNoTracking().FirstOrDefault(p => p.Id == id);
        }

        public Profile? GetByLogin(string login)
        {
            using var context = new AppDbContext();
            return context.Profiles.AsNoTracking()
                .FirstOrDefault(p => p.Login.ToLower() == login.ToLower());
        }

        public void Add(Profile profile)
        {
            if (profile == null) throw new ArgumentNullException(nameof(profile));
            
            using var context = new AppDbContext();
            profile.Id = Guid.NewGuid();
            context.Profiles.Add(profile);
            context.SaveChanges();
        }

        public void Update(Profile profile)
        {
            if (profile == null) throw new ArgumentNullException(nameof(profile));
            
            using var context = new AppDbContext();
            var existing = context.Profiles.FirstOrDefault(p => p.Id == profile.Id);
            if (existing != null)
            {
                existing.Login = profile.Login;
                existing.Password = profile.Password;
                existing.FirstName = profile.FirstName;
                existing.LastName = profile.LastName;
                existing.BirthYear = profile.BirthYear;
                context.SaveChanges();
            }
        }

        public void Delete(Guid profileId)
        {
            using var context = new AppDbContext();
            var profile = context.Profiles.FirstOrDefault(p => p.Id == profileId);
            if (profile != null)
            {
                context.Profiles.Remove(profile);
                context.SaveChanges();
            }
        }

        public void ReplaceAll(IEnumerable<Profile> profiles)
        {
            using var context = new AppDbContext();
            
            context.Profiles.RemoveRange(context.Profiles);
            context.SaveChanges();
            
            foreach (var profile in profiles)
            {
                context.Profiles.Add(profile);
            }
            context.SaveChanges();
        }
    }
}
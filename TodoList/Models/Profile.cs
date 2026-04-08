using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Todolist.Models
{
    [Index(nameof(Login), IsUnique = true)]
    public class Profile
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(64)]
        public string Login { get; set; }

        [Required]
        [MaxLength(128)]
        public string Password { get; set; }

        [Required]
        [MaxLength(64)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(64)]
        public string LastName { get; set; }

        [Required]
        public int BirthYear { get; set; }

        public virtual ICollection<TodoItem> Todos { get; set; }

        public Profile()
        {
            Id = Guid.NewGuid();
            Login = string.Empty;
            Password = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            BirthYear = DateTime.Now.Year - 18;
            Todos = new List<TodoItem>();
        }

        public Profile(string firstName, string lastName, int birthYear) : this()
        {
            FirstName = firstName ?? string.Empty;
            LastName = lastName ?? string.Empty;
            BirthYear = birthYear;
        }

        public Profile(string login, string password, string firstName, string lastName, int birthYear) : this()
        {
            Login = login ?? string.Empty;
            Password = password ?? string.Empty;
            FirstName = firstName ?? string.Empty;
            LastName = lastName ?? string.Empty;
            BirthYear = birthYear;
        }

        public Profile(Guid id, string login, string password, string firstName, string lastName, int birthYear)
        {
            Id = id;
            Login = login ?? string.Empty;
            Password = password ?? string.Empty;
            FirstName = firstName ?? string.Empty;
            LastName = lastName ?? string.Empty;
            BirthYear = birthYear;
            Todos = new List<TodoItem>();
        }

        public string GetInfo()
        {
            int age = DateTime.Now.Year - BirthYear;
            if (!string.IsNullOrEmpty(Login))
                return $"{FirstName} {LastName}, возраст {age} (логин: {Login})";
            return $"{FirstName} {LastName}, возраст {age}";
        }
    }
}
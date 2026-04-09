using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Todolist.Interfaces;

namespace Todolist.Models
{
    public enum TodoStatus
    {
        NotStarted,
        InProgress,
        Completed,
        Postponed,
        Failed
    }

    public class TodoItem
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Text { get; set; }

        [Required]
        public TodoStatus Status { get; set; }

        [Required]
        public DateTime LastUpdate { get; set; }

        public int SortOrder { get; set; }
        public Guid ProfileId { get; set; }

        [ForeignKey("ProfileId")]
        public virtual Profile Profile { get; set; }

        private readonly IClock _clock;

        // Конструктор по умолчанию для EF Core
        public TodoItem() : this(new SystemClock())
        {
        }

        public TodoItem(IClock clock)
        {
            Id = Guid.NewGuid();
            Status = TodoStatus.NotStarted;
            LastUpdate = clock.Now;
            Text = string.Empty;
            SortOrder = 0;
            Profile = null!;
            _clock = clock;
        }

        public TodoItem(string text) : this(text, new SystemClock())
        {
        }

        public TodoItem(string text, IClock clock) : this(clock)
        {
            Text = text ?? string.Empty;
        }

        public TodoItem(string text, TodoStatus status, DateTime lastUpdate) : this(text, new SystemClock())
        {
            Status = status;
            LastUpdate = lastUpdate;
        }

        public void UpdateText(string newText)
        {
            Text = newText ?? string.Empty;
            LastUpdate = _clock.Now;
        }

        public void SetStatus(TodoStatus status, bool updateTime = true)
        {
            Status = status;
            if (updateTime)
            {
                LastUpdate = _clock.Now;
            }
        }

        public string GetShortInfo()
        {
            string taskText = Text.Replace("\n", " ");
            if (taskText.Length > 34)
                taskText = taskText.Substring(0, 30) + "... ";
            return taskText;
        }

        public string GetFullInfo()
        {
            return $"Задача: {Text}\nСтатус: {Status}\nДата изменения: {LastUpdate}";
        }
    }
}
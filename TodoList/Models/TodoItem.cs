using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        public TodoItem()
        {
            Id = Guid.NewGuid();
            Status = TodoStatus.NotStarted;
            LastUpdate = DateTime.Now;
            Text = string.Empty;
            SortOrder = 0;
            Profile = null!;
        }

        public TodoItem(string text) : this()
        {
            Text = text ?? string.Empty;
        }

        public TodoItem(string text, TodoStatus status, DateTime lastUpdate) : this()
        {
            Text = text ?? string.Empty;
            Status = status;
            LastUpdate = lastUpdate;
        }

        public void UpdateText(string newText)
        {
            Text = newText ?? string.Empty;
            LastUpdate = DateTime.Now;
        }

        public void SetStatus(TodoStatus status, bool updateTime = true)
        {
            Status = status;
            if (updateTime)
            {
                LastUpdate = DateTime.Now;
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
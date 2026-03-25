using System;

namespace Todolist
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
        public string Text { get; set; }
        public TodoStatus Status { get; set; }
        public DateTime LastUpdate { get; set; }

        public TodoItem(string text)
        {
            Text = text ?? string.Empty;
            Status = TodoStatus.NotStarted;
            LastUpdate = DateTime.Now;
        }

        public TodoItem(string text, TodoStatus status, DateTime lastUpdate)
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
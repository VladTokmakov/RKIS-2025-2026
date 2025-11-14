namespace Todolist
{
    public class TodoItem
    {
        public string Text { get; private set; }
        public TodoStatus Status { get; private set; }
        public DateTime LastUpdate { get; private set; }

        public TodoItem(string text)
        {
            Text = text;
            Status = TodoStatus.NotStarted;
            LastUpdate = DateTime.Now;
        }

        public TodoItem(string text, TodoStatus status, DateTime lastUpdate)
        {
            Text = text;
            Status = status;
            LastUpdate = lastUpdate;
        }

        public void SetLastUpdate(DateTime dateTime)
        {
            LastUpdate = dateTime;
        }

        public void SetStatus(TodoStatus status, bool updateTime = true)
        {
            Status = status;
            if (updateTime)
            {
                LastUpdate = DateTime.Now;
            }
        }

        public void UpdateText(string newText)
        {
            Text = newText;
            LastUpdate = DateTime.Now;
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
            return $"Задача: {Text}\nСтатус: {(IsDone ? "Выполнена" : "Не выполнена")}\nДата изменения: {LastUpdate}";
        }
    }
}
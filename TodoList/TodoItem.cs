namespace Todolist
{
    public class TodoItem
    {
        public string Text { get; private set; }
        public bool IsDone { get; private set; }
        public DateTime LastUpdate { get; private set; }

        public TodoItem(string text)
        {
            Text = text;
            IsDone = false;
            LastUpdate = DateTime.Now;
        }

        public void MarkDone()
        {
            IsDone = true;
            LastUpdate = DateTime.Now;
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
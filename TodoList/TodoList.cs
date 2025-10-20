namespace Todolist
{
    public class Todolist
    {
        private TodoItem[] items;
        private int count;

        public Todolist(int capacity = 2)
        {
            items = new TodoItem[capacity];
            count = 0;
        }

        public void Add(TodoItem item)
        {
            if (count >= items.Length)
            {
                IncreaseArray();
            }
            items[count] = item;
            count++;
        }

        public void Delete(int index)
        {
            for (int i = index; i < count - 1; i++)
            {
                items[i] = items[i + 1];
            }
            items[count - 1] = null;
            count--;
        }

        public void View(bool showIndex, bool showDone, bool showDate)
        {
            if (count == 0)
            {
                Console.WriteLine("Задачи отсутствуют");
                return;
            }

            string header = "";
            if (showIndex) header += "№       ";
            header += "Задача                            ";
            if (showDate) header += "Дата изменения        ";
            if (showDone) header += "Статус";

            Console.WriteLine(header);
            Console.WriteLine(new string('-', header.Length));

            for (int i = 0; i < count; i++)
            {
                string row = "";

                if (showIndex) row += $"{i + 1}       ".Substring(0, 8);

                string taskText = items[i].GetShortInfo();
                row += taskText + new string(' ', 34 - taskText.Length);

                if (showDate) row += $"{items[i].LastUpdate} ";

                if (showDone) row += $"{(items[i].IsDone ? "Выполнена" : "Не выполнена")}";

                Console.WriteLine(row);
            }
        }

        public TodoItem GetItem(int index)
        {
            return items[index];
        }

        public int GetCount()
        {
            return count;
        }

        private void IncreaseArray()
        {
            int newSize = items.Length * 2;
            TodoItem[] newArray = new TodoItem[newSize];
            
            for (int i = 0; i < items.Length; i++)
            {
                newArray[i] = items[i];
            }
            
            items = newArray;
            Console.WriteLine($"Массив увеличен до {newSize} элементов");
        }
    }
}
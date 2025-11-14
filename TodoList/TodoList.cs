using System;
using System.Collections;
using System.Collections.Generic;

namespace Todolist
{
    public class Todolist : IEnumerable<TodoItem>
    {
        private List<TodoItem> items;

        public Todolist()
        {
            items = new List<TodoItem>();
        }

        public void Add(TodoItem item)
        {
            items.Add(item);
        }

        public void Delete(int index)
        {
            if (index < 0 || index >= items.Count)
                throw new ArgumentOutOfRangeException();

            items.RemoveAt(index);
        }

        public void SetStatus(int index, TodoStatus status)
        {
            if (index < 0 || index >= items.Count)
                throw new ArgumentOutOfRangeException();

            items[index].SetStatus(status);
        }

        public void View(bool showIndex, bool showStatus, bool showDate)
        {
            if (items.Сount == 0)
            {
                Console.WriteLine("Задачи отсутствуют");
                return;
            }

            string header = "";
            if (showIndex) header += "№       ";
            header += "Задача                            ";
            if (showDate) header += "Дата изменения        ";
            if (showStatus) header += "Статус";

            Console.WriteLine(header);
            Console.WriteLine(new string('-', header.Length));

            for (int i = 0; i < items.Count; i++)
            {
                string row = "";

                if (showIndex) row += $"{i + 1}       ".Substring(0, 8);

                string taskText = items[i].GetShortInfo();
                row += taskText + new string(' ', 34 - taskText.Length);

                if (showDate) row += $"{items[i].LastUpdate} ";

                if (showStatus) row += $"{items[i].Status}";

                Console.WriteLine(row);
            }
        }

        public TodoItem GetItem(int index)
        {
            if (index < 0 || index >= items.Count)
                throw new ArgumentOutOfRangeException();

            return items[index];
        }

        public int GetCount()
        {
            return items.Count;
        }

        public TodoItem this[int index]
        {
            get
            {
                if (index < 0 || index >= items.Count)
                    throw new ArgumentOutOfRangeException();
                return items[index];
            }
        }

        public IEnumerator<TodoItem> GetEnumerator()
        {
            foreach (var item in items)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    }
}
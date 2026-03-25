using System;
using System.Collections;
using System.Collections.Generic;

namespace Todolist
{
    public class TodoList : IEnumerable<TodoItem>
    {
        private List<TodoItem> items;

        public TodoList()
        {
            items = new List<TodoItem>();
        }

        public TodoList(List<TodoItem> items)
        {
            this.items = items ?? new List<TodoItem>();
        }

        public void Add(TodoItem item)
        {
            items.Add(item);
        }

        public void Insert(int index, TodoItem item)
        {
            if (index < 1 || index > items.Count + 1)
                throw new Exception("Задача с таким индексом не существует.");
            
            int zeroBasedIndex = index - 1;
            items.Insert(zeroBasedIndex, item);
        }

        public void Delete(int index)
        {
            if (index < 1 || index > items.Count)
                throw new Exception("Задача с таким индексом не существует.");
            
            int zeroBasedIndex = index - 1;
            items.RemoveAt(zeroBasedIndex);
        }

        public void Update(int index, string newText)
        {
            if (index < 1 || index > items.Count)
                throw new Exception("Задача с таким индексом не существует.");
            
            int zeroBasedIndex = index - 1;
            items[zeroBasedIndex].UpdateText(newText);
        }

        public void SetStatus(int index, TodoStatus status)
        {
            if (index < 1 || index > items.Count)
                throw new Exception("Задача с таким индексом не существует.");
            
            int zeroBasedIndex = index - 1;
            items[zeroBasedIndex].SetStatus(status);
        }

        public TodoItem GetItem(int index)
        {
            if (index < 1 || index > items.Count)
                throw new Exception("Задача с таким индексом не существует.");
            
            return items[index - 1];
        }

        public int Count => items.Count;

        public int GetCount() => items.Count;

        public void View(bool showIndex, bool showStatus, bool showDate)
        {
            if (items.Count == 0)
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
                if (showDate) row += $"{items[i].LastUpdate:yyyy-MM-dd HH:mm} ";
                if (showStatus) row += $"{items[i].Status}";
                Console.WriteLine(row);
            }
        }

        public void Read(int index)
        {
            var item = GetItem(index);
            Console.WriteLine(item.GetFullInfo());
        }

        public IEnumerator<TodoItem> GetEnumerator()
        {
            foreach (var item in items)
                yield return item;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace Todolist
{
    public class SearchCommand : ICommand
    {
        private readonly SearchFlags _flags;

        public SearchCommand(SearchFlags flags)
        {
            _flags = flags ?? new SearchFlags();
        }

        public void Execute()
        {
            if (AppInfo.Todos == null || AppInfo.Todos.GetCount() == 0)
            {
                Console.WriteLine("Нет задач для поиска.");
                return;
            }

            var results = new List<TodoItem>();
            for (int i = 0; i < AppInfo.Todos.GetCount(); i++)
            {
                results.Add(AppInfo.Todos.GetItem(i));
            }

            var query = results.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(_flags.ContainsText))
            {
                query = query.Where(t => t.Text.Contains(_flags.ContainsText, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(_flags.StartsWithText))
            {
                query = query.Where(t => t.Text.StartsWith(_flags.StartsWithText, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(_flags.EndsWithText))
            {
                query = query.Where(t => t.Text.EndsWith(_flags.EndsWithText, StringComparison.OrdinalIgnoreCase));
            }

            if (_flags.FromDate.HasValue)
            {
                query = query.Where(t => t.LastUpdate >= _flags.FromDate.Value.Date);
            }

            if (_flags.ToDate.HasValue)
            {
                var toDate = _flags.ToDate.Value.Date.AddDays(1).AddSeconds(-1);
                query = query.Where(t => t.LastUpdate <= toDate);
            }

            if (_flags.Status.HasValue)
            {
                query = query.Where(t => t.Status == _flags.Status.Value);
            }

            if (!string.IsNullOrWhiteSpace(_flags.SortBy))
            {
                if (_flags.SortBy.Equals("text", StringComparison.OrdinalIgnoreCase))
                {
                    query = _flags.Descending
                        ? query.OrderByDescending(t => t.Text)
                        : query.OrderBy(t => t.Text);
                }
                else if (_flags.SortBy.Equals("date", StringComparison.OrdinalIgnoreCase))
                {
                    query = _flags.Descending
                        ? query.OrderByDescending(t => t.LastUpdate)
                        : query.OrderBy(t => t.LastUpdate);
                }
            }

            if (_flags.TopCount.HasValue && _flags.TopCount.Value > 0)
            {
                query = query.Take(_flags.TopCount.Value);
            }

            var searchResults = query.ToList();

            if (searchResults.Count == 0)
            {
                Console.WriteLine("Ничего не найдено.");
                return;
            }

            Console.WriteLine($"Найдено задач: {searchResults.Count}");
            Console.WriteLine();

            for (int i = 0; i < searchResults.Count; i++)
            {
                var item = searchResults[i];
                Console.WriteLine($"[{i + 1}] {item.GetShortInfo()}");
            }
        }

        public void Unexecute() { }
    }
}
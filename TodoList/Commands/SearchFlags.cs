using System;

namespace Todolist
{
    public class SearchFlags
    {
        public string ContainsText { get; set; }
        public string StartsWithText { get; set; }
        public string EndsWithText { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public TodoStatus? Status { get; set; }
        public string SortBy { get; set; }
        public bool Descending { get; set; }
        public int? TopCount { get; set; }
    }
}
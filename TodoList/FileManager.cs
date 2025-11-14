using System;
using System.IO;
namespace Todolist
{
    public static class FileManager
    {
        public static void EnsureDataDirectory(string dirPath)
        {
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
        }

        public static void SaveProfile(Profile profile, string filePath)
        {
            if (profile == null) return;
            
            string profileData = $"{profile.FirstName};{profile.LastName};{profile.BirthYear}";
            File.WriteAllText(filePath, profileData);
        }

        public static Profile LoadProfile(string filePath)
        {
            if (!File.Exists(filePath)) return null;

            string content = File.ReadAllText(filePath);
            string[] parts = content.Split(';');
            
            if (parts.Length == 3)
            {
                string firstName = parts[0];
                string lastName = parts[1];
                int birthYear = int.Parse(parts[2]);
                return new Profile(firstName, lastName, birthYear);
            }
            
            return null;
        }

        public static void SaveTodos(Todolist todos, string filePath)
        {
            string[] csvLines = new string[todos.GetCount()];
            
            for (int i = 0; i < todos.GetCount(); i++)
            {
                TodoItem item = todos.GetItem(i);
                string escapedText = EscapeCsv(item.Text);
                string line = $"{i + 1};{item.IsDone};{item.LastUpdate:yyyy-MM-ddTHH:mm:ss};{escapedText}";
                csvLines[i] = line;
            }
            File.WriteAllLines(filePath, csvLines);
        }

        public static Todolist LoadTodos(string filePath)
        {
            Todolist todos = new Todolist();
            if (!File.Exists(filePath)) return todos;
            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                string[] parts = line.Split(';');

                if (parts.Length >= 4)
                {
                    int index = int.Parse(parts[0]);
                    bool isDone = bool.Parse(parts[1]);
                    DateTime lastUpdate = DateTime.Parse(parts[2]);

                    string text = string.Join(";", parts, 3, parts.Length - 3);
                    text = UnescapeCsv(text);

                    TodoItem item = new TodoItem(text, isDone, lastUpdate);
                    todos.Add(item);
                }
            }
            todos.FinishStartedWork();
            return todos;
        }

        private static string EscapeCsv(string text)
        {
            return "\"" + text.Replace("\"", "\"\"").Replace("\n", "\\n") + "\"";
        }

        private static string UnescapeCsv(string text)
        {
            return text.Trim('"').Replace("\\n", "\n").Replace("\"\"", "\"");
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Todolist.Interfaces;
using Todolist.Models;

namespace Todolist.Data
{
    public class FileStorage : IDataStorage
    {
        private readonly string _dataDirectory;
        private readonly IFileSystem _fileSystem;

        public FileStorage(string dataDirectory) : this(dataDirectory, new RealFileSystem())
        {
        }

        public FileStorage(string dataDirectory, IFileSystem fileSystem)
        {
            _dataDirectory = dataDirectory;
            _fileSystem = fileSystem;
            _fileSystem.CreateDirectory(_dataDirectory);
        }

        public void SaveProfiles(IEnumerable<Profile> profiles)
        {
            var profilesList = profiles.ToList();
            var json = JsonSerializer.Serialize(profilesList);
            var path = Path.Combine(_dataDirectory, "profiles.json");
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            _fileSystem.WriteAllBytes(path, bytes);
        }

        public IEnumerable<Profile> LoadProfiles()
        {
            var path = Path.Combine(_dataDirectory, "profiles.json");
            if (!_fileSystem.FileExists(path))
                return new List<Profile>();

            var bytes = _fileSystem.ReadAllBytes(path);
            var json = System.Text.Encoding.UTF8.GetString(bytes);
            return JsonSerializer.Deserialize<List<Profile>>(json) ?? new List<Profile>();
        }

        public void SaveTodos(Guid userId, IEnumerable<TodoItem> todos)
        {
            var todosList = todos.ToList();
            var json = JsonSerializer.Serialize(todosList);
            var path = Path.Combine(_dataDirectory, $"todos_{userId}.json");
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            _fileSystem.WriteAllBytes(path, bytes);
        }

        public IEnumerable<TodoItem> LoadTodos(Guid userId)
        {
            var path = Path.Combine(_dataDirectory, $"todos_{userId}.json");
            if (!_fileSystem.FileExists(path))
                return new List<TodoItem>();

            var bytes = _fileSystem.ReadAllBytes(path);
            var json = System.Text.Encoding.UTF8.GetString(bytes);
            return JsonSerializer.Deserialize<List<TodoItem>>(json) ?? new List<TodoItem>();
        }
    }
}
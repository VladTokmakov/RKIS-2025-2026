using System.Collections.Generic;
using System.IO;
using Todolist.Interfaces;

namespace Todolist
{
    public class RealFileSystem : IFileSystem
    {
        public bool FileExists(string path) => File.Exists(path);
        public void WriteAllBytes(string path, byte[] data) => File.WriteAllBytes(path, data);
        public byte[] ReadAllBytes(string path) => File.ReadAllBytes(path);
        public void CreateDirectory(string path) => Directory.CreateDirectory(path);
        public IEnumerable<string> GetFiles(string directory, string pattern) => Directory.GetFiles(directory, pattern);
    }
}
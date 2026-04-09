using System.Collections.Generic;

namespace Todolist.Interfaces
{
    public interface IFileSystem
    {
        bool FileExists(string path);
        void WriteAllBytes(string path, byte[] data);
        byte[] ReadAllBytes(string path);
        void CreateDirectory(string path);
        IEnumerable<string> GetFiles(string directory, string pattern);
    }
}
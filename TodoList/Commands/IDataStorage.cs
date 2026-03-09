using System.Collections.Generic;

namespace Todolist
{
    public interface IDataStorage
    {
        void SaveProfile(Profile profile, string filePath);
        Profile LoadProfile(string filePath);
        void SaveTodos(Todolist todos, string filePath);
        Todolist LoadTodos(string filePath);
    }
}
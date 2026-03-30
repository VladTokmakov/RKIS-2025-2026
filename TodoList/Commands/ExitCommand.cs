using System;

namespace Todolist
{
    public class ExitCommand : ICommand
    {
        public void Execute()
        {
            Console.WriteLine("Сохранение данных...");
            
            // Сохраняем все данные
            if (AppInfo.Storage != null)
            {
                try
                {

                    if (AppInfo.Profiles.Count > 0)
                    {
                        AppInfo.Storage.SaveProfiles(AppInfo.Profiles);
                        Console.WriteLine("Профили сохранены.");
                    }
                    
                    if (AppInfo.CurrentProfile != null && AppInfo.Todos != null)
                    {
                        AppInfo.Storage.SaveTodos(AppInfo.CurrentProfile.Id, AppInfo.Todos);
                        Console.WriteLine($"Задачи профиля '{AppInfo.CurrentProfile.FirstName} {AppInfo.CurrentProfile.LastName}' сохранены.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при сохранении: {ex.Message}");
                }
            }
            
            Console.WriteLine("Выход из программы...");
            Environment.Exit(0);
        }
    }
}
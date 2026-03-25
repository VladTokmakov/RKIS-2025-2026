using System;
using System.Collections.Generic;
using System.Linq;
using Todolist.Exceptions;

namespace Todolist.Commands
{
    internal sealed class SyncCommand : ICommand
    {
        private readonly bool _pull;
        private readonly bool _push;

        public SyncCommand(bool pull, bool push)
        {
            _pull = pull;
            _push = push;
        }

        public void Execute()
        {
            using var remoteStorage = new ApiDataStorage();
            
            if (!remoteStorage.IsServerAvailable())
            {
                Console.WriteLine("Ошибка: сервер недоступен.");
                return;
            }

            if (_push)
            {
                Push(remoteStorage);
                Console.WriteLine("Локальные данные отправлены на сервер.");
            }

            if (_pull)
            {
                Pull(remoteStorage);
                Console.WriteLine("Локальное состояние обновлено данными с сервера.");
            }
        }

        private static void Push(ApiDataStorage remoteStorage)
        {
            // Сохраняем локальные данные на диск
            AppInfo.Storage.SaveProfiles(AppInfo.Profiles);
            if (AppInfo.CurrentProfileId != Guid.Empty)
                AppInfo.Storage.SaveTodos(AppInfo.CurrentProfileId, AppInfo.Todos);

            // Загружаем и отправляем профили
            List<Profile> localProfiles = AppInfo.Storage.LoadProfiles().ToList();
            remoteStorage.SaveProfiles(localProfiles);

            // Отправляем задачи для каждого профиля
            foreach (Profile profile in localProfiles)
            {
                List<TodoItem> todos = AppInfo.Storage.LoadTodos(profile.Id).ToList();
                remoteStorage.SaveTodos(profile.Id, todos);
            }
        }

        private static void Pull(ApiDataStorage remoteStorage)
        {
            Guid previousProfileId = AppInfo.CurrentProfileId;

            // Получаем профили с сервера
            List<Profile> remoteProfiles = remoteStorage.LoadProfiles().ToList();
            
            // Сохраняем их локально
            AppInfo.Storage.SaveProfiles(remoteProfiles);
            
            var todosByProfile = new Dictionary<Guid, List<TodoItem>>();
            
            // Получаем задачи для каждого профиля
            foreach (Profile profile in remoteProfiles)
            {
                List<TodoItem> todos = remoteStorage.LoadTodos(profile.Id).ToList();
                todosByProfile[profile.Id] = todos;
                AppInfo.Storage.SaveTodos(profile.Id, todos);
            }

            // Обновляем глобальное состояние
            AppInfo.Profiles = remoteProfiles;
            AppInfo.TodosByProfile.Clear();
            foreach ((Guid profileId, List<TodoItem> todos) in todosByProfile)
            {
                AppInfo.TodosByProfile[profileId] = new TodoList(todos);
            }

            // Очищаем стеки отмены/повтора
            AppInfo.UndoStack.Clear();
            AppInfo.RedoStack.Clear();

            // Восстанавливаем текущий профиль, если он был
            if (previousProfileId != Guid.Empty &&
                AppInfo.TodosByProfile.TryGetValue(previousProfileId, out TodoList? pulledCurrentTodos))
            {
                AppInfo.CurrentProfileId = previousProfileId;
                AppInfo.Todos = pulledCurrentTodos;
                return;
            }

            // Если предыдущий профиль не найден, выбираем первый
            if (AppInfo.Profiles.Count > 0)
            {
                AppInfo.CurrentProfile = AppInfo.Profiles[0];
                return;
            }

            // Нет профилей
            AppInfo.CurrentProfileId = Guid.Empty;
            AppInfo.Todos = new TodoList();
        }
    }
}
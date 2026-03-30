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
           
            AppInfo.Storage.SaveProfiles(AppInfo.Profiles);
            if (AppInfo.CurrentProfileId != Guid.Empty)
                AppInfo.Storage.SaveTodos(AppInfo.CurrentProfileId, AppInfo.Todos);

           
            List<Profile> localProfiles = AppInfo.Storage.LoadProfiles().ToList();
            remoteStorage.SaveProfiles(localProfiles);

           
            foreach (Profile profile in localProfiles)
            {
                List<TodoItem> todos = AppInfo.Storage.LoadTodos(profile.Id).ToList();
                remoteStorage.SaveTodos(profile.Id, todos);
            }
        }

        private static void Pull(ApiDataStorage remoteStorage)
        {
            Guid previousProfileId = AppInfo.CurrentProfileId;

           
            List<Profile> remoteProfiles = remoteStorage.LoadProfiles().ToList();
            
          
            AppInfo.Storage.SaveProfiles(remoteProfiles);
            
            var todosByProfile = new Dictionary<Guid, List<TodoItem>>();
            
         
            foreach (Profile profile in remoteProfiles)
            {
                List<TodoItem> todos = remoteStorage.LoadTodos(profile.Id).ToList();
                todosByProfile[profile.Id] = todos;
                AppInfo.Storage.SaveTodos(profile.Id, todos);
            }

         
            AppInfo.Profiles = remoteProfiles;
            AppInfo.TodosByProfile.Clear();
            foreach ((Guid profileId, List<TodoItem> todos) in todosByProfile)
            {
                AppInfo.TodosByProfile[profileId] = new TodoList(todos);
            }

        
            AppInfo.UndoStack.Clear();
            AppInfo.RedoStack.Clear();

      
            if (previousProfileId != Guid.Empty &&
                AppInfo.TodosByProfile.TryGetValue(previousProfileId, out TodoList? pulledCurrentTodos))
            {
                AppInfo.CurrentProfileId = previousProfileId;
                AppInfo.Todos = pulledCurrentTodos;
                return;
            }


            if (AppInfo.Profiles.Count > 0)
            {
                AppInfo.CurrentProfile = AppInfo.Profiles[0];
                return;
            }

            AppInfo.CurrentProfileId = Guid.Empty;
            AppInfo.Todos = new TodoList();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Todolist.Models;

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
            AppInfo.SaveAllProfiles();
            if (AppInfo.CurrentProfileId != Guid.Empty)
                AppInfo.SaveCurrentTodos();
            
            var localProfiles = AppInfo.ProfileRepo.GetAll();
            remoteStorage.SaveProfiles(localProfiles);
            
            foreach (var profile in localProfiles)
            {
                var todos = AppInfo.TodoRepo.GetAll(profile.Id);
                remoteStorage.SaveTodos(profile.Id, todos);
            }
        }

        private static void Pull(ApiDataStorage remoteStorage)
        {
            var remoteProfiles = remoteStorage.LoadProfiles().ToList();
            
            AppInfo.ProfileRepo.ReplaceAll(remoteProfiles);
            AppInfo.Profiles = remoteProfiles;
            
            foreach (var profile in remoteProfiles)
            {
                var todos = remoteStorage.LoadTodos(profile.Id).ToList();
                AppInfo.TodoRepo.ReplaceAll(profile.Id, todos);
            }
            
            AppInfo.UndoStack.Clear();
            AppInfo.RedoStack.Clear();
            
            if (AppInfo.CurrentProfileId != Guid.Empty)
            {
                var currentProfile = AppInfo.Profiles.FirstOrDefault(p => p.Id == AppInfo.CurrentProfileId);
                if (currentProfile != null)
                {
                    var todos = AppInfo.TodoRepo.GetAll(AppInfo.CurrentProfileId);
                    AppInfo.Todos = new TodoList(todos);
                }
            }
            
            Console.WriteLine("Синхронизация завершена.");
        }
    }
}
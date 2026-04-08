﻿﻿using System;
using System.Linq;
using Todolist.Exceptions;
using Todolist.Models;
using Todolist.Services;

namespace Todolist
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Добро пожаловать в TodoList!");
            Console.WriteLine("Введите 'add_user' для создания профиля или 'help' для списка команд.");
            
            // Загружаем профили из БД
            AppInfo.Profiles = AppInfo.ProfileRepo.GetAll();
            
            if (AppInfo.Profiles.Count > 0)
            {
                var lastProfile = AppInfo.Profiles[0];
                AppInfo.CurrentProfile = lastProfile;
                
                Console.WriteLine($"Загружен профиль: {lastProfile.GetInfo()}");
                Console.WriteLine($"Всего задач: {AppInfo.Todos.GetCount()}");
            }
            else
            {
                Console.WriteLine("Нет сохраненных профилей. Создайте новый командой 'add_user'");
            }
            
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            Console.CancelKeyPress += OnCancelKeyPress;
            
            while (true)
            {
                try
                {
                    Console.Write("\n> ");
                    string? input = Console.ReadLine();
                    
                    if (string.IsNullOrWhiteSpace(input))
                        continue;
                    
                    if (AppInfo.ShouldLogout)
                    {
                        AppInfo.ShouldLogout = false;
                        AppInfo.CurrentProfile = null;
                        AppInfo.Todos = new TodoList();
                        Console.WriteLine("Вы вышли из профиля.");
                        continue;
                    }
                    
                    var command = CommandParser.Parse(input, AppInfo.Todos, AppInfo.CurrentProfile);
                    command?.Execute();
                }
                catch (AuthenticationException ex)
                {
                    Console.WriteLine($"Ошибка аутентификации: {ex.Message}");
                    Console.WriteLine("Создайте профиль командой 'add_user'");
                }
                catch (InvalidCommandException ex)
                {
                    Console.WriteLine($"Неизвестная команда: {ex.Message}");
                    Console.WriteLine("Введите 'help' для списка команд.");
                }
                catch (InvalidArgumentException ex)
                {
                    Console.WriteLine($"Ошибка в аргументах: {ex.Message}");
                }
                catch (TaskNotFoundException ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
                catch (ProfileNotFoundException ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                    Console.WriteLine("Создайте профиль командой 'add_user'");
                }
                catch (BusinessLogicException ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Непредвиденная ошибка: {ex.Message}");
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }
        
        private static void OnProcessExit(object? sender, EventArgs e)
        {
            SaveAllData();
        }
        
        private static void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("\nСохранение данных...");
            SaveAllData();
        }
        
        private static void SaveAllData()
        {
            try
            {
                if (AppInfo.CurrentProfile != null && AppInfo.Todos != null)
                {
                    AppInfo.SaveCurrentTodos();
                    Console.WriteLine($"Задачи профиля '{AppInfo.CurrentProfile.FirstName} {AppInfo.CurrentProfile.LastName}' сохранены в БД.");
                }
                
                AppInfo.SaveAllProfiles();
                Console.WriteLine("Профили сохранены в БД.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении: {ex.Message}");
            }
        }
    }
}
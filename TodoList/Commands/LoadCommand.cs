using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Todolist.Exceptions;

namespace Todolist
{
    public class LoadCommand : IUndo
    {
        private readonly int _downloadsCount;
        private readonly int _maxProgress;

        public LoadCommand(int downloadsCount, int maxProgress)
        {
            if (downloadsCount <= 0)
                throw new InvalidArgumentException("Количество загрузок должно быть больше 0.");
            if (maxProgress <= 0)
                throw new InvalidArgumentException("Размер загрузки должен быть больше 0.");

            _downloadsCount = downloadsCount;
            _maxProgress = maxProgress;
        }

        public void Execute()
        {
            RunAsync().Wait();
        }

        public void Unexecute()
        {
        }

        private async Task RunAsync()
        {
            if (Console.IsOutputRedirected)
            {
                await RunSimpleAsync();
                return;
            }

            int startRow = Console.CursorTop;
            int neededRows = startRow + _downloadsCount + 2;

            if (neededRows > Console.BufferHeight)
            {
                try
                {
                    Console.BufferHeight = neededRows;
                }
                catch
                {
                    await RunSimpleAsync();
                    return;
                }
            }

            for (int i = 0; i < _downloadsCount; i++)
            {
                Console.WriteLine();
            }

            var tasks = new List<Task>();
            for (int i = 0; i < _downloadsCount; i++)
            {
                int index = i;
                tasks.Add(DownloadAsync(index, startRow + index));
            }

            await Task.WhenAll(tasks);
            Console.SetCursorPosition(0, startRow + _downloadsCount);
            Console.WriteLine("Все загрузки завершены.");
        }

        private async Task RunSimpleAsync()
        {
            Console.WriteLine($"Запуск {_downloadsCount} загрузок (упрощённый режим):");
            var tasks = new List<Task>();
            for (int i = 0; i < _downloadsCount; i++)
            {
                int index = i;
                tasks.Add(DownloadSimpleAsync(index));
            }
            await Task.WhenAll(tasks);
            Console.WriteLine("Все загрузки завершены.");
        }

        private async Task DownloadSimpleAsync(int downloadIndex)
        {
            var random = new Random();
            for (int current = 1; current <= _maxProgress; current++)
            {
                int percent = current * 100 / _maxProgress;
                Console.WriteLine($"Загрузка {downloadIndex + 1}: {percent}%");
                await Task.Delay(random.Next(50, 150));
            }
        }

        private async Task DownloadAsync(int downloadIndex, int row)
        {
            var random = new Random();
            for (int current = 1; current <= _maxProgress; current++)
            {
                int percent = current * 100 / _maxProgress;
                UpdateProgressBar(row, percent, downloadIndex + 1);
                int delay = random.Next(50, 150);
                await Task.Delay(delay);
            }
            UpdateProgressBar(row, 100, downloadIndex + 1);
        }

        private static readonly object _consoleLock = new object();

        private static void UpdateProgressBar(int row, int percent, int number)
        {
            const int barLength = 20;
            int filled = percent * barLength / 100;
            string bar = new string('#', filled) + new string('-', barLength - filled);
            string line = $"Загрузка {number}: [{bar}] {percent}%";

            lock (_consoleLock)
            {
                try
                {
                    int currentTop = Console.CursorTop;
                    int currentLeft = Console.CursorLeft;
                    Console.SetCursorPosition(0, row);
                    Console.Write(line.PadRight(Console.WindowWidth - 1));
                    Console.SetCursorPosition(currentLeft, currentTop);
                }
                catch
                {
                    Console.WriteLine(line);
                }
            }
        }
    }
}
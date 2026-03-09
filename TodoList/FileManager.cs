using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using Todolist.Exceptions;

namespace Todolist
{
    public class FileManager : IDataStorage
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("0123456789abcdef0123456789abcdef");
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("abcdefghijklmnop");

        public void SaveProfile(Profile profile, string filePath)
        {
            if (profile == null)
                throw new InvalidArgumentException("Профиль не может быть null");

            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                using (var bufferedStream = new BufferedStream(fileStream))
                using (var aes = Aes.Create())
                {
                    aes.Key = Key;
                    aes.IV = IV;

                    using (var cryptoStream = new CryptoStream(bufferedStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    using (var writer = new StreamWriter(cryptoStream, Encoding.UTF8))
                    {
                        string profileData = $"{EscapeCsvField(profile.FirstName)};{EscapeCsvField(profile.LastName)};{profile.BirthYear}";
                        writer.Write(profileData);
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                throw new BusinessLogicException($"Нет прав на запись в файл: {filePath}");
            }
            catch (DirectoryNotFoundException)
            {
                throw new BusinessLogicException($"Директория для файла профиля не найдена: {filePath}");
            }
            catch (CryptographicException ex)
            {
                throw new BusinessLogicException($"Ошибка шифрования при сохранении профиля: {ex.Message}");
            }
            catch (IOException ex)
            {
                throw new BusinessLogicException($"Ошибка ввода-вывода при сохранении профиля: {ex.Message}");
            }
        }

        public Profile LoadProfile(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                using (var bufferedStream = new BufferedStream(fileStream))
                using (var aes = Aes.Create())
                {
                    aes.Key = Key;
                    aes.IV = IV;

                    using (var cryptoStream = new CryptoStream(bufferedStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    using (var reader = new StreamReader(cryptoStream, Encoding.UTF8))
                    {
                        string content = reader.ReadToEnd();
                        if (string.IsNullOrEmpty(content))
                            return null;

                        string[] parts = ParseCsvLine(content, ';');
                        if (parts.Length == 3)
                        {
                            string firstName = UnescapeCsvField(parts[0]);
                            string lastName = UnescapeCsvField(parts[1]);

                            if (!int.TryParse(parts[2], out int birthYear))
                                throw new BusinessLogicException("Неверный формат года рождения в файле профиля");

                            return new Profile(firstName, lastName, birthYear);
                        }

                        throw new BusinessLogicException("Неверный формат файла профиля");
                    }
                }
            }
            catch (FileNotFoundException)
            {
                return null;
            }
            catch (CryptographicException)
            {
                throw new BusinessLogicException("Файл профиля повреждён или используется неверный ключ шифрования.");
            }
            catch (UnauthorizedAccessException)
            {
                throw new BusinessLogicException($"Нет прав на чтение файла: {filePath}");
            }
            catch (IOException ex)
            {
                throw new BusinessLogicException($"Ошибка ввода-вывода при загрузке профиля: {ex.Message}");
            }
        }

        public void SaveTodos(Todolist todos, string filePath)
        {
            if (todos == null)
                throw new InvalidArgumentException("Список задач не может быть null");

            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                using (var bufferedStream = new BufferedStream(fileStream))
                using (var aes = Aes.Create())
                {
                    aes.Key = Key;
                    aes.IV = IV;

                    using (var cryptoStream = new CryptoStream(bufferedStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    using (var writer = new StreamWriter(cryptoStream, Encoding.UTF8))
                    {
                        int index = 1;
                        foreach (var item in todos)
                        {
                            string escapedText = EscapeCsvField(item.Text);
                            string line = $"{index};{item.Status};{item.LastUpdate:yyyy-MM-ddTHH:mm:ss};{escapedText}";
                            writer.WriteLine(line);
                            index++;
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                throw new BusinessLogicException($"Нет прав на запись в файл: {filePath}");
            }
            catch (DirectoryNotFoundException)
            {
                throw new BusinessLogicException($"Директория для файла задач не найдена: {filePath}");
            }
            catch (CryptographicException ex)
            {
                throw new BusinessLogicException($"Ошибка шифрования при сохранении задач: {ex.Message}");
            }
            catch (IOException ex)
            {
                throw new BusinessLogicException($"Ошибка ввода-вывода при сохранении задач: {ex.Message}");
            }
        }

        public Todolist LoadTodos(string filePath)
        {
            Todolist todos = new Todolist();

            if (!File.Exists(filePath))
                return todos;

            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                using (var bufferedStream = new BufferedStream(fileStream))
                using (var aes = Aes.Create())
                {
                    aes.Key = Key;
                    aes.IV = IV;

                    using (var cryptoStream = new CryptoStream(bufferedStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    using (var reader = new StreamReader(cryptoStream, Encoding.UTF8))
                    {
                        string? line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (string.IsNullOrWhiteSpace(line))
                                continue;

                            string[] parts = ParseCsvLine(line, ';');
                            if (parts.Length >= 4)
                            {
                                if (!Enum.TryParse<TodoStatus>(parts[1], out TodoStatus status))
                                    throw new BusinessLogicException($"Неверный статус в файле: {parts[1]}");

                                if (!DateTime.TryParse(parts[2], out DateTime lastUpdate))
                                    throw new BusinessLogicException($"Неверный формат даты в файле: {parts[2]}");

                                string text = string.Join(";", parts, 3, parts.Length - 3);
                                text = UnescapeCsvField(text);

                                TodoItem item = new TodoItem(text, status, lastUpdate);
                                todos.Add(item);
                            }
                        }
                    }
                }

                return todos;
            }
            catch (FileNotFoundException)
            {
                return todos;
            }
            catch (CryptographicException)
            {
                throw new BusinessLogicException("Файл задач повреждён или используется неверный ключ шифрования.");
            }
            catch (UnauthorizedAccessException)
            {
                throw new BusinessLogicException($"Нет прав на чтение файла: {filePath}");
            }
            catch (IOException ex)
            {
                throw new BusinessLogicException($"Ошибка ввода-вывода при загрузке задач: {ex.Message}");
            }
        }

        public static void EnsureDataDirectory(string dirPath)
        {
            try
            {
                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);
            }
            catch (UnauthorizedAccessException)
            {
                throw new BusinessLogicException($"Нет прав на создание директории: {dirPath}");
            }
            catch (PathTooLongException)
            {
                throw new BusinessLogicException("Слишком длинный путь к директории");
            }
            catch (IOException ex)
            {
                throw new BusinessLogicException($"Ошибка ввода-вывода при создании директории: {ex.Message}");
            }
        }

        private static string EscapeCsvField(string field)
        {
            if (field.Contains(";") || field.Contains("\"") || field.Contains("\n"))
            {
                string temp = field.Replace("\n", "\\n");
                temp = temp.Replace("\"", "\"\"");
                return "\"" + temp + "\"";
            }
            return field;
        }

        private static string UnescapeCsvField(string field)
        {
            if (field.StartsWith("\"") && field.EndsWith("\""))
            {
                field = field.Substring(1, field.Length - 2);
            }
            field = field.Replace("\\n", "\n");
            field = field.Replace("\"\"", "\"");
            return field;
        }

        private static string[] ParseCsvLine(string line, char delimiter)
        {
            var result = new List<string>();
            var current = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        current.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == delimiter && !inQuotes)
                {
                    result.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }

            result.Add(current.ToString());
            return result.ToArray();
        }
    }
}
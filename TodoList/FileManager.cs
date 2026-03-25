using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using Todolist.Exceptions;

namespace Todolist
{
    public class FileManager : IDataStorage
    {
        private readonly string _dataDir;
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public FileManager(string dataDir)
        {
            _dataDir = dataDir;
            _key = Encoding.UTF8.GetBytes("0123456789abcdef0123456789abcdef");
            _iv = Encoding.UTF8.GetBytes("abcdefghijklmnop");
            EnsureDataDirectory();
        }

        public void SaveProfiles(IEnumerable<Profile> profiles)
        {
            if (profiles == null) throw new ArgumentNullException(nameof(profiles));
            
            string path = Path.Combine(_dataDir, "profiles.dat");
            
            try
            {
                using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                using (var bufferedStream = new BufferedStream(fileStream))
                using (var aes = Aes.Create())
                {
                    aes.Key = _key;
                    aes.IV = _iv;

                    using (var cryptoStream = new CryptoStream(bufferedStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    using (var writer = new StreamWriter(cryptoStream, Encoding.UTF8))
                    {
                        foreach (var profile in profiles)
                        {
                            string profileData = $"{EscapeCsvField(profile.Id.ToString())};" +
                                               $"{EscapeCsvField(profile.Login)};" +
                                               $"{EscapeCsvField(profile.Password)};" +
                                               $"{EscapeCsvField(profile.FirstName)};" +
                                               $"{EscapeCsvField(profile.LastName)};" +
                                               $"{profile.BirthYear}";
                            writer.WriteLine(profileData);
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new StorageException($"Нет доступа к файлу профилей: {path}", ex);
            }
            catch (IOException ex)
            {
                throw new StorageException($"Ошибка записи файла профилей: {path}", ex);
            }
            catch (CryptographicException ex)
            {
                throw new StorageException("Ошибка шифрования профилей.", ex);
            }
        }

        public IEnumerable<Profile> LoadProfiles()
        {
            string path = Path.Combine(_dataDir, "profiles.dat");
            var result = new List<Profile>();
            
            if (!File.Exists(path))
                return result;
            
            try
            {
                using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (var bufferedStream = new BufferedStream(fileStream))
                using (var aes = Aes.Create())
                {
                    aes.Key = _key;
                    aes.IV = _iv;

                    using (var cryptoStream = new CryptoStream(bufferedStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    using (var reader = new StreamReader(cryptoStream, Encoding.UTF8))
                    {
                        string? line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (string.IsNullOrWhiteSpace(line)) continue;
                            
                            string[] parts = ParseCsvLine(line, ';');
                            if (parts.Length >= 6)
                            {
                                if (!Guid.TryParse(parts[0], out Guid id))
                                    continue;
                                
                                if (!int.TryParse(parts[5], out int birthYear))
                                    continue;
                                
                                result.Add(new Profile(
                                    id,
                                    UnescapeCsvField(parts[1]),
                                    UnescapeCsvField(parts[2]),
                                    UnescapeCsvField(parts[3]),
                                    UnescapeCsvField(parts[4]),
                                    birthYear
                                ));
                            }
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new StorageException($"Нет доступа к файлу профилей: {path}", ex);
            }
            catch (IOException ex)
            {
                throw new StorageException($"Ошибка чтения файла профилей: {path}", ex);
            }
            catch (CryptographicException ex)
            {
                throw new StorageException("Ошибка расшифровки профилей.", ex);
            }
            
            return result;
        }

        public void SaveTodos(Guid userId, IEnumerable<TodoItem> todos)
        {
            if (userId == Guid.Empty) throw new ArgumentException("Некорректный userId.", nameof(userId));
            if (todos == null) throw new ArgumentNullException(nameof(todos));
            
            string path = Path.Combine(_dataDir, $"todos_{userId}.dat");
            
            try
            {
                using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                using (var bufferedStream = new BufferedStream(fileStream))
                using (var aes = Aes.Create())
                {
                    aes.Key = _key;
                    aes.IV = _iv;

                    using (var cryptoStream = new CryptoStream(bufferedStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    using (var writer = new StreamWriter(cryptoStream, Encoding.UTF8))
                    {
                        foreach (var item in todos)
                        {
                            string escapedText = EscapeCsvField(item.Text);
                            string line = $"{escapedText};{item.Status};{item.LastUpdate:yyyy-MM-ddTHH:mm:ss}";
                            writer.WriteLine(line);
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new StorageException($"Нет доступа к файлу задач пользователя {userId}.", ex);
            }
            catch (IOException ex)
            {
                throw new StorageException($"Ошибка записи задач пользователя {userId}.", ex);
            }
            catch (CryptographicException ex)
            {
                throw new StorageException("Ошибка шифрования задач.", ex);
            }
        }

        public IEnumerable<TodoItem> LoadTodos(Guid userId)
        {
            if (userId == Guid.Empty) throw new ArgumentException("Некорректный userId.", nameof(userId));
            
            string path = Path.Combine(_dataDir, $"todos_{userId}.dat");
            var result = new List<TodoItem>();
            
            if (!File.Exists(path))
                return result;
            
            try
            {
                using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (var bufferedStream = new BufferedStream(fileStream))
                using (var aes = Aes.Create())
                {
                    aes.Key = _key;
                    aes.IV = _iv;

                    using (var cryptoStream = new CryptoStream(bufferedStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    using (var reader = new StreamReader(cryptoStream, Encoding.UTF8))
                    {
                        string? line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (string.IsNullOrWhiteSpace(line)) continue;
                            
                            string[] parts = ParseCsvLine(line, ';');
                            if (parts.Length >= 3)
                            {
                                string text = UnescapeCsvField(parts[0]);
                                var item = new TodoItem(text);
                                
                                if (Enum.TryParse<TodoStatus>(parts[1], out TodoStatus status))
                                    item.Status = status;
                                
                                if (DateTime.TryParse(parts[2], out DateTime lastUpdate))
                                    item.LastUpdate = lastUpdate;
                                
                                result.Add(item);
                            }
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new StorageException($"Нет доступа к файлу задач пользователя {userId}.", ex);
            }
            catch (IOException ex)
            {
                throw new StorageException($"Ошибка чтения задач пользователя {userId}.", ex);
            }
            catch (CryptographicException ex)
            {
                throw new StorageException("Ошибка расшифровки задач.", ex);
            }
            
            return result;
        }

        private void EnsureDataDirectory()
        {
            if (!Directory.Exists(_dataDir))
                Directory.CreateDirectory(_dataDir);
        }

        private static string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
                return string.Empty;
                
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
            if (string.IsNullOrEmpty(field))
                return string.Empty;
                
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
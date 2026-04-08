using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Todolist.Exceptions;
using Todolist.Models;

namespace Todolist
{
    public sealed class ApiDataStorage : IDataStorage, IDisposable
    {
        private const string DefaultBaseAddress = "http://localhost:5000/";
        private readonly HttpClient _httpClient;
        private readonly bool _ownsHttpClient;
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public ApiDataStorage(string baseAddress = DefaultBaseAddress)
        {
            if (string.IsNullOrWhiteSpace(baseAddress))
                throw new ArgumentException("Base address is required.", nameof(baseAddress));
            
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseAddress, UriKind.Absolute),
                Timeout = TimeSpan.FromSeconds(10)
            };
            _ownsHttpClient = true;
            _key = (byte[])StorageCryptoConfig.Key.Clone();
            _iv = (byte[])StorageCryptoConfig.Iv.Clone();
        }

        public bool IsServerAvailable()
        {
            try
            {
                using HttpResponseMessage response = _httpClient.GetAsync("profiles", HttpCompletionOption.ResponseHeadersRead)
                    .GetAwaiter()
                    .GetResult();
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public void SaveProfiles(IEnumerable<Profile> profiles)
        {
            if (profiles == null) throw new ArgumentNullException(nameof(profiles));
            
            var payload = profiles.Select(p => new ProfileDto
            {
                Id = p.Id,
                Login = p.Login ?? string.Empty,
                Password = p.Password ?? string.Empty,
                FirstName = p.FirstName ?? string.Empty,
                LastName = p.LastName ?? string.Empty,
                BirthYear = p.BirthYear
            }).ToList();
            
            byte[] encrypted = EncryptJson(payload);
            PostBytes("profiles", encrypted);
        }

        public IEnumerable<Profile> LoadProfiles()
        {
            byte[] encrypted = GetBytes("profiles");
            if (encrypted.Length == 0)
                return new List<Profile>();
            
            List<ProfileDto>? payload = DecryptJson<List<ProfileDto>>(encrypted);
            if (payload == null)
                return new List<Profile>();
            
            return payload.Select(x => new Profile(
                x.Id, 
                x.Login, 
                x.Password, 
                x.FirstName, 
                x.LastName, 
                x.BirthYear
            )).ToList();
        }

        public void SaveTodos(Guid userId, IEnumerable<TodoItem> todos)
        {
            if (userId == Guid.Empty) throw new ArgumentException("User ID is required.", nameof(userId));
            if (todos == null) throw new ArgumentNullException(nameof(todos));
            
            var payload = todos.Select(t => new TodoItemDto
            {
                Text = t.Text ?? string.Empty,
                Status = t.Status,
                LastUpdate = t.LastUpdate
            }).ToList();
            
            byte[] encrypted = EncryptJson(payload);
            PostBytes(GetTodosPath(userId), encrypted);
        }

        public IEnumerable<TodoItem> LoadTodos(Guid userId)
        {
            if (userId == Guid.Empty) throw new ArgumentException("User ID is required.", nameof(userId));
            
            byte[] encrypted = GetBytes(GetTodosPath(userId));
            if (encrypted.Length == 0)
                return new List<TodoItem>();
            
            List<TodoItemDto>? payload = DecryptJson<List<TodoItemDto>>(encrypted);
            if (payload == null)
                return new List<TodoItem>();
            
            return payload.Select(x => new TodoItem(x.Text ?? string.Empty) 
            { 
                Status = x.Status, 
                LastUpdate = x.LastUpdate 
            }).ToList();
        }

        public void Dispose()
        {
            if (_ownsHttpClient)
                _httpClient.Dispose();
        }

        private void PostBytes(string relativePath, byte[] payload)
        {
            try
            {
                using var content = new ByteArrayContent(payload);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                using HttpResponseMessage response = _httpClient.PostAsync(relativePath, content)
                    .GetAwaiter()
                    .GetResult();
                EnsureSuccessStatusCode(response, $"POST {relativePath}");
            }
            catch (StorageException)
            {
                throw;
            }
            catch (HttpRequestException ex)
            {
                throw new StorageException("Ошибка сетевого запроса к серверу.", ex);
            }
            catch (TaskCanceledException ex)
            {
                throw new StorageException("Превышено время ожидания ответа сервера.", ex);
            }
        }

        private byte[] GetBytes(string relativePath)
        {
            try
            {
                using HttpResponseMessage response = _httpClient.GetAsync(relativePath)
                    .GetAwaiter()
                    .GetResult();
                if (response.StatusCode == HttpStatusCode.NotFound)
                    return Array.Empty<byte>();
                
                EnsureSuccessStatusCode(response, $"GET {relativePath}");
                return response.Content.ReadAsByteArrayAsync()
                    .GetAwaiter()
                    .GetResult();
            }
            catch (StorageException)
            {
                throw;
            }
            catch (HttpRequestException ex)
            {
                throw new StorageException("Ошибка сетевого запроса к серверу.", ex);
            }
            catch (TaskCanceledException ex)
            {
                throw new StorageException("Превышено время ожидания ответа сервера.", ex);
            }
        }

        private static void EnsureSuccessStatusCode(HttpResponseMessage response, string operation)
        {
            if (response.IsSuccessStatusCode)
                return;
            
            throw new StorageException($"Ошибка сервера при операции {operation}: {(int)response.StatusCode} {response.ReasonPhrase}");
        }

        private byte[] EncryptJson<T>(T value)
        {
            try
            {
                string json = JsonSerializer.Serialize(value);
                return EncryptString(json);
            }
            catch (JsonException ex)
            {
                throw new StorageException("Не удалось сериализовать данные для отправки.", ex);
            }
            catch (CryptographicException ex)
            {
                throw new StorageException("Не удалось зашифровать данные для отправки.", ex);
            }
        }

        private T? DecryptJson<T>(byte[] encryptedBytes)
        {
            try
            {
                string json = DecryptToString(encryptedBytes);
                if (string.IsNullOrWhiteSpace(json))
                    return default;
                return JsonSerializer.Deserialize<T>(json);
            }
            catch (JsonException ex)
            {
                throw new StorageException("Не удалось десериализовать данные, полученные с сервера.", ex);
            }
            catch (CryptographicException ex)
            {
                throw new StorageException("Не удалось расшифровать данные, полученные с сервера.", ex);
            }
        }

        private byte[] EncryptString(string plainText)
        {
            byte[] source = Encoding.UTF8.GetBytes(plainText);
            using var output = new MemoryStream();
            using Aes aes = CreateAes();
            using ICryptoTransform encryptor = aes.CreateEncryptor();
            using (var crypto = new CryptoStream(output, encryptor, CryptoStreamMode.Write, leaveOpen: true))
            {
                crypto.Write(source, 0, source.Length);
                crypto.FlushFinalBlock();
            }
            return output.ToArray();
        }

        private string DecryptToString(byte[] encryptedBytes)
        {
            using var input = new MemoryStream(encryptedBytes);
            using Aes aes = CreateAes();
            using ICryptoTransform decryptor = aes.CreateDecryptor();
            using var crypto = new CryptoStream(input, decryptor, CryptoStreamMode.Read);
            using var reader = new StreamReader(crypto, Encoding.UTF8);
            return reader.ReadToEnd();
        }

        private Aes CreateAes()
        {
            Aes aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            return aes;
        }

        private static string GetTodosPath(Guid userId)
        {
            return $"todos/{Uri.EscapeDataString(userId.ToString())}";
        }

        private sealed class ProfileDto
        {
            public Guid Id { get; set; }
            public string Login { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public int BirthYear { get; set; }
        }

        private sealed class TodoItemDto
        {
            public string Text { get; set; } = string.Empty;
            public TodoStatus Status { get; set; }
            public DateTime LastUpdate { get; set; }
        }
    }
}
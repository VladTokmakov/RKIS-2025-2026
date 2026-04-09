using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Xunit;
using Todolist.Data;
using Todolist.Interfaces;
using Todolist.Models;

namespace Todolist.Tests
{
    public class FileStorageTests
    {
        private readonly Mock<IFileSystem> _mockFileSystem;
        private readonly string _testDirectory;
        private readonly FileStorage _storage;

        public FileStorageTests()
        {
            _mockFileSystem = new Mock<IFileSystem>();
            _testDirectory = "test_data";
            _storage = new FileStorage(_testDirectory, _mockFileSystem.Object);
        }

        [Fact]
        public void SaveProfiles_WhenCalled_WritesProfilesToFile()
        {
            // Arrange
            var profiles = new List<Profile>
            {
                new Profile("test1", "pass1", "Иван", "Иванов", 1990),
                new Profile("test2", "pass2", "Петр", "Петров", 1995)
            };
            byte[]? writtenData = null;
            string? writtenPath = null;
            
            _mockFileSystem.Setup(fs => fs.WriteAllBytes(It.IsAny<string>(), It.IsAny<byte[]>()))
                .Callback<string, byte[]>((path, data) => 
                {
                    writtenPath = path;
                    writtenData = data;
                });

            // Act
            _storage.SaveProfiles(profiles);

            // Assert
            _mockFileSystem.Verify(fs => fs.WriteAllBytes(
                It.Is<string>(p => p.Contains("profiles.json")),
                It.IsAny<byte[]>()), Times.Once);
            
            Assert.NotNull(writtenData);
            Assert.True(writtenData.Length > 0);
            Assert.Contains("profiles.json", writtenPath);
        }

        [Fact]
        public void LoadProfiles_WhenFileExists_ReturnsDeserializedProfiles()
        {
            // Arrange
            var expectedProfiles = new List<Profile>
            {
                new Profile("test1", "pass1", "Иван", "Иванов", 1990),
                new Profile("test2", "pass2", "Петр", "Петров", 1995)
            };
            var json = System.Text.Json.JsonSerializer.Serialize(expectedProfiles);
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            
            _mockFileSystem.Setup(fs => fs.FileExists(It.IsAny<string>())).Returns(true);
            _mockFileSystem.Setup(fs => fs.ReadAllBytes(It.IsAny<string>())).Returns(bytes);

            // Act
            var result = _storage.LoadProfiles().ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("test1", result[0].Login);
            Assert.Equal("Иван", result[0].FirstName);
            Assert.Equal("test2", result[1].Login);
        }

        [Fact]
        public void LoadProfiles_WhenFileDoesNotExist_ReturnsEmptyList()
        {
            // Arrange
            _mockFileSystem.Setup(fs => fs.FileExists(It.IsAny<string>())).Returns(false);

            // Act
            var result = _storage.LoadProfiles();

            // Assert
            Assert.Empty(result);
            _mockFileSystem.Verify(fs => fs.ReadAllBytes(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void SaveTodos_WhenCalled_WritesTodosToUserFile()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var todos = new List<TodoItem>
            {
                new TodoItem("Задача 1"),
                new TodoItem("Задача 2")
            };
            byte[]? writtenData = null;
            string? writtenPath = null;
            
            _mockFileSystem.Setup(fs => fs.WriteAllBytes(It.IsAny<string>(), It.IsAny<byte[]>()))
                .Callback<string, byte[]>((path, data) =>
                {
                    writtenPath = path;
                    writtenData = data;
                });

            // Act
            _storage.SaveTodos(userId, todos);

            // Assert
            _mockFileSystem.Verify(fs => fs.WriteAllBytes(
                It.Is<string>(p => p.Contains($"todos_{userId}")),
                It.IsAny<byte[]>()), Times.Once);
            
            Assert.NotNull(writtenData);
            Assert.True(writtenData.Length > 0);
            Assert.Contains($"todos_{userId}", writtenPath);
        }

        [Fact]
        public void LoadTodos_WhenFileExists_ReturnsDeserializedTodos()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var expectedTodos = new List<TodoItem>
            {
                new TodoItem("Задача 1"),
                new TodoItem("Задача 2")
            };
            var json = System.Text.Json.JsonSerializer.Serialize(expectedTodos);
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            
            _mockFileSystem.Setup(fs => fs.FileExists(It.IsAny<string>())).Returns(true);
            _mockFileSystem.Setup(fs => fs.ReadAllBytes(It.IsAny<string>())).Returns(bytes);

            // Act
            var result = _storage.LoadTodos(userId).ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Задача 1", result[0].Text);
            Assert.Equal("Задача 2", result[1].Text);
        }

        [Fact]
        public void LoadTodos_WhenFileDoesNotExist_ReturnsEmptyList()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockFileSystem.Setup(fs => fs.FileExists(It.IsAny<string>())).Returns(false);

            // Act
            var result = _storage.LoadTodos(userId);

            // Assert
            Assert.Empty(result);
            _mockFileSystem.Verify(fs => fs.ReadAllBytes(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void Constructor_WhenCalled_CreatesDataDirectory()
        {
            // Act
            var newStorage = new FileStorage("new_test_dir", _mockFileSystem.Object);

            // Assert
            _mockFileSystem.Verify(fs => fs.CreateDirectory("new_test_dir"), Times.Once);
        }
    }
}
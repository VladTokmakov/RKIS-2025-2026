using System;
using Xunit;
using Todolist;

namespace Todolist.Tests
{
    public class TodoItemTests
    {
        [Fact]
        public void Constructor_WithTextOnly_SetsDefaultValues()
        {
            // Arrange
            string text = "Купить молоко";

            // Act
            var item = new TodoItem(text);

            // Assert
            Assert.Equal(text, item.Text);
            Assert.Equal(TodoStatus.NotStarted, item.Status);
            Assert.True(item.LastUpdate <= DateTime.Now);
        }

        [Fact]
        public void Constructor_WithAllParameters_SetsPropertiesCorrectly()
        {
            // Arrange
            string text = "Купить молоко";
            TodoStatus status = TodoStatus.InProgress;
            DateTime lastUpdate = new DateTime(2024, 1, 15, 10, 30, 0);

            // Act
            var item = new TodoItem(text, status, lastUpdate);

            // Assert
            Assert.Equal(text, item.Text);
            Assert.Equal(status, item.Status);
            Assert.Equal(lastUpdate, item.LastUpdate);
        }

        [Fact]
        public void SetStatus_WithUpdateTime_ChangesStatusAndUpdatesTime()
        {
            // Arrange
            var item = new TodoItem("Тестовая задача");
            DateTime originalDate = item.LastUpdate;
            TodoStatus newStatus = TodoStatus.Completed;

            // Act
            System.Threading.Thread.Sleep(10);
            item.SetStatus(newStatus);

            // Assert
            Assert.Equal(newStatus, item.Status);
            Assert.True(item.LastUpdate > originalDate);
        }

        [Fact]
        public void SetStatus_WithoutUpdateTime_ChangesStatusOnly()
        {
            // Arrange
            var item = new TodoItem("Тестовая задача");
            DateTime originalDate = item.LastUpdate;
            TodoStatus newStatus = TodoStatus.Completed;

            // Act
            item.SetStatus(newStatus, false);

            // Assert
            Assert.Equal(newStatus, item.Status);
            Assert.Equal(originalDate, item.LastUpdate);
        }

        [Fact]
        public void UpdateText_ChangesTextAndUpdatesTime()
        {
            // Arrange
            var item = new TodoItem("Старый текст");
            DateTime originalDate = item.LastUpdate;
            string newText = "Новый текст";

            // Act
            System.Threading.Thread.Sleep(10);
            item.UpdateText(newText);

            // Assert
            Assert.Equal(newText, item.Text);
            Assert.True(item.LastUpdate > originalDate);
        }

        [Fact]
        public void SetLastUpdate_SetsExactDateTime()
        {
            // Arrange
            var item = new TodoItem("Тест");
            DateTime newDate = new DateTime(2023, 12, 31, 23, 59, 59);

            // Act
            item.SetLastUpdate(newDate);

            // Assert
            Assert.Equal(newDate, item.LastUpdate);
        }

        [Theory]
        [InlineData("Короткий текст", "Короткий текст")]
        [InlineData("Очень длинный текст, который точно превысит тридцать четыре символа", "Очень длинный текст, который т... ")]
        [InlineData("Текст с\nпереносом", "Текст с переносом")]
        public void GetShortInfo_ReturnsCorrectFormat(string input, string expected)
        {
            // Arrange
            var item = new TodoItem(input);

            // Act
            string result = item.GetShortInfo();

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetFullInfo_ReturnsAllInformation()
        {
            // Arrange
            var item = new TodoItem("Купить молоко", TodoStatus.InProgress, new DateTime(2024, 1, 15, 10, 30, 0));
            string expected = $"Задача: Купить молоко\nСтатус: InProgress\nДата изменения: {new DateTime(2024, 1, 15, 10, 30, 0)}";

            // Act
            string result = item.GetFullInfo();

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
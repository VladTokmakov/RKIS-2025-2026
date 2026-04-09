using System;
using Moq;
using Xunit;
using Todolist.Models;
using Todolist.Interfaces;

namespace Todolist.Tests
{
    public class TodoItemTests
    {
        private readonly Mock<IClock> _mockClock;
        private readonly DateTime _fixedTime;

        public TodoItemTests()
        {
            _fixedTime = new DateTime(2024, 1, 15, 10, 30, 0);
            _mockClock = new Mock<IClock>();
            _mockClock.Setup(c => c.Now).Returns(_fixedTime);
        }

        [Fact]
        public void Constructor_WithTextOnly_SetsDefaultValuesAndUsesClock()
        {
            // Arrange
            string text = "Купить молоко";

            // Act
            var item = new TodoItem(text, _mockClock.Object);

            // Assert
            Assert.Equal(text, item.Text);
            Assert.Equal(TodoStatus.NotStarted, item.Status);
            Assert.Equal(_fixedTime, item.LastUpdate);
            _mockClock.Verify(c => c.Now, Times.Once);
        }

        [Fact]
        public void Constructor_WithoutClock_UsesSystemClock()
        {
            // Act
            var item = new TodoItem("Тест");

            // Assert
            Assert.True(item.LastUpdate <= DateTime.Now && item.LastUpdate > DateTime.Now.AddSeconds(-1));
        }

        [Fact]
        public void SetStatus_WithUpdateTime_ChangesStatusAndUpdatesTimeUsingClock()
        {
            // Arrange
            var item = new TodoItem("Тестовая задача", _mockClock.Object);
            TodoStatus newStatus = TodoStatus.Completed;
            var newTime = _fixedTime.AddHours(1);
            _mockClock.Setup(c => c.Now).Returns(newTime);

            // Act
            item.SetStatus(newStatus, true);

            // Assert
            Assert.Equal(newStatus, item.Status);
            Assert.Equal(newTime, item.LastUpdate);
            _mockClock.Verify(c => c.Now, Times.Exactly(2)); // 1 раз в конструкторе, 1 раз в SetStatus
        }

        [Fact]
        public void SetStatus_WithoutUpdateTime_ChangesStatusOnly()
        {
            // Arrange
            var item = new TodoItem("Тестовая задача", _mockClock.Object);
            DateTime originalDate = item.LastUpdate;
            TodoStatus newStatus = TodoStatus.Completed;

            // Act
            item.SetStatus(newStatus, false);

            // Assert
            Assert.Equal(newStatus, item.Status);
            Assert.Equal(originalDate, item.LastUpdate);
            _mockClock.Verify(c => c.Now, Times.Once); // Только в конструкторе
        }

        [Fact]
        public void UpdateText_ChangesTextAndUpdatesTimeUsingClock()
        {
            // Arrange
            var item = new TodoItem("Старый текст", _mockClock.Object);
            string newText = "Новый текст";
            var newTime = _fixedTime.AddHours(2);
            _mockClock.Setup(c => c.Now).Returns(newTime);

            // Act
            item.UpdateText(newText);

            // Assert
            Assert.Equal(newText, item.Text);
            Assert.Equal(newTime, item.LastUpdate);
            _mockClock.Verify(c => c.Now, Times.Exactly(2)); // 1 раз в конструкторе, 1 раз в UpdateText
        }
    }
}
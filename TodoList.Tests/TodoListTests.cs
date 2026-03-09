using System;
using Xunit;
using Todolist;

namespace Todolist.Tests
{
    public class TodolistTests
    {
        [Fact]
        public void Constructor_CreatesEmptyList()
        {
            // Arrange & Act
            var list = new Todolist();

            // Assert
            Assert.Equal(0, list.GetCount());
        }

        [Fact]
        public void Add_ValidItem_IncreasesCount()
        {
            // Arrange
            var list = new Todolist();
            var item = new TodoItem("Купить молоко");

            // Act
            list.Add(item);

            // Assert
            Assert.Equal(1, list.GetCount());
            Assert.Same(item, list.GetItem(0));
        }

        [Fact]
        public void Add_MultipleItems_AddsAllCorrectly()
        {
            // Arrange
            var list = new Todolist();
            var item1 = new TodoItem("Задача 1");
            var item2 = new TodoItem("Задача 2");
            var item3 = new TodoItem("Задача 3");

            // Act
            list.Add(item1);
            list.Add(item2);
            list.Add(item3);

            // Assert
            Assert.Equal(3, list.GetCount());
            Assert.Same(item1, list.GetItem(0));
            Assert.Same(item2, list.GetItem(1));
            Assert.Same(item3, list.GetItem(2));
        }

        [Fact]
        public void Delete_ValidIndex_RemovesItem()
        {
            // Arrange
            var list = new Todolist();
            list.Add(new TodoItem("Задача 1"));
            list.Add(new TodoItem("Задача 2"));
            list.Add(new TodoItem("Задача 3"));

            // Act
            list.Delete(1); // Удаляем второй элемент (индекс 1)

            // Assert
            Assert.Equal(2, list.GetCount());
            Assert.Equal("Задача 1", list.GetItem(0).Text);
            Assert.Equal("Задача 3", list.GetItem(1).Text);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(5)]
        public void Delete_InvalidIndex_ThrowsException(int index)
        {
            // Arrange
            var list = new Todolist();
            list.Add(new TodoItem("Задача 1"));

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => list.Delete(index));
        }

        [Fact]
        public void SetStatus_ValidIndex_ChangesStatus()
        {
            // Arrange
            var list = new Todolist();
            list.Add(new TodoItem("Задача 1"));
            var item = list.GetItem(0);

            // Act
            list.SetStatus(0, TodoStatus.Completed);

            // Assert
            Assert.Equal(TodoStatus.Completed, item.Status);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1)]
        public void SetStatus_InvalidIndex_ThrowsException(int index)
        {
            // Arrange
            var list = new Todolist();
            list.Add(new TodoItem("Задача 1"));

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => list.SetStatus(index, TodoStatus.Completed));
        }

        [Fact]
        public void GetItem_ValidIndex_ReturnsCorrectItem()
        {
            // Arrange
            var list = new Todolist();
            var item1 = new TodoItem("Задача 1");
            var item2 = new TodoItem("Задача 2");
            list.Add(item1);
            list.Add(item2);

            // Act
            var result = list.GetItem(1);

            // Assert
            Assert.Same(item2, result);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(2)]
        public void GetItem_InvalidIndex_ThrowsException(int index)
        {
            // Arrange
            var list = new Todolist();
            list.Add(new TodoItem("Задача 1"));
            list.Add(new TodoItem("Задача 2"));

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => list.GetItem(index));
        }

        [Fact]
        public void Indexer_ValidIndex_ReturnsCorrectItem()
        {
            // Arrange
            var list = new Todolist();
            var item = new TodoItem("Задача 1");
            list.Add(item);

            // Act
            var result = list[0];

            // Assert
            Assert.Same(item, result);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1)]
        public void Indexer_InvalidIndex_ThrowsException(int index)
        {
            // Arrange
            var list = new Todolist();
            list.Add(new TodoItem("Задача 1"));

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => list[index]);
        }

        [Fact]
        public void GetEnumerator_ReturnsAllItems()
        {
            // Arrange
            var list = new Todolist();
            list.Add(new TodoItem("Задача 1"));
            list.Add(new TodoItem("Задача 2"));
            list.Add(new TodoItem("Задача 3"));

            // Act & Assert
            int count = 0;
            foreach (var item in list)
            {
                Assert.NotNull(item);
                count++;
            }
            Assert.Equal(3, count);
        }

        [Fact]
        public void View_WithEmptyList_DoesNotThrow()
        {
            // Arrange
            var list = new Todolist();

            // Act & Assert
            var exception = Record.Exception(() => list.View(false, false, false));
            Assert.Null(exception);
        }
    }
}
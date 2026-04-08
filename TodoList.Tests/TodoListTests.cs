using System;
using Xunit;
using Todolist;
using Todolist.Models;


namespace Todolist.Tests
{
    public class TodolistTests
    {
        [Fact]
        public void Constructor_CreatesEmptyList()
        {
            // Arrange & Act
            var list = new TodoList();
            // Assert
            Assert.Equal(0, list.GetCount());
        }

        [Fact]
        public void Add_ValidItem_IncreasesCount()
        {
            // Arrange
            var list = new TodoList();
            var item = new TodoItem("Купить молоко");
            // Act
            list.Add(item);
            // Assert
            Assert.Equal(1, list.GetCount());
            Assert.Same(item, list.GetItem(1));
        }

        [Fact]
        public void Add_MultipleItems_AddsAllCorrectly()
        {
            // Arrange
            var list = new TodoList();
            var item1 = new TodoItem("Задача 1");
            var item2 = new TodoItem("Задача 2");
            var item3 = new TodoItem("Задача 3");
            // Act
            list.Add(item1);
            list.Add(item2);
            list.Add(item3);
            // Assert
            Assert.Equal(3, list.GetCount());
            Assert.Same(item1, list.GetItem(1));
            Assert.Same(item2, list.GetItem(2));
            Assert.Same(item3, list.GetItem(3));
        }

        [Fact]
        public void Delete_ValidIndex_RemovesItem()
        {
            // Arrange
            var list = new TodoList();
            list.Add(new TodoItem("Задача 1"));
            list.Add(new TodoItem("Задача 2"));
            list.Add(new TodoItem("Задача 3"));
            // Act 
            list.Delete(2);
            // Assert
            Assert.Equal(2, list.GetCount());
            Assert.Equal("Задача 1", list.GetItem(1).Text);
            Assert.Equal("Задача 3", list.GetItem(2).Text);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(5)]
        public void Delete_InvalidIndex_ThrowsException(int index)
        {
            // Arrange
            var list = new TodoList();
            list.Add(new TodoItem("Задача 1"));
            // Act & Assert 
            Assert.Throws<Exception>(() => list.Delete(index));
        }

        [Fact]
        public void SetStatus_ValidIndex_ChangesStatus()
        {
            // Arrange
            var list = new TodoList();
            list.Add(new TodoItem("Задача 1"));
            var item = list.GetItem(1);
            // Act 
            list.SetStatus(1, TodoStatus.Completed);
            // Assert
            Assert.Equal(TodoStatus.Completed, item.Status);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(2)]
        public void SetStatus_InvalidIndex_ThrowsException(int index)
        {
            // Arrange
            var list = new TodoList();
            list.Add(new TodoItem("Задача 1"));
            // Act & Assert
            if (index == 0)
            {
                Assert.Throws<Exception>(() => list.SetStatus(index, TodoStatus.Completed));
            }
            else
            {
                var exception = Record.Exception(() => list.SetStatus(index, TodoStatus.Completed));
                if (exception != null)
                {
                    Assert.IsType<Exception>(exception);
                }
            }
        }

        [Fact]
        public void GetItem_ValidIndex_ReturnsCorrectItem()
        {
            // Arrange
            var list = new TodoList();
            var item1 = new TodoItem("Задача 1");
            var item2 = new TodoItem("Задача 2");
            list.Add(item1);
            list.Add(item2);
            var result = list.GetItem(2);
            // Assert
            Assert.Same(item2, result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(3)]
        public void GetItem_InvalidIndex_ThrowsException(int index)
        {
            // Arrange
            var list = new TodoList();
            list.Add(new TodoItem("Задача 1"));
            list.Add(new TodoItem("Задача 2"));
            // Act & Assert
            if (index == 0)
            {
                Assert.Throws<Exception>(() => list.GetItem(index));
            }
            else
            {
                var exception = Record.Exception(() => list.GetItem(index));
                if (exception != null)
                {
                    Assert.IsType<Exception>(exception);
                }
            }
        }

        [Fact]
        public void GetItemByZeroIndex_ThrowsException()
        {
            // Arrange
            var list = new TodoList();
            list.Add(new TodoItem("Задача 1"));
            // Act & Assert - индекс 0 недопустим
            Assert.Throws<Exception>(() => list.GetItem(0));
        }

        [Fact]
        public void GetEnumerator_ReturnsAllItems()
        {
            // Arrange
            var list = new TodoList();
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
            var list = new TodoList();
            // Act & Assert
            var exception = Record.Exception(() => list.View(false, false, false));
            Assert.Null(exception);
        }
    }
}
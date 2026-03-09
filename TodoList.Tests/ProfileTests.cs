using System;
using Xunit;
using Todolist;

namespace Todolist.Tests
{
    public class ProfileTests
    {
        [Fact]
        public void Constructor_WithValidData_SetsPropertiesCorrectly()
        {
            // Arrange
            string firstName = "Иван";
            string lastName = "Иванов";
            int birthYear = 1990;

            // Act
            var profile = new Profile(firstName, lastName, birthYear);

            // Assert
            Assert.Equal(firstName, profile.FirstName);
            Assert.Equal(lastName, profile.LastName);
            Assert.Equal(birthYear, profile.BirthYear);
        }

        [Fact]
        public void GetInfo_ReturnsCorrectFormattedString()
        {
            // Arrange
            var profile = new Profile("Иван", "Иванов", 1990);
            int expectedAge = DateTime.Now.Year - 1990;

            // Act
            string result = profile.GetInfo();

            // Assert
            Assert.Equal($"Иван Иванов, возраст {expectedAge}", result);
        }

        [Theory]
        [InlineData("Петр", "Петров", 2000)]
        [InlineData("Анна", "Сидорова", 1985)]
        [InlineData("Мария", "Иванова", 1995)]
        public void GetInfo_WithDifferentData_ReturnsCorrectFormat(string firstName, string lastName, int birthYear)
        {
            // Arrange
            var profile = new Profile(firstName, lastName, birthYear);
            int expectedAge = DateTime.Now.Year - birthYear;

            // Act
            string result = profile.GetInfo();

            // Assert
            Assert.Equal($"{firstName} {lastName}, возраст {expectedAge}", result);
        }
    }
}
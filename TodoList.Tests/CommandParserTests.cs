using System;
using Xunit;
using Todolist;
using Todolist.Exceptions;

namespace Todolist.Tests
{
    public class CommandParserTests
    {
        private readonly TodoList _todoList;
        private readonly Profile _profile;
        
        public CommandParserTests()
        {
            _todoList = new TodoList();
            _profile = new Profile("Тест", "Тестовый", 2000);
            AppInfo.CurrentProfile = _profile;
            AppInfo.Todos = _todoList;
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Parse_EmptyInput_ThrowsInvalidArgumentException(string input)
        {
            // Act & Assert
            Assert.Throws<InvalidArgumentException>(() => 
                CommandParser.Parse(input, _todoList, _profile));
        }

        [Theory]
        [InlineData("help", typeof(HelpCommand))]
        [InlineData("exit", typeof(ExitCommand))]
        [InlineData("profile", typeof(ProfileCommand))]
        [InlineData("undo", typeof(UndoCommand))]
        [InlineData("redo", typeof(RedoCommand))]
        [InlineData("add_user", typeof(SetDataUserCommand))]
        public void Parse_SimpleCommands_ReturnsCorrectType(string input, Type expectedType)
        {
            // Act
            var command = CommandParser.Parse(input, _todoList, _profile);
            // Assert
            Assert.IsType(expectedType, command);
        }

        [Theory]
        [InlineData("add Купить молоко", "Купить молоко", false)]
        [InlineData("add \"Текст в кавычках\"", "\"Текст в кавычках\"", false)]
        [InlineData("add --multiline", "", true)]
        [InlineData("add -m", "", true)]
        public void Parse_AddCommand_ReturnsCorrectCommand(string input, string expectedText, bool expectedMultiline)
        {
            // Act
            var command = CommandParser.Parse(input, _todoList, _profile) as AddCommand;
            // Assert
            Assert.NotNull(command);
            Assert.Equal(expectedText, command.TaskText);
            Assert.Equal(expectedMultiline, command.IsMultiline);
        }

        [Fact]
        public void Parse_AddCommand_WithoutProfile_ThrowsAuthenticationException()
        {
            // Act & Assert
            Assert.Throws<AuthenticationException>(() => 
                CommandParser.Parse("add Тест", _todoList, null));
        }

        [Fact]
        public void Parse_AddCommand_WithoutArguments_ThrowsInvalidArgumentException()
        {
            // Act & Assert
            Assert.Throws<InvalidArgumentException>(() => 
                CommandParser.Parse("add", _todoList, _profile));
        }

        [Theory]
        [InlineData("profile --out")]
        [InlineData("profile -o")]
        public void Parse_ProfileOutCommand_ReturnsNullAndSetsShouldLogout(string input)
        {
            // Arrange
            AppInfo.ShouldLogout = false;
            // Act
            var command = CommandParser.Parse(input, _todoList, _profile);
            // Assert
            Assert.Null(command);
            Assert.True(AppInfo.ShouldLogout);
        }

        [Fact]
        public void Parse_ProfileCommand_WithoutProfile_ThrowsProfileNotFoundException()
        {
            // Act & Assert
            Assert.Throws<ProfileNotFoundException>(() => 
                CommandParser.Parse("profile", _todoList, null));
        }

        [Fact]
        public void Parse_ReadCommand_WithValidNumber_ReturnsReadCommand()
        {
            // Arrange
            _todoList.Add(new TodoItem("Тестовая задача"));
            // Act
            var command = CommandParser.Parse("read 1", _todoList, _profile);
            // Assert
            Assert.IsType<ReadCommand>(command);
        }

        [Theory]
        [InlineData("read abc")]
        [InlineData("read 0")]
        [InlineData("read -1")]
        public void Parse_ReadCommand_WithInvalidNumber_ThrowsInvalidArgumentException(string input)
        {
            // Arrange
            _todoList.Add(new TodoItem("Тестовая задача"));
            // Act & Assert
            Assert.Throws<InvalidArgumentException>(() => 
                CommandParser.Parse(input, _todoList, _profile));
        }

        [Fact]
        public void Parse_ReadCommand_WithNonExistentNumber_ThrowsTaskNotFoundException()
        {
            // Arrange
            _todoList.Add(new TodoItem("Тестовая задача"));
            // Act & Assert
            Assert.Throws<TaskNotFoundException>(() => 
                CommandParser.Parse("read 2", _todoList, _profile));
        }

        [Theory]
        [InlineData("view")]
        [InlineData("view -i")]
        [InlineData("view -s")]
        [InlineData("view -d")]
        [InlineData("view -a")]
        [InlineData("view --index")]
        [InlineData("view --status")]
        [InlineData("view --update-date")]
        [InlineData("view --all")]
        [InlineData("view -i -s -d")]
        public void Parse_ViewCommand_ReturnsViewCommand(string input)
        {
            // Act
            var command = CommandParser.Parse(input, _todoList, _profile);
            // Assert
            Assert.IsType<ViewCommand>(command);
        }

        [Fact]
        public void Parse_StatusCommand_WithValidArguments_ReturnsStatusCommand()
        {
            // Arrange
            _todoList.Add(new TodoItem("Тестовая задача"));
            // Act
            var command = CommandParser.Parse("status 1 Completed", _todoList, _profile);
            // Assert
            Assert.IsType<StatusCommand>(command);
        }

        [Theory]
        [InlineData("status 1 InvalidStatus")]
        [InlineData("status abc Completed")]
        [InlineData("status")]
        public void Parse_StatusCommand_WithInvalidArguments_ThrowsInvalidArgumentException(string input)
        {
            // Arrange
            _todoList.Add(new TodoItem("Тестовая задача"));
            // Act & Assert
            Assert.Throws<InvalidArgumentException>(() => 
                CommandParser.Parse(input, _todoList, _profile));
        }

        [Fact]
        public void Parse_DeleteCommand_WithValidNumber_ReturnsDeleteCommand()
        {
            // Arrange
            _todoList.Add(new TodoItem("Тестовая задача"));
            // Act
            var command = CommandParser.Parse("delete 1", _todoList, _profile);
            // Assert
            Assert.IsType<DeleteCommand>(command);
        }

        [Theory]
        [InlineData("delete abc")]
        [InlineData("delete 0")]
        [InlineData("delete")]
        public void Parse_DeleteCommand_WithInvalidArguments_ThrowsInvalidArgumentException(string input)
        {
            // Arrange
            _todoList.Add(new TodoItem("Тестовая задача"));
            // Act & Assert
            Assert.Throws<InvalidArgumentException>(() => 
                CommandParser.Parse(input, _todoList, _profile));
        }

        [Fact]
        public void Parse_UpdateCommand_WithValidArguments_ReturnsUpdateCommand()
        {
            // Arrange
            _todoList.Add(new TodoItem("Старая задача"));
            // Act
            var command = CommandParser.Parse("update 1 Новая задача", _todoList, _profile);
            // Assert
            Assert.IsType<UpdateCommand>(command);
        }

        [Theory]
        [InlineData("update")]
        [InlineData("update 1")]
        [InlineData("update abc")]
        public void Parse_UpdateCommand_WithInvalidArguments_ThrowsInvalidArgumentException(string input)
        {
            // Arrange
            _todoList.Add(new TodoItem("Тестовая задача"));
            // Act & Assert
            Assert.Throws<InvalidArgumentException>(() => 
                CommandParser.Parse(input, _todoList, _profile));
        }

        [Fact]
        public void Parse_UnknownCommand_ThrowsInvalidCommandException()
        {
            // Act & Assert
            Assert.Throws<InvalidCommandException>(() => 
                CommandParser.Parse("unknowncommand", _todoList, _profile));
        }

        [Fact]
        public void Parse_SearchCommand_WithoutArgs_ReturnsSearchCommand()
        {
            // Act
            var command = CommandParser.Parse("search", _todoList, _profile);
            // Assert
            Assert.IsType<SearchCommand>(command);
        }

        [Theory]
        [InlineData("load 5 10")]
        [InlineData("load 1 100")]
        [InlineData("load 100 1")]
        public void Parse_LoadCommand_WithValidArguments_ReturnsLoadCommand(string input)
        {
            // Act
            var command = CommandParser.Parse(input, _todoList, _profile);
            // Assert
            Assert.IsType<LoadCommand>(command);
        }

        [Theory]
        [InlineData("load")]
        [InlineData("load 5")]
        [InlineData("load abc 10")]
        [InlineData("load 5 abc")]
        [InlineData("load 0 10")]
        [InlineData("load 5 0")]
        public void Parse_LoadCommand_WithInvalidArguments_ThrowsInvalidArgumentException(string input)
        {
            // Act & Assert
            Assert.Throws<InvalidArgumentException>(() => 
                CommandParser.Parse(input, _todoList, _profile));
        }

        [Fact]
        public void Parse_Commands_ThatDoNotRequireProfile_WorkWithoutProfile()
        {
            var commands = new[]
            {
                "help",
                "exit",
                "add_user"
            };
            // Act & Assert
            foreach (var cmd in commands)
            {
                var exception = Record.Exception(() => 
                    CommandParser.Parse(cmd, _todoList, null));
                Assert.Null(exception);
            }
        }

        [Fact]
        public void Parse_Commands_ThatRequireProfile_ThrowAuthenticationException()
        {
            var commands = new[]
            {
                "add Тест",
                "read 1",
                "view",
                "status 1 Completed",
                "delete 1",
                "update 1 Текст"
            };
            // Arrange
            _todoList.Add(new TodoItem("Тест"));
            // Act & Assert
            foreach (var cmd in commands)
            {
                Assert.Throws<AuthenticationException>(() => 
                    CommandParser.Parse(cmd, _todoList, null));
            }
        }
    }
}
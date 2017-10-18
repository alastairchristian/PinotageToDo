using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

using PinotageTodo.Data.Models;

namespace PinotageTodo.Tests.Controllers.TodosControllerTests
{
    public class GetAll : TodosControllerTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("abc")]
        [InlineData("123-456-789")]
        public void Throws_InvalidOperationException_If_A_Valid_UserId_Cannot_Be_Found_In_Claims(string userId)
        {
            // Arrange
            _objectUnderTest.UserId = () => userId;

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>_objectUnderTest.GetAll());
        }

        [Fact]
        public void Returns_Empty_Enumerable_If_TodoRepository_GetAll_Returns_Null()
        {
            // Arrange
            List<TodoDataModel> repositoryResult = null;

            _mockTodoRepository.Setup(r => r.GetAll(_testUserId)).Returns(repositoryResult);

            // Act
            var actualResult = _objectUnderTest.GetAll();

            // Assert
            Assert.NotNull(actualResult);
            Assert.Empty(actualResult);
        }

        [Fact]
        public void Returns_Empty_Enumerable_If_TodoRepository_GetAll_Returns_Empty_List()
        {
            // Arrange
            var repositoryResult = new List<TodoDataModel>();

            _mockTodoRepository.Setup(r => r.GetAll(_testUserId)).Returns(repositoryResult);

            // Act
            var actualResult = _objectUnderTest.GetAll();

            // Assert
            Assert.NotNull(actualResult);
            Assert.Empty(actualResult);
        }

        [Fact]
        public void Returns_TodoApiModel_For_Each_TodoDataModel_Returned_From_TodoRepository_GetAll()
        {
            // Arrange
            var repositoryResult = new List<TodoDataModel>()
            {
                new TodoDataModel() { Id = Guid.NewGuid(), Title = "Todo 1", UserId = _testUserId, IsCompleted = false },
                new TodoDataModel() { Id = Guid.NewGuid(), Title = "Todo 2", UserId = _testUserId, IsCompleted = false },
                new TodoDataModel() { Id = Guid.NewGuid(), Title = "Todo 3", UserId = _testUserId, IsCompleted = true },
                new TodoDataModel() { Id = Guid.NewGuid(), Title = "Todo 4", UserId = _testUserId, IsCompleted = true },
                new TodoDataModel() { Id = Guid.NewGuid(), Title = "Todo 5", UserId = _testUserId, IsCompleted = false },
            };

            _mockTodoRepository.Setup(r => r.GetAll(_testUserId)).Returns(repositoryResult);

            // Act
            var actualResult = _objectUnderTest.GetAll();

            // Assert
            Assert.NotNull(actualResult);
            Assert.Equal(repositoryResult.Count, actualResult.Count());
        }
    }
}

using System;

using Microsoft.AspNetCore.Mvc;

using FluentAssertions;
using Moq;
using Xunit;

using PinotageTodo.Data.Models;
using PinotageTodo.Models;

namespace PinotageTodo.Tests.Controllers.TodosControllerTests
{
    public class Get : TodosControllerTests
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
            Assert.Throws<InvalidOperationException>(() =>
                _objectUnderTest.Get(Guid.NewGuid()));
        }
        
        [Fact]
        public void Returns_BadRequestResult_If_Id_Is_Empty()
        {
            // Arrange
            var id = Guid.Empty;
            
            // Act 
            var actualResult = _objectUnderTest.Get(id);
            
            // Assert
            Assert.IsType<BadRequestResult>(actualResult);
        }
        
        [Fact]
        public void Calls_TodoRepository_Get()
        {
            // Arrange
            var id = Guid.NewGuid();

            _mockTodoRepository.Setup(x => x.Get(id, _testUserId))
                .Returns(new TodoDataModel
                {
                    Id = id,
                    IsCompleted = false,
                    Title = "A saved todo",
                    UserId = _testUserId
                });
            
            // Act
            _objectUnderTest.Get(id);
            
            // Assert
            _mockTodoRepository.Verify(
                t => t.Get(id, _testUserId), Times.Once());
        }

        [Fact]
        public void Returns_NotFoundResult_If_Repository_Get_Returns_Nothing()
        {
            // Arrange
            var id = Guid.NewGuid();

            TodoDataModel repositoryGetResult = null;
            _mockTodoRepository.Setup(x => x.Get(id, _testUserId))
                .Returns(repositoryGetResult);
            
            // Act
            var actualResult = _objectUnderTest.Get(id);
            
            // Assert
            Assert.IsType<NotFoundResult>(actualResult);
        }

        [Fact]
        public void Returns_TodoApiModel_If_Repository_Get_Returns_DataModel()
        {
            // Arrange
            var id = Guid.NewGuid();

            var repositoryGetResult = new TodoDataModel
            {
                Id = id,
                IsCompleted = false,
                Title = "A saved todo",
                UserId = _testUserId
            };
            
            _mockTodoRepository.Setup(x => x.Get(id, _testUserId))
                .Returns(repositoryGetResult);
            
            // Act
            var actualResult = _objectUnderTest.Get(id);
            
            // Assert
            Assert.IsType<ObjectResult>(actualResult);
            var objectResult = actualResult as ObjectResult;

            var expectedResult = new TodoApiModel
            {
                id = repositoryGetResult.Id,
                completed = repositoryGetResult.IsCompleted,
                title = repositoryGetResult.Title
            };

            var objectResultTodoModel = objectResult?.Value as TodoApiModel;
            objectResultTodoModel.ShouldBeEquivalentTo(expectedResult);
        }
    }
}
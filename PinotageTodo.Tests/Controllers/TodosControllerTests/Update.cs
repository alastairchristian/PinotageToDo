using System;

using Microsoft.AspNetCore.Mvc;

using Moq;
using Xunit;
using FluentAssertions;

using PinotageTodo.Models;
using PinotageTodo.Data.Models;

namespace PinotageTodo.Tests.Controllers.TodosControllerTests
{
    public class Update : TodosControllerTests
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

            var testModel = new TodoApiModel
            {
                id = Guid.NewGuid(),
                title = "My updated todo",
                completed = false
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                _objectUnderTest.Update(testModel.id, testModel));
        }
        
        [Fact]
        public void Returns_BadRequest_If_Todo_Item_Is_Null()
        {
            // Act
            var actualResult = _objectUnderTest.Update(Guid.NewGuid(), null);
            
            // Assert
            Assert.IsType<BadRequestResult>(actualResult);
        }

        [Fact]
        public void Returns_BadRequest_If_Id_From_Route_Does_Not_Match_Item_Id()
        {
            // Arrange
            var modelId = Guid.NewGuid();
            var routeId = Guid.NewGuid();
            
            var testModel = new TodoApiModel
            {
                id = modelId,
                title = "My updated todo",
                completed = false
            };
            
            // Act
            var actualResult = _objectUnderTest.Update(routeId, testModel);
            
            // Assert
            Assert.IsType<BadRequestResult>(actualResult);
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Returns_BadRequest_If_Todo_Title_Is_Not_Set(string title)
        {
            // Arrange
            var testModel = new TodoApiModel()
            {
                id = Guid.NewGuid(),
                title = title,
                completed = false
            };
            
            // Act
            var actualResult = _objectUnderTest.Update(testModel.id, testModel);
            
            // Assert
            Assert.IsType<BadRequestResult>(actualResult);
        }
        
        [Fact]
        public void Calls_TodoRepository_Update()
        {
            // Arrange
            var testModel = new TodoApiModel()
            {
                id = Guid.NewGuid(),
                title = "Todo 1",
                completed = false
            };

            TodoDataModel dataModelSentToRepository = null;
            _mockTodoRepository.Setup(t => t.Update(It.IsAny<TodoDataModel>()))
                .Callback<TodoDataModel>(m => dataModelSentToRepository = m);
            
            // Act
            _objectUnderTest.Update(testModel.id, testModel);
            
            // Assert
            _mockTodoRepository.Verify(t => t.Update(It.IsAny<TodoDataModel>()), Times.Once());

            var expectedDataModel = new TodoDataModel()
            {
                Id = testModel.id,
                IsCompleted = testModel.completed,
                Title = testModel.title,
                UserId = _testUserId
            };
            
            dataModelSentToRepository.ShouldBeEquivalentTo(expectedDataModel);
        }
        
        [Fact]
        public void Returns_NoContentResult_On_Success()
        {
            // Arrange
            var testModel = new TodoApiModel()
            {
                id = Guid.NewGuid(),
                title = "Todo 1",
                completed = false
            };
            
            // Act
            var actualResult = _objectUnderTest.Update(testModel.id, testModel);
            
            // Assert
            Assert.IsType<NoContentResult>(actualResult);
        }    
    }
}
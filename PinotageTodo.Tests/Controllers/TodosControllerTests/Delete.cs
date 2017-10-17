using System;

using Microsoft.AspNetCore.Mvc;

using Moq;
using Xunit;

namespace PinotageTodo.Tests.Controllers.TodosControllerTests
{
    public class Delete : TodosControllerTests
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
                _objectUnderTest.Delete(Guid.NewGuid()));
        }

        [Fact]
        public void Returns_BadRequestResult_If_Id_Is_Empty()
        {
            // Arrange
            var id = Guid.Empty;
            
            // Act 
            var actualResult = _objectUnderTest.Delete(id);
            
            // Assert
            Assert.IsType<BadRequestResult>(actualResult);
        }
        
        [Fact]
        public void Calls_TodoRepository_Delete()
        {
            // Arrange
            var id = Guid.NewGuid();
            
            // Act
            _objectUnderTest.Delete(id);
            
            // Assert
            _mockTodoRepository.Verify(
                t => t.Delete(id, _testUserId), Times.Once());
        }
        
        [Fact]
        public void Returns_NoContentResult_On_Success()
        {
            // Arrange
            var id = Guid.NewGuid();
            
            // Act
            var actualResult = _objectUnderTest.Delete(id);
            
            // Assert
            Assert.IsType<NoContentResult>(actualResult);
        } 
    }
}
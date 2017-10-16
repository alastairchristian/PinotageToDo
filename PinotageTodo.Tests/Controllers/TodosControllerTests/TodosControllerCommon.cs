using System;
using System.Collections.Generic;
using System.Security.Claims;

using Moq;
using Xunit;

using PinotageTodo.Controllers;
using PinotageTodo.Data.Repository;


namespace PinotageTodo.Tests.Controllers.TodosControllerTests
{
    public partial class TodosControllerTests
    {
        private readonly Mock<ITodoRepository> _mockTodoRepository;

        private Guid _testUserId = Guid.NewGuid();

        private TodosController _objectUnderTest;

        public TodosControllerTests()
        {
            _mockTodoRepository = new Mock<ITodoRepository>();
            _objectUnderTest = new TodosController(_mockTodoRepository.Object);

            _objectUnderTest.UserId = () => { return _testUserId.ToString(); };
        }

        [Fact]
        public void Can_Instantiate_Controller()
        {
            Assert.NotNull(_objectUnderTest);
        }

        private ClaimsPrincipal CreateTestUser(Guid userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userId.ToString())
            };

            var userIdentity = new ClaimsIdentity(claims, "login");
            return new ClaimsPrincipal(userIdentity);
        }
    }
}

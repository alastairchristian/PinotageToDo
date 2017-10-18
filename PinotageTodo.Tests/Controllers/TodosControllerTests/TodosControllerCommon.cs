using System;
using System.Collections.Generic;
using System.Security.Claims;

using Moq;
using Xunit;

using PinotageTodo.Controllers;
using PinotageTodo.Data.Repository;


namespace PinotageTodo.Tests.Controllers.TodosControllerTests
{
    public class TodosControllerTests
    {
        internal readonly Mock<ITodoRepository> _mockTodoRepository;

        internal Guid _testUserId = Guid.NewGuid();

        internal TodosController _objectUnderTest;

        public TodosControllerTests()
        {
            _mockTodoRepository = new Mock<ITodoRepository>();
            _objectUnderTest = new TodosController(_mockTodoRepository.Object);

            _objectUnderTest.UserId = () => { return _testUserId.ToString(); };
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

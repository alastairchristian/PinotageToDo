using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PinotageTodo.Data.Repository;
using PinotageTodo.Models;

namespace PinotageTodo.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class TodosController : Controller
    {
        private readonly ITodoRepository _todoRepository;

        public Func<string> UserId;

        public TodosController(ITodoRepository todoRepository)
        {
            _todoRepository = todoRepository;

            UserId = () => HttpContext.User.Claims.First(c => c.Type.Equals(ClaimTypes.Name)).Value;
        }

        [HttpGet]
        public IEnumerable<TodoApiModel> GetAll()
        {
            var userId = GetUserIdFromContext();

            var returnList = new List<TodoApiModel>();

            var dataModels = _todoRepository.GetAll(userId);
            if (dataModels != null)
            {
                foreach (var dataModel in dataModels)
                {
                    returnList.Add(new TodoApiModel()
                    {
                        id = dataModel.Id,
                        title = dataModel.Title,
                        completed = dataModel.IsCompleted
                    });
                }
            }

            return returnList;
        }

		[HttpPost("add", Name = "AddTodo")]
		public IActionResult Add([FromBody] TodoApiModel item)
		{
			if (item == null)
			{
				return BadRequest();
			}

			return CreatedAtRoute("GetTodo", new { id = item.id }, item);
		}

        [HttpGet("{id}", Name = "GetTodo")]
        public IActionResult Get(Guid id)
        {
            var model = new TodoApiModel() { id = id, title = "I'm a stub", completed = false };
            return new ObjectResult(model);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody] TodoApiModel item)
        {
            // this is an example of how to get the userId
            var userId = HttpContext.User.Claims.First(c => c.Type.Equals(ClaimTypes.Name)).Value;
            if (item == null || item.id != id)
            {
                return BadRequest();
            }

            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            return new NoContentResult();
        }

        private Guid GetUserIdFromContext()
        {
            string userIdString = UserId();

            if (string.IsNullOrWhiteSpace(UserId()))
            {
                throw new InvalidOperationException("No userId found in the ClaimsPrincipal");
            }

            Guid userId;

            if (!Guid.TryParse(userIdString, out userId))
            {
                throw new InvalidOperationException("Could not convert userId in ClaimsPrincipal to a guid");
            }

            return userId;
        }
    }
}

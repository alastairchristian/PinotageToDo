using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PinotageTodo.Web.Models;

namespace PinotageTodo.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class TodosController : Controller
    {
        [HttpGet]
        public IEnumerable<TodoApiModel> GetAll()
        {
            
            return new List<TodoApiModel>()
            {
                new TodoApiModel() { Id = Guid.NewGuid(), Name = "Todo 1", IsCompleted = false },
                new TodoApiModel() { Id = Guid.NewGuid(), Name = "Todo 2", IsCompleted = true }
            };
        }

		[HttpPost("add", Name = "AddTodo")]
		public IActionResult Add([FromBody] TodoApiModel item)
		{
			if (item == null)
			{
				return BadRequest();
			}

			return CreatedAtRoute("GetTodo", new { id = item.Id }, item);
		}

        [HttpGet("{id}", Name = "GetTodo")]
        public IActionResult Get(Guid id)
        {
            var model = new TodoApiModel() { Id = id, Name = "I'm a stub", IsCompleted = false };
            return new ObjectResult(model);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody] TodoApiModel item)
        {
            var userId = HttpContext.User.Claims.First(c => c.Type.Equals(ClaimTypes.Name)).Value;
            if (item == null || item.Id != id)
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
    }
}

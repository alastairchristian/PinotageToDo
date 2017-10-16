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
                new TodoApiModel() { id = Guid.NewGuid(), title = "Todo 1", completed = false },
                new TodoApiModel() { id = Guid.NewGuid(), title = "Todo 2", completed = true }
            };
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
    }
}

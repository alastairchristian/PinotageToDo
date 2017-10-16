using System;
using System.Collections.Generic;
using System.Linq;

using PinotageTodo.Data.Models;

namespace PinotageTodo.Data.Repository.EF
{
    public class EFTodoRepository : ITodoRepository
    {
        private readonly TodoContext _todoContext;

        public EFTodoRepository(TodoContext todoContext)
        {
            _todoContext = todoContext;
        }

        public void Add(TodoDataModel model)
        {
            throw new NotImplementedException();
        }

        public void Delete(Guid id, Guid userId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TodoDataModel> GetAll(Guid userId)
        {
            return _todoContext.TodoItems.Where(t => t.UserId.Equals(userId));
        }

        public void Update(TodoDataModel model)
        {
            throw new NotImplementedException();
        }
    }
}

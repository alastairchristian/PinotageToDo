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
            _todoContext.TodoItems.Add(model);
            _todoContext.SaveChanges();
        }

        public void Delete(Guid id, Guid userId)
        {
            var model = Get(id, userId);
            _todoContext.TodoItems.Remove(model);
            _todoContext.SaveChanges();
        }

        public IEnumerable<TodoDataModel> GetAll(Guid userId)
        {
            return _todoContext.TodoItems.Where(t => t.UserId.Equals(userId));
        }

        public TodoDataModel Get(Guid id, Guid userId)
        {
            return _todoContext.TodoItems.SingleOrDefault(
                t => t.UserId.Equals(userId) && t.Id.Equals(id));
        }

        public void Update(TodoDataModel model)
        {
            var existingModel = Get(model.Id, model.UserId);

            if (existingModel == null)
            {
                throw new InvalidOperationException();
            }

            existingModel.Title = model.Title;
            existingModel.IsCompleted = model.IsCompleted;

            _todoContext.TodoItems.Update(existingModel);
            _todoContext.SaveChanges();
        }
    }
}

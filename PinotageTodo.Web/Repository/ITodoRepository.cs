using System;
using System.Collections.Generic;

using PinotageTodo.Web.Models;

namespace PinotageTodo.Web.Repository
{
    public interface ITodoRepository
    {
        void Add(TodoDataModel model);

        void Update(TodoDataModel model);

        void Delete(Guid id, Guid userId);

        IEnumerable<TodoDataModel> GetAll(Guid userId);
    }
}

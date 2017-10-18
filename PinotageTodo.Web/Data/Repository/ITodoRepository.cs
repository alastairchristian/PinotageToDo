using System;
using System.Collections.Generic;

using PinotageTodo.Data.Models;

namespace PinotageTodo.Data.Repository
{
    public interface ITodoRepository
    {
        void Add(TodoDataModel model);

        void Update(TodoDataModel model);

        void Delete(Guid id, Guid userId);

        TodoDataModel Get(Guid id, Guid userId);

        IEnumerable<TodoDataModel> GetAll(Guid userId);
    }
}

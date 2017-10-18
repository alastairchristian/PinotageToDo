using System;

using Microsoft.EntityFrameworkCore;

using PinotageTodo.Data.Models;

namespace PinotageTodo.Data.Repository.EF
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options)
            :base(options)
        {
        }

        public DbSet<TodoDataModel> TodoItems { get; set; }
    }
}

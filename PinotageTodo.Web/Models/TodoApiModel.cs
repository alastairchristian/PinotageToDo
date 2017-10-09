using System;

namespace PinotageTodo.Web.Models
{
    public class TodoApiModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsCompleted { get; set; }
    }
}

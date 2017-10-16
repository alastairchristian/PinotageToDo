using System;

namespace PinotageTodo.Models
{
    public class TodoApiModel
    {
        public Guid id { get; set; }
        public string title { get; set; }
        public bool completed { get; set; }
    }
}

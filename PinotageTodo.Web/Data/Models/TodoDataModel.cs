﻿using System;

namespace PinotageTodo.Data.Models
{
    public class TodoDataModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public bool IsCompleted { get; set; }
    }
}

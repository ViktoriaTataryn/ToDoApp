using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoApp.Entities.Models
{
    public class ToDoTask
    {
        public int Id { get; set; }
        public string Title { get; set; } 
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }

        public bool IsImportant { get; set; } 
        public DateTime? DueDate { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int UserId { get; set; }
        public User? User { get; set; }

        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoApp.Entities.Models;

namespace ToDoApp.Core.DTOs
{
    public class AddTaskDto
    {
        [Required(ErrorMessage = "Назва завдання є обов'язковою")]
        public string Title { get; set; }
        public string? Description { get; set; }
        public bool IsImportant { get; set; }
        public DateTime? DueDate { get; set; }
        public bool IsCompleted = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? CategoryId { get; set; }
        public string? CategoryTitle { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoApp.Entities.Models;

namespace ToDoApp.Core.DTOs
{
    public class UpdateTaskDto
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public bool IsImportant { get; set; }
        public DateTime? DueDate { get; set; }

        public int? CategoryId { get; set; }
        public string? CategoryTitle { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoApp.Entities.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }
        public ICollection<ToDoTask> Tasks { get; set; } = new List<ToDoTask>();
    }
}

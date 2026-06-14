using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoApp.Core.DTOs
{
    public class AddCategoryDto
    {
        [Required(ErrorMessage = "Назва категорії є обов'язковою")]
        public string Title { get; set; }
      
    }
}

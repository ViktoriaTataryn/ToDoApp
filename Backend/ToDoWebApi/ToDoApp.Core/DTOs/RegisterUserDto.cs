using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoApp.Core.DTOs
{
    public class RegisterUserDto
    {
        [Required(ErrorMessage = "Ім'я є обов'язковим для заповнення")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Ім'я повинно містити від 2 до 50 символів")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Прізвище є обов'язковим для заповнення")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Прізвище повинно містити від 2 до 50 символів")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Email є обов'язковим")]
        [EmailAddress(ErrorMessage = "Некоректний формат Email адреси")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Пароль є обов'язковим")]
     
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

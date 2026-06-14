using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoApp.Core.DTOs
{
    public class AuthResultDto
    {
       
        
            public bool IsSuccess { get; set; }

         
            public string? Token { get; set; }

           
            public string? ErrorMessage { get; set; }

        
            public int? UserId { get; set; }
            public string? Email { get; set; }
            public string? FirstName { get; set; }
        }
    }


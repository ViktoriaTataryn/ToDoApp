using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoApp.Core.DTOs;
using ToDoApp.Entities.Models;

namespace ToDoApp.Core.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterUserDto registerUserDTO, CancellationToken cancelationToken = default);
        Task<string> LoginAsync(string email, string password, CancellationToken cancelationToken = default);
        Task<string> LogoutAsync( CancellationToken cancelationToken = default);
    }
}

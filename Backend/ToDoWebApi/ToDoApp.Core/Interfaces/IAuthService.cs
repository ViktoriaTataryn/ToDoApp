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
        Task<UserDto> RegisterAsync(RegisterUserDto registerUserDTO, CancellationToken cancelationToken = default);
        Task<AuthResultDto> LoginAsync(LoginDto loginDto, CancellationToken cancelationToken = default);
       
    }
}

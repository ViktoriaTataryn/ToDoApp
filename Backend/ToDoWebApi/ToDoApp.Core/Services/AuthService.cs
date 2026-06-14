using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ToDoApp.Core.DTOs;
using ToDoApp.Core.Interfaces;
using ToDoApp.Entities.Models;
using ToDoApp.Storage;
using BC = BCrypt.Net.BCrypt;

namespace ToDoApp.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly ToDoContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ToDoContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResultDto> LoginAsync(LoginDto loginDto, CancellationToken cancelationToken = default)
        {
           
            var user =await _context.Users.FirstOrDefaultAsync(u=>u.Email==loginDto.Email,cancelationToken);
            if (user == null) {
                return new AuthResultDto { IsSuccess = false, ErrorMessage = "Неправильний email " };
            }
            //  Перевіряємо, чи введений пароль підходить під хеш у базі
            bool isPasswordValid = BC.Verify(loginDto.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                return new AuthResultDto { IsSuccess = false, ErrorMessage = "Неправильний  пароль" };
            }

                string token = GenerateJwtToken(user);

            return new AuthResultDto
            {
                IsSuccess = true,
                Token = token,
                Email = user.Email,
                FirstName = user.FirstName,
            };



        }

        public async Task<UserDto> RegisterAsync(RegisterUserDto registerUserDTO, CancellationToken cancelationToken = default)
        {
           var user =await _context.Users.AnyAsync(u=>u.Email==registerUserDTO.Email, cancelationToken);
            if (user)
            {
                throw new ArgumentException("User is alredy register");
            }
            string passwordHash = BC.HashPassword(registerUserDTO.PasswordHash);

            var newUser = new User
            {
                Email = registerUserDTO.Email,
                FirstName = registerUserDTO.FirstName,
                LastName = registerUserDTO.LastName,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow,
            };
            _context.Users.Add(newUser);
           await _context.SaveChangesAsync(cancelationToken);

            return new UserDto
            {
                Id = newUser.Id,
                Email = registerUserDTO.Email,
                FirstName = registerUserDTO.FirstName,
                LastName = registerUserDTO.LastName,
            };

}

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            // Беремо ключ з нашого appsettings.json
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Secret"]);

            // Описуємо "Клейми" (Claims) — інформацію всередині токена, яку зможе прочитати фронтенд
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Зашиваємо ID юзера!
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("FullName", $"{user.FirstName} {user.LastName}")
            }),
                Expires = DateTime.UtcNow.AddDays(7), // Токен буде дійсний 7 днів
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }

    }


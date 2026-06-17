
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using ToDoApp.Core.Interfaces;
using ToDoApp.Core.Services;
using ToDoApp.Storage;

namespace ToDoWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddDbContext<ToDoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // --- НАЛАШТУВАННЯ JWT АВТЕНТИФІКАЦІЇ ---
            var jwtSecret = builder.Configuration["JwtSettings:Secret"];
            if (string.IsNullOrEmpty(jwtSecret))
            {
                throw new Exception("JWT Secret key is missing in configuration!");
            }
            var key = Encoding.ASCII.GetBytes(jwtSecret);

            // 2. Підключаємо схему автентифікації JwtBearer
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; 
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),

                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],

                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],

                    ValidateLifetime = true, // Перевіряти, чи не збіг термін дії токена
                    ClockSkew = TimeSpan.Zero // Прибираємо затримку часу при перевірці токена
                };
            });

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IToDoService, ToDoService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "TodoApp API", Version = "v1" });

             
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Введіть JWT токен у форматі: Bearer {твій_токен}"
                });

              
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
            });
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AngularClient", policy =>
                {
                    policy.WithOrigins("http://localhost:4200") // Адреса  Angular додатка
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("AngularClient");

            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

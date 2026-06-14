using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ToDoApp.Core.DTOs;
using ToDoApp.Core.Interfaces;
using ToDoApp.Entities.Models;
using ToDoApp.Storage;

namespace ToDoApp.Core.Services
{
    public class ToDoService : IToDoService
    {
        private readonly ToDoContext _context;

        public ToDoService(ToDoContext context)
        {
            _context = context;
        }

        public async Task<ToDoTaskDto> AddTaskAsync(int userId, AddTaskDto addTaskDto, CancellationToken cancellationToken = default)
        {
            var newTask = new ToDoTask
            {
                Title = addTaskDto.Title,
                Description = addTaskDto.Description,
                IsImportant = addTaskDto.IsImportant,
                IsCompleted = addTaskDto.IsCompleted,
                DueDate = addTaskDto.DueDate,
                CreatedAt = DateTime.UtcNow,
                UserId = userId,
                CategoryId = addTaskDto.CategoryId,
            };
            _context.Tasks.Add(newTask);
            await _context.SaveChangesAsync();

            string categoryTitle = null;
            if (newTask.CategoryId.HasValue)
            {
                var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == newTask.CategoryId, cancellationToken);
                categoryTitle = category?.Title;
            }

            return new ToDoTaskDto
            {
                Id = newTask.Id,
                Title = newTask.Title,
                Description = newTask.Description,
                IsImportant = newTask.IsImportant,
                IsCompleted = newTask.IsCompleted,
                DueDate = newTask.DueDate,
                CreatedAt = newTask.CreatedAt,
                CategoryTitle = categoryTitle,

            };
        }


        public async Task DeleteTaskAsync(int userId, int taskId, CancellationToken cancellationToken = default)
        {
            var taskToDelete = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId, cancellationToken);
            if (taskToDelete == null) {
                throw new KeyNotFoundException("Task not found");
            }
            _context.Tasks.Remove(taskToDelete);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ToDoTaskDto>> GetTasksAsync(int userId, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _context.Tasks
          .AsNoTracking()
          .Where(x => x.UserId == userId)
          .OrderByDescending(t => t.IsImportant)
        .ThenByDescending(t => t.CreatedAt);

            var pagedQuery = query
        .Skip((page - 1) * pageSize)
        .Take(pageSize);

            return await pagedQuery.Select(x => new ToDoTaskDto
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                IsCompleted = x.IsCompleted,
                IsImportant = x.IsImportant,
                CreatedAt = x.CreatedAt,
                DueDate = x.DueDate,
                CategoryTitle = x.Category != null ? x.Category.Title : null,
            }).ToListAsync(cancellationToken);

        }

        public async Task<ToDoTaskDto> UpdateTaskAsync(int userId, int taskId, UpdateTaskDto updateTaskDto, CancellationToken cancellationToken = default)
        {
            var task = await _context.Tasks
         .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId, cancellationToken);

            if (task == null)
            {
                throw new KeyNotFoundException("Task not found");
            }
           task.Title = updateTaskDto.Title;
            task.Description = updateTaskDto.Description;
            task.IsImportant = updateTaskDto.IsImportant;
            task.DueDate = updateTaskDto.DueDate;
            task.CategoryId = updateTaskDto.CategoryId;

           await _context.SaveChangesAsync(cancellationToken);

            string categoryTitle = null;
            if (task.CategoryId.HasValue)
            {
                var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == task.CategoryId, cancellationToken);
                categoryTitle = category?.Title;
            }

            return new ToDoTaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                IsImportant = task.IsImportant,
                IsCompleted = task.IsCompleted,
                DueDate = task.DueDate,
                CategoryTitle = categoryTitle,

            };
        }

      
    }
}

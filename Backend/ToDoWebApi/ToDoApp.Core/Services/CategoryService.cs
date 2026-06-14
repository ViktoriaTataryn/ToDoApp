using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoApp.Core.DTOs;
using ToDoApp.Core.Interfaces;
using ToDoApp.Entities.Models;
using ToDoApp.Storage;

namespace ToDoApp.Core.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ToDoContext _context;

        public CategoryService(ToDoContext context)
        {
            _context = context;
        }
        public async Task<CategoryDto> AddCategoryAsync(int userId, AddCategoryDto addCategoryDto, CancellationToken cancellationToken = default)
        {
            var isDuplicate = await _context.Categories
        .AnyAsync(c => c.UserId == userId && c.Title.ToLower() == addCategoryDto.Title.ToLower(), cancellationToken);

            if (isDuplicate)
            {
             
                throw new InvalidOperationException($"Категорія з назвою '{addCategoryDto.Title}' вже існує.");
            }
            var newCategory = new Category
            {
                Title = addCategoryDto.Title,
                UserId = userId,
            };
            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();
            return new CategoryDto
            {
                Id = newCategory.Id,
                Title = newCategory.Title,
            };
        }

        public async Task DeleteCategoryAsync(int userId, int categoryId, CancellationToken cancellationToken = default)
        {
            var category =await _context.Categories.FirstOrDefaultAsync(c => c.UserId == userId && c.Id == categoryId, cancellationToken);
            if (category == null) {
                throw new KeyNotFoundException("Category was not found");            
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync(int userId,  CancellationToken cancellationToken = default)
        {
            var allCategory = _context.Categories.AsNoTracking().Where(u=>u.UserId==userId);
            return await allCategory.Select(c=> new CategoryDto
            {
                Id = c.Id,
                Title = c.Title,
            }).ToListAsync(cancellationToken);
            
        }
    }
}

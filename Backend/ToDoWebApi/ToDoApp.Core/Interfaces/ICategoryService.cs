using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoApp.Core.DTOs;
using ToDoApp.Entities.Models;

namespace ToDoApp.Core.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryDto> AddCategoryAsync(int userId, AddCategoryDto addCategoryDto, CancellationToken cancellationToken = default);
        Task<IEnumerable<CategoryDto>> GetCategoriesAsync(int userId,  CancellationToken cancellationToken = default);
        Task DeleteCategoryAsync(int userId, int categoryId, CancellationToken cancellationToken = default);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoApp.Core.DTOs;
using ToDoApp.Entities.Models;

namespace ToDoApp.Core.Interfaces
{
    public interface ICategory
    {
        Task<CategoryDto> AddCategoryAsync(int userId, AddCategoryDto addCategoryDto, CancellationToken cancellationToken = default);
        Task<IEnumerable<CategoryDto>> GetCategoriesAsync(int userId, int page, int pageSize, CancellationToken cancellationToken = default);
        Task<bool> DeleteCategoryAsync(int userId, int taskId, CancellationToken cancellationToken = default);
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Sockets;
using System.Security.Claims;
using ToDoApp.Core.DTOs;
using ToDoApp.Core.Interfaces;

namespace ToDoWebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/tasks")]
    public class ToDoController : Controller
    {
        private readonly IToDoService _toDoService;
        private readonly ICategoryService _categoryService;

        public ToDoController(IToDoService toDoService, ICategoryService categoryService)
        {
            _toDoService = toDoService;
            _categoryService = categoryService;
        }
        private int CurrentUserId => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new UnauthorizedAccessException("Користувач не авторизований"));

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ToDoTaskDto>>> GetTasksAsync(int page=1, int pageSize=10,
      CancellationToken cancellationToken = default)
        {
            var tasks = await _toDoService.GetTasksAsync(CurrentUserId,page, pageSize, cancellationToken);
            return Ok(tasks);
        }
        [HttpPost]
        public async Task<ActionResult<ToDoTaskDto>> AddTaskAsync([FromBody]  AddTaskDto addTaskDto, CancellationToken cancellationToken = default)
        {
            var task = await _toDoService.AddTaskAsync(CurrentUserId, addTaskDto, cancellationToken);
            return Created($"api/tasks/{task.Id}", task);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<ToDoTaskDto>> UpdateTaskAsync([FromRoute] int id, [FromBody] UpdateTaskDto updateTaskDto, CancellationToken cancellationToken = default)
        {
            var task = await _toDoService.UpdateTaskAsync(CurrentUserId, id,updateTaskDto, cancellationToken);
            return Ok(task);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTask([FromRoute] int id, CancellationToken cancellationToken=default)
        {
            await _toDoService.DeleteTaskAsync(CurrentUserId, id, cancellationToken);
            return NoContent();
        }

        [HttpGet("/category")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories( 
      CancellationToken cancellationToken = default)
        {
            var categories = await _categoryService.GetCategoriesAsync(CurrentUserId, cancellationToken);
            return Ok(categories);
        }
        [HttpPost("/category")]
        public async Task<ActionResult<CategoryDto>> AddCategory( [FromBody] AddCategoryDto addCategoryDto, CancellationToken cancellationToken = default)
        {
            var category = await _categoryService.AddCategoryAsync(CurrentUserId, addCategoryDto, cancellationToken);
            return Created($"api/tasks/{category.Id}", category);
        }
        [HttpDelete("/category/{id}")]
        public async Task<ActionResult> DeleteCategory( [FromRoute] int id, CancellationToken cancellationToken = default)
        {
            await _categoryService.DeleteCategoryAsync( CurrentUserId,id, cancellationToken);
            return NoContent();
        }
    }
}

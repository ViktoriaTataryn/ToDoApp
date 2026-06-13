using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoApp.Core.DTOs;

namespace ToDoApp.Core.Interfaces
{
    public interface IToDoService
    {
        Task<ToDoTaskDto> AddTaskAsync(int userId, AddTaskDto addTaskDto, CancellationToken cancellationToken = default);
        Task<IEnumerable<ToDoTaskDto>> GetTasksAsync(int userId, int page, int pageSize, CancellationToken cancellationToken = default);
        Task<ToDoTaskDto> UpdateTaskAsync(int userId, int taskId, UpdateTaskDto updateTaskDto, CancellationToken cancellationToken = default);
        Task<bool> DeleteTaskAsync(int userId, int taskId, CancellationToken cancellationToken = default);
    }
}

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkPlanner.Api.Data;
using WorkPlanner.Api.Models;

namespace WorkPlanner.Api.Controllers;

[ApiController]
[Route("api/tasks/{taskId:int}/comments")]
[Authorize]
public class TaskCommentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public TaskCommentsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskCommentDto>>> GetComments(int taskId)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var task = await _context.TaskItems
            .AsNoTracking()
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task == null)
        {
            return NotFound();
        }

        if (!task.ProjectId.HasValue)
        {
            return BadRequest("Task is missing ProjectId.");
        }

        var hasAccess = await _context.ProjectMembers
            .AnyAsync(m => m.ProjectId == task.ProjectId.Value && m.UserId == userId);

        if (!hasAccess)
        {
            return Forbid();
        }

        var comments = await _context.TaskComments
            .AsNoTracking()
            .Where(c => c.TaskItemId == taskId)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new TaskCommentDto
            {
                Id = c.Id,
                TaskItemId = c.TaskItemId,
                AuthorId = c.AuthorId,
                AuthorName = string.Join(" ", new[] { c.Author.FirstName, c.Author.LastName }.Where(n => !string.IsNullOrWhiteSpace(n))).Trim(),
                AuthorEmail = c.Author.Email ?? string.Empty,
                Body = c.Body,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync();

        return comments;
    }

    [HttpPost]
    public async Task<ActionResult<TaskCommentDto>> CreateComment(int taskId, CreateTaskCommentRequest request)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(request.Body))
        {
            return BadRequest("Comment body is required.");
        }

        var task = await _context.TaskItems
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task == null)
        {
            return NotFound();
        }

        if (!task.ProjectId.HasValue)
        {
            return BadRequest("Task is missing ProjectId.");
        }

        var hasAccess = await _context.ProjectMembers
            .AnyAsync(m => m.ProjectId == task.ProjectId.Value && m.UserId == userId);

        if (!hasAccess)
        {
            return Forbid();
        }

        var comment = new TaskComment
        {
            TaskItemId = taskId,
            AuthorId = userId,
            Body = request.Body.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _context.TaskComments.Add(comment);
        await _context.SaveChangesAsync();

        var author = await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new { u.FirstName, u.LastName, u.Email })
            .FirstAsync();

        var dto = new TaskCommentDto
        {
            Id = comment.Id,
            TaskItemId = comment.TaskItemId,
            AuthorId = comment.AuthorId,
            AuthorName = string.Join(" ", new[] { author.FirstName, author.LastName }.Where(n => !string.IsNullOrWhiteSpace(n))).Trim(),
            AuthorEmail = author.Email ?? string.Empty,
            Body = comment.Body,
            CreatedAt = comment.CreatedAt
        };

        return CreatedAtAction(nameof(GetComments), new { taskId }, dto);
    }

    private string? GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}

public class CreateTaskCommentRequest
{
    public string Body { get; set; } = string.Empty;
}

public class TaskCommentDto
{
    public int Id { get; set; }
    public int TaskItemId { get; set; }
    public string AuthorId { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorEmail { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

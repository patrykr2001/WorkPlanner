using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkPlanner.Api.Data;
using WorkPlanner.Api.Models;

namespace WorkPlanner.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkEntriesController : ControllerBase
{
    private readonly AppDbContext _context;

    public WorkEntriesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkEntry>>> GetWorkEntries()
    {
        return await _context.WorkEntries
            .Include(we => we.TaskItem)
            .OrderByDescending(we => we.StartTime)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WorkEntry>> GetWorkEntry(int id)
    {
        var workEntry = await _context.WorkEntries
            .Include(we => we.TaskItem)
            .FirstOrDefaultAsync(we => we.Id == id);

        if (workEntry == null)
        {
            return NotFound();
        }

        return workEntry;
    }

    [HttpGet("by-task/{taskId}")]
    public async Task<ActionResult<IEnumerable<WorkEntry>>> GetWorkEntriesByTask(int taskId)
    {
        return await _context.WorkEntries
            .Where(we => we.TaskItemId == taskId)
            .OrderByDescending(we => we.StartTime)
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<WorkEntry>> CreateWorkEntry(WorkEntry workEntry)
    {
        workEntry.CreatedAt = DateTime.UtcNow;
        _context.WorkEntries.Add(workEntry);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetWorkEntry), new { id = workEntry.Id }, workEntry);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateWorkEntry(int id, WorkEntry workEntry)
    {
        if (id != workEntry.Id)
        {
            return BadRequest();
        }

        _context.Entry(workEntry).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!WorkEntryExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWorkEntry(int id)
    {
        var workEntry = await _context.WorkEntries.FindAsync(id);
        if (workEntry == null)
        {
            return NotFound();
        }

        _context.WorkEntries.Remove(workEntry);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool WorkEntryExists(int id)
    {
        return _context.WorkEntries.Any(e => e.Id == id);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodosAPI24.Data;
using TodosAPI24.Models;

namespace TodosAPI24.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodosController : ControllerBase
    {
        private readonly TodosAPI24Context _db;

        public TodosController(TodosAPI24Context context)
        {
            _db = context;
        }

        // GET: api/Todos
        [HttpGet, Authorize]
        public async Task<ActionResult<IEnumerable<Todo>>> GetTodos()
        {
            return await _db.Todos
                .Where(t => t.UserId == GetUserId())
                .ToListAsync();
        }

        // GET: api/Todos/5
        [HttpGet("{id}"), Authorize]
        public async Task<ActionResult<Todo>> GetTodo(int id)
        {
            var todo = await _db.Todos
                .FirstOrDefaultAsync(t => t.UserId == GetUserId() && t.Id == id);

            if (todo == null)
            {
                return NotFound();
            }

            return todo;
        }

        // PUT: api/Todos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> PutTodo(int id, Todo update)
        {
            var original = await _db.Todos
                .Where(t => t.Id == id)
                .FirstOrDefaultAsync();
            

            if (id != update.Id || original?.UserId != GetUserId())
            {
                return BadRequest();
            }



            if (original?.Done == false && update.Done == true)
            {
                update.CompletedAt = DateTime.Now;
            }

            // Map properties from update to original
            _db.Entry(original).CurrentValues.SetValues(update);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Todos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost, Authorize]
        public async Task<ActionResult<Todo>> PostTodo(Todo todo)
        {
            todo.UserId = GetUserId();

            _db.Todos.Add(todo);
            await _db.SaveChangesAsync();

            return CreatedAtAction("GetTodo", new { id = todo.Id }, todo);
        }

        // DELETE: api/Todos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            var todo = await _db.Todos.FindAsync(id);
            if (todo == null)
            {
                return NotFound();
            }

            if(todo.UserId != GetUserId())
            {
                return BadRequest();
            }

            _db.Todos.Remove(todo);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        private bool TodoExists(int id)
        {
            return _db.Todos.Any(e => e.Id == id);
        }

        private string? GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}

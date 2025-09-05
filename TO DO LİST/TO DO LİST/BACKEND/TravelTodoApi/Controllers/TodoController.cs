using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelTodoApi.Models;
using TravelTodoApi.Data;
using System.Security.Claims;

namespace TravelTodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly TravelTodoDbContext _context;

        public TodoController(TravelTodoDbContext context)
        {
            _context = context;
        }

        // GET: api/Todo
        [HttpGet]
        [Authorize] // JWT doğrulaması
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)); // JWT'den kullanıcı ID'sini al
            return await _context.TodoItems
                .Where(t => t.UserId == userId) // Kullanıcıya ait olan todo item'larını getir
                .Include(t => t.User)
                .ToListAsync();
        }

        // GET: api/Todo/5
        [HttpGet("{id}")]
        [Authorize] // JWT doğrulaması
        public async Task<ActionResult<TodoItem>> GetTodoItem(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)); // JWT'den kullanıcı ID'sini al
            var todoItem = await _context.TodoItems
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId); // Kullanıcıya ait olan todo item'ını getir

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }

        // POST: api/Todo/add
        [HttpPost("add")]
        [Authorize] // JWT doğrulaması
        public async Task<IActionResult> AddTodoItem([FromBody] TodoItemRequest request)
        {
            if (request == null)
            {
                return BadRequest("Geçersiz veri.");
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)); // JWT'den kullanıcı ID'sini al
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }

            var todoItem = new TodoItem
            {
                City = request.City,
                TravelDate = DateTime.Parse(request.TravelDate),
                UserId = userId,
                IsCompleted = request.IsCompleted
            };

            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            return Ok(todoItem); // Eklenen todo item'ı geri döndür
        }

        // PUT: api/Todo/5
        [HttpPut("{id}")]
        [Authorize] // JWT doğrulaması
        public async Task<IActionResult> UpdateTodoItem(int id, [FromBody] TodoItemRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)); // JWT'den kullanıcı ID'sini al
            var todoItem = await _context.TodoItems
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId); // Kullanıcıya ait todo item'ını bul

            if (todoItem == null)
            {
                return NotFound();
            }

            todoItem.City = request.City;
            todoItem.TravelDate = DateTime.Parse(request.TravelDate);
            todoItem.IsCompleted = request.IsCompleted;

            _context.TodoItems.Update(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Todo/5
        [HttpDelete("{id}")]
        [Authorize] // JWT doğrulaması
        public async Task<IActionResult> DeleteTodoItem(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)); // JWT'den kullanıcı ID'sini al
            var todoItem = await _context.TodoItems
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId); // Kullanıcıya ait todo item'ını bul

            if (todoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    public class TodoItemRequest
    {
        public string City { get; set; }
        public string TravelDate { get; set; }
        public bool IsCompleted { get; set; }  // İsteğe bağlı olarak tamamlanmış olup olmadığını belirlemek için
    }
}

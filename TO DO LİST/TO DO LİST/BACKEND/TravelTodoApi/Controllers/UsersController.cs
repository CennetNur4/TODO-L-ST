using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TravelTodoApi.Data;
using TravelTodoApi.Models;
using TravelTodoApi.Services;

namespace TravelTodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly TravelTodoDbContext _context;
        private readonly TokenService _tokenService;

        public UsersController(TravelTodoDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Kullanıcıyı e-posta ile ara
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                return Unauthorized("Geçersiz e-posta veya şifre.");
            }

            // Şifreyi kontrol et
            if (user.Password != request.Password)
            {
                return Unauthorized("Geçersiz e-posta veya şifre.");
            }

            // JWT token oluştur
            var token = _tokenService.GenerateToken(user);

            // Başarılı giriş, token ve kullanıcı ID ile birlikte döndür
            return Ok(new
            {
                token,
                userId = user.Id,
                message = "Giriş başarılı!"
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // Eğer aynı e-posta ile bir kullanıcı zaten varsa, hata mesajı döndür
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest("Bu e-posta adresi zaten kayıtlı.");
            }

            // Yeni kullanıcı oluşturma
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                Birthdate = request.Birthdate,
                Password = request.Password // Şifreyi burada hash'lemek gerekebilir
            };

            // Veritabanına ekle
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Kayıt başarılı.");
        }
    }
}

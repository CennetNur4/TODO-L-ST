namespace TravelTodoApi.Models
{
    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime Birthdate { get; set; }
        public string Password { get; set; } = string.Empty;
    }
}

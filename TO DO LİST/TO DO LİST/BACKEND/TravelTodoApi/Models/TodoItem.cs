using Newtonsoft.Json;

namespace TravelTodoApi.Models
{
    public class TodoItem
    {
        public int Id { get; set; }
        public string City { get; set; }
        public DateTime TravelDate { get; set; }
        public bool IsCompleted { get; set; }
        public string? Comment { get; set; }
        public string? PhotoPath { get; set; }
        public int UserId { get; set; }

        [JsonIgnore]  // Sonsuz döngüyü engellemek için
        public User User { get; set; }
    }
}

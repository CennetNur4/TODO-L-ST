using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TravelTodoApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime Birthdate { get; set; }

        [JsonIgnore]  // Sonsuz döngüyü engellemek için
        public ICollection<TodoItem> TodoItems { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;
using TravelTodoApi.Models;

namespace TravelTodoApi.Data
{
    public class TravelTodoDbContext : DbContext
    {
        public TravelTodoDbContext(DbContextOptions<TravelTodoDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<TodoItem> TodoItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TodoItem>()
                .HasOne(t => t.User)
                .WithMany(u => u.TodoItems)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

using FirstASP.Models;
using Microsoft.EntityFrameworkCore;

namespace FirstASP.Data
{
    public class FirstAPIContext(DbContextOptions<FirstAPIContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Book>().HasData(
                new Book
                {
                    Id = 1,
                    Title = "The Great Gatsby",
                    Author = "F. Scott Fitzgerald",
                    YearPublished = 1925
                }
            );
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }
    }
}

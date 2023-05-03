using Elearn.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Elearn.Data
{
    public class AppDbContext: IdentityDbContext<AppUser>
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Slider> Slider { get; set; }

        public DbSet<CourseImage> CourseImages { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<CourseHost> CourseHosts { get; set; }

        public DbSet<Event> Events { get; set; }

        public DbSet<Publisher> Publishers { get; set; }

        public DbSet<News> News { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

    }
}

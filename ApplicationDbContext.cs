using Microsoft.EntityFrameworkCore;
using Movie_App_MinimalApi.Entity;

namespace Movie_App_MinimalApi
{
    public class ApplicationDbContext : DbContext
    {
        //purpose of this constructor is to pass the options to the base class which is DbContext and defined in EntityFrameworkCore
        public ApplicationDbContext(DbContextOptions options): base(options)
        {
            
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Genre>().Property(p => p.Name).HasMaxLength(150);
            modelBuilder.Entity<Actor>().Property(p => p.Name).HasMaxLength(200);
            modelBuilder.Entity<Actor>().Property(p => p.ActorPic).IsUnicode();//to store the image as unicode string.


        }

        //DbSet represent the table in the database
        DbSet<Genre> Genres { get; set; }
        public DbSet<Actor> Actors { get; set; }

    }
}

using Microsoft.EntityFrameworkCore;
using SpotifyLibraryManager.Models;

namespace SpotifyLibraryManager.Database
{
    public class LibraryContext : DbContext
    {
        public DbSet<Album> Albums { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Tag> Tags { get; set; }

        public LibraryContext() : base()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source="+ System.AppDomain.CurrentDomain.BaseDirectory + @"\Library.db");
    }
}

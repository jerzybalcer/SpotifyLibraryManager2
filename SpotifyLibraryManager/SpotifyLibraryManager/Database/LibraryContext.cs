using Microsoft.EntityFrameworkCore;
using SpotifyLibraryManager.Models;
using System;
using System.IO;

namespace SpotifyLibraryManager.Database
{
    public class LibraryContext : DbContext
    {
        private string _databaseLocationPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\SpotifyLibraryManager";

        public DbSet<Album> Albums { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Tag> Tags { get; set; }

        public LibraryContext() : base()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            Directory.CreateDirectory(_databaseLocationPath);
            options.UseSqlite("Data Source=" + _databaseLocationPath + @"\Library.db");
        }
    }
}

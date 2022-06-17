using Microsoft.EntityFrameworkCore;
using SpotifyLibraryManager.Database;
using SpotifyLibraryManager.Models;
using System;
using System.Collections.ObjectModel;

namespace SpotifyLibraryManager.ViewModels
{
    public class LibraryManager : ViewModelBase
    {
        public ObservableCollection<Album> AllAlbums { get; set; }
        public ObservableCollection<Album> VisibleAlbums { get; set; }
        public ObservableCollection<Tag> AllTags { get; set; }
        public ObservableCollection<Tag> Filters { get; set; }

        private Album _selectedAlbum;
        public Album SelectedAlbum
        {
            get { return _selectedAlbum; }
            set { _selectedAlbum = value; AlbumSelected?.Invoke(this, EventArgs.Empty); }
        }

        public event EventHandler AlbumSelected;

        public LibraryManager()
        {
            LoadAllAlbums();
            LoadAllTags();
            Filters = new ObservableCollection<Tag>();
        }
        public async void LoadAllAlbums()
        {
            var albumsFromDb = await AlbumsManager.GetAlbumsFromDb();
            AllAlbums = new ObservableCollection<Album>(albumsFromDb);
            VisibleAlbums = new ObservableCollection<Album>(albumsFromDb);
        }
        public async void LoadAllTags()
        {
            using (var db = new LibraryContext())
            {
                AllTags = new ObservableCollection<Tag>(await db.Tags.ToListAsync());
            }
        }
    }
}

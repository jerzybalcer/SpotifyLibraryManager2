using Microsoft.EntityFrameworkCore;
using SpotifyLibraryManager.Database;
using SpotifyLibraryManager.Models;
using System.Collections.ObjectModel;

namespace SpotifyLibraryManager.ViewModels
{
    public class LibraryManager : ViewModelBase
    {
        // events to handle
        // all tags change (add, remove), affected: toolbar, details
        // filters change (check, uncheck), affected: toolbar, albumsList
        // albums change (sort, filter, search, sync), affected: albumsList, details
        // album change (tag assigned, tag removed), affected: details; album as parameter

        public ObservableCollection<Album> AllAlbums { get; set; }
        public ObservableCollection<Album> VisibleAlbums { get; set; }
        public ObservableCollection<Tag> AllTags { get; set; }
        public ObservableCollection<Tag> Filters { get; set; }
        public Album SelectedAlbum { get; set; }

        public LibraryManager()
        {
            LoadAllAlbums();
            LoadAllTags();
            Filters = new ObservableCollection<Tag>();
        }

        ////////////////////////////////////////////////////////////
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

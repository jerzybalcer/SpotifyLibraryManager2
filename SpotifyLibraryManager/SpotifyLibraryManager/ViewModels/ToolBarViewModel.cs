using SpotifyLibraryManager.Helpers;
using SpotifyLibraryManager.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SpotifyLibraryManager.ViewModels
{
    public class ToolBarViewModel : ViewModelBase
    {
        public LibraryManager LibraryManager { get; private set; }
        public string SearchPhrase { get; set; }
        public Command SearchCommand { get; set; }
        public Command SyncCommand { get; set; }
        public Command LoginCommand { get; set; }

        public void SearchAlbums(object param)
        {
            if (string.IsNullOrEmpty(SearchPhrase))
            {
                LibraryManager.VisibleAlbums = new ObservableCollection<Album>(LibraryManager.AllAlbums);
                return;
            }

            List<Album> matching = new List<Album>();

            foreach (var album in LibraryManager.AllAlbums)
            {
                foreach (var artist in album.Artists)
                {
                    if (artist.Name.ToLower().Contains(SearchPhrase.ToLower()))
                    {
                        matching.Add(album);
                        continue;
                    }
                }

                if (album.Title.ToLower().Contains(SearchPhrase.ToLower()))
                {
                    matching.Add(album);
                }
            }

            LibraryManager.VisibleAlbums = new ObservableCollection<Album>(matching);
        }

        public void FilterAlbums(object param)
        {
            var matching = new List<Album>();

            foreach (var album in LibraryManager.AllAlbums)
            {
                if (LibraryManager.Filters.All(filter => album.Tags.Any(tag => tag.Name == filter.Name)))
                {
                    matching.Add(album);
                }
            }

            LibraryManager.VisibleAlbums = new ObservableCollection<Album>(matching);
        }
        
        public void SortAlbums(object param)
        {

        }

        public async void SyncAlbums(object param)
        {
            var syncedAlbums = await AlbumsManager.SyncAlbums();
            LibraryManager.AllAlbums = new ObservableCollection<Album>(syncedAlbums);
            LibraryManager.VisibleAlbums = new ObservableCollection<Album>(syncedAlbums);
        }

        public async void LoginToSpotify(object param)
        {
            await Spotify.StartAuthentication();
            SyncAlbums(null);
            LibraryManager.LoadAllTags();
        }

        public ToolBarViewModel(LibraryManager libraryManager)
        {
            LibraryManager = libraryManager;
            LibraryManager.Filters.CollectionChanged += (s, e) => FilterAlbums(null);

            SearchCommand = new Command(SearchAlbums);
            SyncCommand = new Command(SyncAlbums);
            LoginCommand = new Command(LoginToSpotify);
        }
    }
}

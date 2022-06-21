using SpotifyLibraryManager.Helpers;
using SpotifyLibraryManager.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SpotifyLibraryManager.ViewModels
{
    public class ToolBarViewModel : ViewModelBase
    {
        private string _sortBy = "LikeDate";

        public LibraryManager LibraryManager { get; private set; }
        public bool IsSortingAscending { get; set; }
        public string SearchPhrase { get; set; }
        public FilterType FilterType { get; set; } = FilterType.AllMatching;
        public string SortBy
        {
            get { return _sortBy; }
            set { _sortBy = value; SortAlbums(null); }
        }
        public Command SearchCommand { get; set; }
        public Command ToggleSortingDirectionCommand { get; set; }
        public Command SyncCommand { get; set; }
        public Command LoginCommand { get; set; }
        public Command ChangeFilterTypeCommand { get; set; }

        public void SearchAlbums(object param)
        {
            string searchPhrase = (string)param;

            if (string.IsNullOrEmpty(searchPhrase))
            {
                LibraryManager.VisibleAlbums = new ObservableCollection<Album>(LibraryManager.AllAlbums);
                return;
            }

            List<Album> matching = new List<Album>();

            foreach (var album in LibraryManager.AllAlbums)
            {
                foreach (var artist in album.Artists)
                {
                    if (artist.Name.ToLower().Contains(searchPhrase.ToLower()))
                    {
                        matching.Add(album);
                        continue;
                    }
                }

                if (album.Title.ToLower().Contains(searchPhrase.ToLower()))
                {
                    if (!matching.Contains(album))
                    {
                        matching.Add(album);
                    }
                }
            }

            LibraryManager.VisibleAlbums = new ObservableCollection<Album>(matching);
        }

        public void FilterAlbums(object param)
        {
            var matching = new List<Album>();

            foreach (var album in LibraryManager.AllAlbums)
            {
                if (FilterType == FilterType.AllMatching)
                {
                    if (LibraryManager.Filters.All(filter => album.Tags.Any(tag => tag.Name == filter.Name)))
                    {
                        matching.Add(album);
                    }
                }
                else if (FilterType == FilterType.AtLeastOneMatching)
                {
                    if (LibraryManager.Filters.Any(filter => album.Tags.Any(tag => tag.Name == filter.Name)))
                    {
                        matching.Add(album);
                    }
                }
                else if (FilterType == FilterType.WithNoTags)
                {
                    if (album.Tags.Count == 0)
                    {
                        matching.Add(album);
                    }
                }
            }

            LibraryManager.VisibleAlbums = new ObservableCollection<Album>(matching);
        }

        public void SortAlbums(object param)
        {
            if (SortBy == "Artist")
            {
                if (IsSortingAscending)
                {
                    LibraryManager.VisibleAlbums = new ObservableCollection<Album>(LibraryManager.VisibleAlbums.OrderBy(a => a.Artists[0].Name));
                }
                else
                {
                    LibraryManager.VisibleAlbums = new ObservableCollection<Album>(LibraryManager.VisibleAlbums.OrderByDescending(a => a.Artists[0].Name));
                }
            }
            else
            {
                if (IsSortingAscending)
                {
                    LibraryManager.VisibleAlbums = new ObservableCollection<Album>(LibraryManager.VisibleAlbums.OrderBy(a => a.GetType().GetProperty(SortBy).GetValue(a, null)));
                }
                else
                {
                    LibraryManager.VisibleAlbums = new ObservableCollection<Album>(LibraryManager.VisibleAlbums.OrderByDescending(a => a.GetType().GetProperty(SortBy).GetValue(a, null)));
                }
            }
        }

        public void ToggleSortingDirection(object param)
        {
            IsSortingAscending = !IsSortingAscending;
            SortAlbums(SortBy);
        }

        public void ChangeFilterType(object param)
        {
            FilterType = FilterType == FilterType.WithNoTags ? FilterType.AllMatching : FilterType + 1;
            FilterAlbums(null);
        }

        public async void SyncAlbums(object param)
        {
            LibraryManager.SelectedAlbum = new Album();
            var syncedAlbums = await AlbumsProvider.SyncAlbums();
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
            ToggleSortingDirectionCommand = new Command(ToggleSortingDirection);
            SyncCommand = new Command(SyncAlbums);
            LoginCommand = new Command(LoginToSpotify);
            ChangeFilterTypeCommand = new Command(ChangeFilterType);
        }
    }
}

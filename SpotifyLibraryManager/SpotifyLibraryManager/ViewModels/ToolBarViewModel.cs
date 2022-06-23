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
        private ObservableCollection<Album> _searchedAlbums;
        private ObservableCollection<Album> _filteredAlbums;

        public LibraryManager LibraryManager { get; private set; }
        public bool IsSortingAscending { get; set; }
        public string SearchPhrase { get; set; }
        public FilterType FilterType { get; set; } = FilterType.AllMatching;
        public string SortBy
        {
            get { return _sortBy; }
            set { _sortBy = value; SortAlbums(null); }
        }
        public Command SearchCommand { get; private set; }
        public Command ToggleSortingDirectionCommand { get; private set; }
        public Command SyncCommand { get; private set; }
        public Command LoginCommand { get; private set; }
        public Command ChangeFilterTypeCommand { get; private set; }

        public void SearchAlbums(object param)
        {
            string searchPhrase = (string)param;

            if (string.IsNullOrEmpty(searchPhrase))
            {
                _searchedAlbums = LibraryManager.AllAlbums;
            }
            else
            {
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

                _searchedAlbums = new ObservableCollection<Album>(matching);
            }

            LibraryManager.VisibleAlbums = new ObservableCollection<Album>(_filteredAlbums.Join(
                _searchedAlbums, searched => searched.AlbumId, filtered => filtered.AlbumId, (searched, filtered) => searched = filtered)
                );
        }

        public void FilterAlbums(object param)
        {
            if (LibraryManager.Filters.Count == 0 && FilterType != FilterType.WithNoTags)
            {
                _filteredAlbums = LibraryManager.AllAlbums;
            }
            else
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

                _filteredAlbums = new ObservableCollection<Album>(matching);
            }

            LibraryManager.VisibleAlbums = new ObservableCollection<Album>(_filteredAlbums.Join(
                _searchedAlbums, searched => searched.AlbumId, filtered => filtered.AlbumId, (searched, filtered) => searched = filtered)
                );
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
            LibraryManager.AllAlbums = new ObservableCollection<Album>(await AlbumsProvider.SyncAlbums());
            LibraryManager.VisibleAlbums = LibraryManager.AllAlbums;
            _filteredAlbums = LibraryManager.AllAlbums;
            _searchedAlbums = LibraryManager.AllAlbums;
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
            LibraryManager.AllAlbums.CollectionChanged += (s, e) => FilterAlbums(null);
            _filteredAlbums = LibraryManager.AllAlbums;
            _searchedAlbums = LibraryManager.AllAlbums;

            SearchCommand = new Command(SearchAlbums);
            ToggleSortingDirectionCommand = new Command(ToggleSortingDirection);
            SyncCommand = new Command(SyncAlbums);
            LoginCommand = new Command(LoginToSpotify);
            ChangeFilterTypeCommand = new Command(ChangeFilterType);
        }
    }
}

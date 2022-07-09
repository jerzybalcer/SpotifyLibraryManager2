using SpotifyLibraryManager.Helpers;
using SpotifyLibraryManager.Models;
using SpotifyLibraryManager.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SpotifyLibraryManager.ViewModels
{
    public class ToolBarViewModel : ViewModelBase
    {
        private SortBy _sortBy = Models.SortBy.LikeDate;
        private FilterRequirement _filterRequirement = FilterRequirement.All;
        private bool _showTagless;

        public LibraryManager LibraryManager { get; private set; }
        public bool IsSortingAscending { get; set; }
        public string SearchPhrase { get; set; }

        public FilterRequirement FilterRequirement
        {
            get { return _filterRequirement; }
            set { _filterRequirement = value; ShowTagless = false; ModifyVisibleAlbums(null); }
        }

        public bool ShowTagless
        {
            get { return _showTagless; }
            set { _showTagless = value; ModifyVisibleAlbums(null); }
        }

        public SortBy SortBy
        {
            get { return _sortBy; }
            set { _sortBy = value; ModifyVisibleAlbums(null); }
        }

        public Command SearchCommand { get; private set; }
        public Command ToggleSortingDirectionCommand { get; private set; }
        public Command SyncCommand { get; private set; }
        public Command LoginCommand { get; private set; }

        public void ModifyVisibleAlbums(object searchPhraseParam)
        {
            // Applied whenever user wants to modify his visible albums (search/sort/filter).

            // Update search phrase only whenever user searches.
            bool isSearchPhraseModified = searchPhraseParam != null; // If it's modified it contains string (empty string if search phrase removed).
            if (isSearchPhraseModified)
            {
                SearchPhrase = (string)searchPhraseParam;
            }

            // Most optimal way: search -> filter -> sort
            // That's because search reduces filter time, and sort sorts out results after filtering.
            ObservableCollection<Album> albumsAfterSearch = SearchAlbums(SearchPhrase);
            ObservableCollection<Album> albumsAfterSearchAndFilter = FilterAlbums(albumsAfterSearch);
            SortAndDisplayAlbums(albumsAfterSearchAndFilter);
        }

        private ObservableCollection<Album> SearchAlbums(string searchPhrase)
        {
            if (string.IsNullOrEmpty(searchPhrase))
            {
                return LibraryManager.AllAlbums;
            }
            else
            {
                List<Album> matching = new List<Album>();

                foreach (var album in LibraryManager.AllAlbums) // All albums, because search is invoked first in filtering sequence.
                {
                    foreach (var artist in album.Artists)
                    {
                        if (artist.Name.ToLower().Contains(searchPhrase.ToLower()))
                        {
                            matching.Add(album);
                        }
                    }

                    if (album.Title.ToLower().Contains(searchPhrase.ToLower()) && !matching.Contains(album))
                    {
                        matching.Add(album);
                    }
                }

                return new ObservableCollection<Album>(matching);
            }
        }

        private ObservableCollection<Album> FilterAlbums(ObservableCollection<Album> albumsToFilter)
        {
            if (LibraryManager.Filters.Count == 0 && !ShowTagless)
            {
                // Then no need to filter search results.
                return albumsToFilter;
            }
            else
            {
                var matching = new List<Album>();

                foreach (var album in albumsToFilter)
                {
                    if (ShowTagless)
                    {
                        if (album.Tags.Count == 0)
                        {
                            matching.Add(album);
                        }
                    }
                    else
                    {
                        if (FilterRequirement == FilterRequirement.All)
                        {
                            if (LibraryManager.Filters.All(filter => album.Tags.Any(tag => tag.Name == filter.Name)))
                            {
                                matching.Add(album);
                            }
                        }
                        else if (FilterRequirement == FilterRequirement.Any)
                        {
                            if (LibraryManager.Filters.Any(filter => album.Tags.Any(tag => tag.Name == filter.Name)))
                            {
                                matching.Add(album);
                            }
                        }
                    }
                }
                return new ObservableCollection<Album>(matching);
            }
        }

        private void SortAndDisplayAlbums(ObservableCollection<Album> albumsToSort)
        {
            var sortedAlbums = new ObservableCollection<Album>();

            if (SortBy == SortBy.Artist)
            {
                if (IsSortingAscending)
                {
                    sortedAlbums = new ObservableCollection<Album>(albumsToSort.OrderBy(a => a.Artists[0].Name));
                }
                else
                {
                    sortedAlbums = new ObservableCollection<Album>(albumsToSort.OrderByDescending(a => a.Artists[0].Name));
                }
            }
            else
            {
                if (IsSortingAscending)
                {
                    sortedAlbums = new ObservableCollection<Album>(albumsToSort.OrderBy(a => a.GetType().GetProperty(SortBy.ToString()).GetValue(a, null)));
                }
                else
                {
                    sortedAlbums = new ObservableCollection<Album>(albumsToSort.OrderByDescending(a => a.GetType().GetProperty(SortBy.ToString()).GetValue(a, null)));
                }
            }

            // Display sorted albums (sort is invoked last in filtering sequence).
            LibraryManager.VisibleAlbums = sortedAlbums;
        }

        public void ToggleSortingDirection(object param)
        {
            IsSortingAscending = !IsSortingAscending;
            ModifyVisibleAlbums(null);
        }

        public async void SyncAlbums(object param)
        {
            if (Spotify.Client is not null)
            {
                LibraryManager.SelectedAlbum = new Album();
                LibraryManager.AllAlbums = new ObservableCollection<Album>(await AlbumsProvider.SyncAlbums());
                ModifyVisibleAlbums(null); // Refresh filters after ansychrouous album sync.
            }
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
            LibraryManager.Filters.CollectionChanged += (s, e) => ModifyVisibleAlbums(null);
            LibraryManager.AllAlbums.CollectionChanged += (s, e) => ModifyVisibleAlbums(null);

            SearchCommand = new Command(ModifyVisibleAlbums);
            ToggleSortingDirectionCommand = new Command(ToggleSortingDirection);
            SyncCommand = new Command(SyncAlbums);
            LoginCommand = new Command(LoginToSpotify);
        }
    }
}
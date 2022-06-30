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
		private string _sortBy = "LikeDate";

		public LibraryManager LibraryManager { get; private set; }
		public bool IsSortingAscending { get; set; }
		public string SearchPhrase { get; set; }
		public FilterType FilterType { get; set; } = FilterType.AllMatching;

		public string SortBy
		{
			get { return _sortBy; }
			set { _sortBy = value; ModifyVisibleAlbums(null); }
		}

		public Command SearchCommand { get; private set; }
		public Command ToggleSortingDirectionCommand { get; private set; }
		public Command SyncCommand { get; private set; }
		public Command LoginCommand { get; private set; }
		public Command ChangeFilterTypeCommand { get; private set; }

		public void ModifyVisibleAlbums(object searchPhraseParam)
		{
			// Applied whenever user wants to modify his visible albums (search/sort/filter).

			// Update search phrase only whenever user searches.
			bool isSearchPhraseModified = searchPhraseParam != null; // If it's modified it contains string (empty string if search phrase removed).
			if (isSearchPhraseModified)
			{
				SearchPhrase = (string) searchPhraseParam;
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
			if (LibraryManager.Filters.Count == 0 && FilterType != FilterType.WithNoTags)
			{
				// Then no need to filter search results.
				return albumsToFilter;
			}
			else
			{
				var matching = new List<Album>();

				foreach (var album in albumsToFilter)
				{
					switch (FilterType)
					{
						case FilterType.AllMatching:
							if (LibraryManager.Filters.All(filter => album.Tags.Any(tag => tag.Name == filter.Name)))
							{
								matching.Add(album);
							}
							break;

						case FilterType.AtLeastOneMatching:
							if (LibraryManager.Filters.Any(filter => album.Tags.Any(tag => tag.Name == filter.Name)))
							{
								matching.Add(album);
							}
							break;

						case FilterType.WithNoTags:
							if (album.Tags.Count == 0)
							{
								matching.Add(album);
							}
							break;
					}
				}

				return new ObservableCollection<Album>(matching);
			}
		}

		private void SortAndDisplayAlbums(ObservableCollection<Album> albumsToSort)
		{
			var sortedAlbums = new ObservableCollection<Album>();

			if (SortBy == "Artist")
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
					sortedAlbums = new ObservableCollection<Album>(albumsToSort.OrderBy(a => a.GetType().GetProperty(SortBy).GetValue(a, null)));
				}
				else
				{
					sortedAlbums = new ObservableCollection<Album>(albumsToSort.OrderByDescending(a => a.GetType().GetProperty(SortBy).GetValue(a, null)));
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

		public void ChangeFilterType(object param)
		{
			FilterType = FilterType == FilterType.WithNoTags ? FilterType.AllMatching : FilterType + 1;
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
			ChangeFilterTypeCommand = new Command(ChangeFilterType);
		}
	}
}
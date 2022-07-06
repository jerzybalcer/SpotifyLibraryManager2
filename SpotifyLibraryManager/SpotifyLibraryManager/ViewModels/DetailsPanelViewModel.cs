using Microsoft.EntityFrameworkCore;
using SpotifyLibraryManager.Database;
using SpotifyLibraryManager.Helpers;
using SpotifyLibraryManager.Models;
using System.Linq;

namespace SpotifyLibraryManager.ViewModels
{
	public class DetailsPanelViewModel : ViewModelBase
	{
		public LibraryManager LibraryManager { get; private set; }
		public bool IsSuggestionPopupOpen { get; set; }
		public string NewTagText { get; set; }
		public Command AddTagCommand { get; set; }
		public Command RemoveTagCommand { get; set; }

		private ToolBarViewModel toolBarContext;

		public string ArtistsString
		{
			get
			{
				if (LibraryManager.SelectedAlbum is null || LibraryManager.SelectedAlbum.Artists is null)
				{
					return string.Empty;
				}

				string returnString = LibraryManager.SelectedAlbum.Artists[0].Name;

				for (var i = 1; i < LibraryManager.SelectedAlbum.Artists.Count; i++)
				{
					returnString += ", " + LibraryManager.SelectedAlbum.Artists[i].Name;
				}

				return returnString;
			}
		}

		public DetailsPanelViewModel(LibraryManager libraryManager, ToolBarViewModel toolbar)
		{
			LibraryManager = libraryManager;
			toolBarContext = toolbar;
			AddTagCommand = new Command(AddTag);
			RemoveTagCommand = new Command(RemoveTag);

			LibraryManager.AlbumSelected += (s, e) => OnPropertyChanged(nameof(ArtistsString));
		}

		private async void AddTag(object param)
		{
			string tagName = (string) param;
			IsSuggestionPopupOpen = false;

			using (var db = new LibraryContext())
			{
				var thisAlbum = db.Albums
					.Include(album => album.Tags)
					.Include(album => album.Artists)
					.First(album => album.AlbumId == LibraryManager.SelectedAlbum.AlbumId);

				bool tagExists = LibraryManager.AllTags.ToList().Exists(tag => tag.Name.ToLower() == tagName.ToLower());

				if (!tagExists)
				{
					thisAlbum.Tags.Add(new Tag { Name = tagName, ColorHex = RandomHexGenerator.GenerateRandomHex() });
					LibraryManager.AllTags.Add(thisAlbum.Tags.Last());
				}
				else
				{
					bool albumNotTaggedWithIt = thisAlbum.Tags.FirstOrDefault(tag => tag.Name.ToLower() == tagName.ToLower()) is null;

					if (albumNotTaggedWithIt)
					{
						thisAlbum.Tags.Add(LibraryManager.AllTags.First(tag => tag.Name.ToLower() == tagName.ToLower()));
					}
				}
				await db.SaveChangesAsync();
				OnTagUpdate(thisAlbum);
			}
		}

		private async void RemoveTag(object param)
		{
			string tagName = (string) param;

			using (var db = new LibraryContext())
			{
				var thisAlbum = db.Albums
					.Include(album => album.Tags)
					.Include(album => album.Artists)
					.First(album => album.AlbumId == LibraryManager.SelectedAlbum.AlbumId);

				var tagToRemove = thisAlbum.Tags.First(tag => tag.Name.ToLower() == tagName.ToLower());

				thisAlbum.Tags.Remove(tagToRemove);

				bool isTagAssignedToSingleAlbum = db.Tags.AsNoTracking().Include(t => t.Albums).Single(tag => tag.TagId == tagToRemove.TagId).Albums.Count == 1;

				if (isTagAssignedToSingleAlbum)
				{
					db.Tags.Remove(tagToRemove);
					LibraryManager.AllTags.Remove(LibraryManager.AllTags.Single(t => t.TagId == tagToRemove.TagId));
					LibraryManager.Filters.Remove(LibraryManager.Filters.FirstOrDefault(f => f.TagId == tagToRemove.TagId));
				}

				await db.SaveChangesAsync();
				OnTagUpdate(thisAlbum);
			}
		}

		public Tag? GetSuggestionTag()
		{
			var equalTag = LibraryManager.AllTags.FirstOrDefault(tag => tag.Name.ToLower() == NewTagText.ToLower());

			if (equalTag is not null)
			{
				return equalTag;
			}
			else
			{
				return LibraryManager.AllTags.Where(tag => tag.Name.ToLower().StartsWith(NewTagText.ToLower())).FirstOrDefault();
			}
		}

		private void OnTagUpdate(Album targetAlbum)
		{
			LibraryManager.SelectedAlbum = targetAlbum;

			var indexInVisible = LibraryManager.VisibleAlbums.ToList().FindIndex(a => a.AlbumId == targetAlbum.AlbumId);

			if (indexInVisible >= 0)
			{
				LibraryManager.VisibleAlbums[indexInVisible] = targetAlbum;
			}

			LibraryManager.AllAlbums[LibraryManager.AllAlbums.ToList().FindIndex(a => a.AlbumId == targetAlbum.AlbumId)] = targetAlbum;

			toolBarContext.ModifyVisibleAlbums(null);
		}
	}
}
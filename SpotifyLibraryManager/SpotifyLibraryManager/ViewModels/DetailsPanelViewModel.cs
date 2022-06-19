using Microsoft.EntityFrameworkCore;
using SpotifyLibraryManager.Database;
using SpotifyLibraryManager.Helpers;
using SpotifyLibraryManager.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        public Command OpenWithSpotifyCommand { get; private set; }

        public string ArtistsString
        {
            get 
            {
                if(LibraryManager.SelectedAlbum is null)
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

        public DetailsPanelViewModel(LibraryManager libraryManager)
        {
            LibraryManager = libraryManager;
            AddTagCommand = new Command(AddTag);
            RemoveTagCommand = new Command(RemoveTag);
            OpenWithSpotifyCommand = new Command(OpenWithSpotify);

            LibraryManager.AlbumSelected += (s, e) => OnPropertyChanged(nameof(ArtistsString));
        }

        private async void AddTag(object param)
        {
            IsSuggestionPopupOpen = false;
            string tagName = (string)param;

            using (var db = new LibraryContext())
            {
                var thisAlbum = db.Albums
                    .Include(album => album.Tags)
                    .Include(album => album.Artists)
                    .First(album => album.AlbumId == LibraryManager.SelectedAlbum.AlbumId);

                if (LibraryManager.AllTags.ToList().Exists(tag => tag.Name.ToLower() == tagName.ToLower()) == false)
                {
                    thisAlbum.Tags.Add(new Tag { Name = tagName, ColorHex = RandomHexGenerator.GenerateRandomHex() });
                }
                else
                {
                    if (thisAlbum.Tags.FirstOrDefault(tag => tag.Name.ToLower() == tagName.ToLower()) is null)
                    {
                        thisAlbum.Tags.Add(LibraryManager.AllTags.First(tag => tag.Name.ToLower() == tagName.ToLower()));
                    }
                }
                await db.SaveChangesAsync();
                OnTagUpdate(thisAlbum);
                LibraryManager.AllTags = new ObservableCollection<Tag>(await db.Tags.AsNoTracking().ToListAsync());
            }
        }

        private async void RemoveTag(object param)
        {
            string tagName = (string)param;

            using (var db = new LibraryContext())
            {
                var thisAlbum = db.Albums
                    .Include(album => album.Tags)
                    .Include(album => album.Artists)
                    .First(album => album.AlbumId == LibraryManager.SelectedAlbum.AlbumId);

                var tagToRemove = thisAlbum.Tags.FindIndex(tag => tag.Name.ToLower() == tagName.ToLower());

                thisAlbum.Tags.RemoveAt(tagToRemove);

                await db.SaveChangesAsync();
                OnTagUpdate(thisAlbum);
            }
        }

        public Tag? GetSuggestionTag()
        {
            return LibraryManager.AllTags.Where(tag => tag.Name.ToLower().StartsWith(NewTagText.ToLower())).FirstOrDefault();
        }

        private void OnTagUpdate(Album targetAlbum)
        {
            LibraryManager.SelectedAlbum = targetAlbum;
            LibraryManager.VisibleAlbums[LibraryManager.VisibleAlbums.ToList().FindIndex(a => a.AlbumId == targetAlbum.AlbumId)] = targetAlbum;
            LibraryManager.AllAlbums[LibraryManager.AllAlbums.ToList().FindIndex(a => a.AlbumId == targetAlbum.AlbumId)] = targetAlbum;
        }

        private void OpenWithSpotify(object obj)
        {
            try
            {
                Process.Start("spotify", "--uri=" + LibraryManager.SelectedAlbum.SpotifyUri);
            }
            catch
            {
                var browser = new Process();
                browser.StartInfo.UseShellExecute = true;
                browser.StartInfo.FileName = "https://www.spotify.com/us/download/other/";
                browser.Start();
            }
        }
    }
}

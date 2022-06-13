using Microsoft.EntityFrameworkCore;
using SpotifyLibraryManager.Database;
using SpotifyLibraryManager.Helpers;
using SpotifyLibraryManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyLibraryManager.ViewModels
{
    public class DetailsPanelViewModel : ViewModelBase
    {
        public LibraryManager LibraryManager { get; set; }
        public Album Album { get; set; }
        public List<Tag> AllTags { get; set; }
        public bool IsSuggestionPopupOpen { get; set; }
        public Command AddTagCommand { get; set; }

        public string ArtistsString
        {
            get 
            {
                if(Album is null)
                {
                    return string.Empty;
                }

                string returnString = Album.Artists[0].Name;

                for (var i = 1; i < Album.Artists.Count; i++)
                {
                    returnString += ", " + Album.Artists[i].Name;
                }

                return returnString;
            }
        }

        public DetailsPanelViewModel()
        {
            AddTagCommand = new Command(AddTag);
            LoadAllTags();
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
                    .First(album => album.AlbumId == Album.AlbumId);

                if (AllTags.Exists(tag => tag.Name.ToLower() == tagName.ToLower()) == false)
                {
                    thisAlbum.Tags.Add(new Tag { Name = tagName, ColorHex = RandomHexGenerator.GenerateRandomHex() });
                }
                else
                {
                    if (thisAlbum.Tags.FirstOrDefault(tag => tag.Name.ToLower() == tagName.ToLower()) is null)
                    {
                        thisAlbum.Tags.Add(AllTags.First(tag => tag.Name.ToLower() == tagName.ToLower()));
                    }
                }
                await db.SaveChangesAsync();
                Album = thisAlbum;
                AllTags = await db.Tags.ToListAsync();
            }
        }

        private async void LoadAllTags()
        {
            using (var db = new LibraryContext())
            {
                AllTags = await db.Tags.ToListAsync();
            }
        }
    }
}

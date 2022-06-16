using SpotifyLibraryManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using SpotifyLibraryManager.Database;
using Microsoft.EntityFrameworkCore;

namespace SpotifyLibraryManager.ViewModels
{
    public class LibraryManager : ViewModelBase
    {
        public List<Album> Albums { get; set; }
        public List<Tag> AllTags { get; set; }
        public DetailsPanelViewModel DetailsPanel { get; set; }
        public ToolBarViewModel ToolBar { get; set; }
        public AlbumsListViewModel AlbumsList { get; set; }

        public LibraryManager(AlbumsListViewModel albumsList, DetailsPanelViewModel detailsPanel, ToolBarViewModel toolBar)
        {
            AlbumsList = albumsList;
            DetailsPanel = detailsPanel;
            ToolBar = toolBar;
            ReloadAlbums();
            LoadAllTags();
            ToolBar.AllTags = AllTags;
        }
        
        public async void SelectAlbum(Album album)
        {
            DetailsPanel.Album = album;
            DetailsPanel.AllTags = AllTags;
        }

        public async void ReloadAlbums()
        {
            Albums = await AlbumsManager.GetAlbumsFromDb();
            AlbumsList.VisibleAlbums = new ObservableCollection<Album>(Albums);
        }

        public async void LoadAllTags()
        {
            using (var db = new LibraryContext())
            {
                AllTags = await db.Tags.ToListAsync();
            }
        }

        public void UpdateAlbum(Album album)
        {
            AlbumsList.VisibleAlbums[AlbumsList.VisibleAlbums.ToList().FindIndex(a => a.AlbumId == album.AlbumId)] = album;
            Albums[Albums.FindIndex(a => a.AlbumId == album.AlbumId)] = album;
        }

        public void FilterAlbums()
        {
            var matching = new List<Album>();

            foreach (var album in Albums)
            {
                if (ToolBar.Filters.All(filter => album.Tags.Any(tag => tag.Name == filter.Name)))
                {
                    matching.Add(album);
                }
            }

            AlbumsList.VisibleAlbums = new ObservableCollection<Album>(matching);
        }

        public void SearchAlbums(string searchPhrase)
        {
            if(string.IsNullOrEmpty(searchPhrase))
            {
                AlbumsList.VisibleAlbums = new ObservableCollection<Album>(Albums);
                return;
            }

            List<Album> matching = new List<Album>();
            
            foreach(var album in Albums)
            {
                foreach(var artist in album.Artists)
                {
                    if (artist.Name.ToLower().Contains(searchPhrase.ToLower()))
                    {
                        matching.Add(album);
                        continue;
                    }
                }

                if (album.Title.ToLower().Contains(searchPhrase.ToLower()))
                {
                    matching.Add(album);
                }
            }

            AlbumsList.VisibleAlbums = new ObservableCollection<Album>(matching);
        }
    }
}

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
        public DetailsPanelViewModel DetailsPanel { get; set; }
        public ToolBarViewModel ToolBar { get; set; }
        public AlbumsListViewModel AlbumsList { get; set; }

        public LibraryManager(AlbumsListViewModel albumsList, DetailsPanelViewModel detailsPanel, ToolBarViewModel toolBar)
        {
            AlbumsList = albumsList;
            DetailsPanel = detailsPanel;
            ToolBar = toolBar;
            ReloadAlbums();
        }
        
        public async void SelectAlbum(Album album)
        {
            DetailsPanel.Album = album;

            using (var db = new LibraryContext())
            {
                ToolBar.AllTags = await db.Tags.ToListAsync();
            }
        }

        public async void ReloadAlbums()
        {
            Albums = await AlbumsManager.GetAlbumsFromDb();
            AlbumsList.VisibleAlbums = new ObservableCollection<Album>(Albums);
        }

        public void UpdateAlbum(Album album)
        {
            AlbumsList.VisibleAlbums[Albums.FindIndex(a => a.AlbumId == album.AlbumId)] = album;
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
            var matching = Albums.Where(a => a.Title.ToLower().Contains(searchPhrase.ToLower())).ToList();
            
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

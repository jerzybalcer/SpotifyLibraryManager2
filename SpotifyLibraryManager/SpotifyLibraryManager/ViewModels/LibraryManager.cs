using SpotifyLibraryManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyLibraryManager.ViewModels
{
    public class LibraryManager : ViewModelBase
    {
        public List<Album> Albums { get; set; }

        public DetailsPanelViewModel DetailsPanel { get; set; }
        public ToolBarViewModel ToolBar { get; set; }
        public AlbumsListViewModel AlbumsList { get; set; }

        public LibraryManager(AlbumsListViewModel albumsList, DetailsPanelViewModel detailsPanel)
        {
            AlbumsList = albumsList;
            DetailsPanel = detailsPanel;
            ReloadAlbums();
        }
        
        public void SelectAlbum(Album album)
        {
            DetailsPanel.Album = album;
        }

        public async void ReloadAlbums()
        {
            Albums = await AlbumsManager.GetAlbumsFromDb();
            AlbumsList.VisibleAlbums = new ObservableCollection<Album>(Albums);
        }
    }
}

using SpotifyLibraryManager.Helpers;
using SpotifyLibraryManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyLibraryManager.ViewModels
{
    public class AlbumsListViewModel : ViewModelBase
    {
        public ObservableCollection<Album> VisibleAlbums { get; set; } = new ObservableCollection<Album>();
        public LibraryManager LibraryManager { get; set; }

        public AlbumsListViewModel()
        {
            SelectAlbumCommand = new Command(SelectAlbum, null);
        }

        public Command SelectAlbumCommand { get; private set; }
        private void SelectAlbum(object album)
        {
            LibraryManager.SelectAlbum((Album)album);
        }
    }
}

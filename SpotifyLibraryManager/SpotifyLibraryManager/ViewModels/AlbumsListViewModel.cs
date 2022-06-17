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
        public LibraryManager LibraryManager { get; private set; }

        public AlbumsListViewModel(LibraryManager libraryManager)
        {
            LibraryManager = libraryManager;
            SelectAlbumCommand = new Command(SelectAlbum);
        }

        public Command SelectAlbumCommand { get; private set; }
        private void SelectAlbum(object album)
        {
            LibraryManager.SelectedAlbum = (Album)album;
        }
    }
}

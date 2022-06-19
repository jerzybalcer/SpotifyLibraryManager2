using SpotifyLibraryManager.Helpers;
using SpotifyLibraryManager.Models;

namespace SpotifyLibraryManager.ViewModels
{
    public class AlbumsListViewModel : ViewModelBase
    {
        public LibraryManager LibraryManager { get; private set; }
        public int Columns { get; set; } = 4;

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

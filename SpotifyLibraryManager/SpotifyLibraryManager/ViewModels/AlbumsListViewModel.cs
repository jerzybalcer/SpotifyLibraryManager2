using SpotifyLibraryManager.Helpers;
using SpotifyLibraryManager.Models;
using System.Diagnostics;
using System.Windows;

namespace SpotifyLibraryManager.ViewModels
{
    public class AlbumsListViewModel : ViewModelBase
    {
        public LibraryManager LibraryManager { get; private set; }
        public int Columns { get; set; } = 4;
        public Command SelectAlbumCommand { get; private set; }
        public Command OpenWithSpotifyCommand { get; private set; }
        public Command CopyLinkCommand { get; private set; }

        public AlbumsListViewModel(LibraryManager libraryManager)
        {
            LibraryManager = libraryManager;
            SelectAlbumCommand = new Command(SelectAlbum);
            OpenWithSpotifyCommand = new Command(OpenWithSpotify);
            CopyLinkCommand = new Command(CopyLink);
        }

        private void SelectAlbum(object album)
        {
            LibraryManager.SelectedAlbum = (Album)album;
        }
        private void OpenWithSpotify(object obj)
        {
            Album album = (Album)obj;

            try
            {
                Process.Start("spotify", "--uri=" + album.SpotifyUri);
            }
            catch
            {
                var browser = new Process();
                browser.StartInfo.UseShellExecute = true;
                browser.StartInfo.FileName = "https://www.spotify.com/us/download/other/";
                browser.Start();
            }
        }

        private void CopyLink(object obj)
        {
            Album album = (Album)obj;

            Clipboard.SetText(album.ExternalUrl);
        }
    }
}

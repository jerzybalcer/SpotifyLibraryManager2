using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyLibraryManager
{
    public static class AlbumsManager
    {
        public static List<SavedAlbum> Albums = new List<SavedAlbum>();

        public static async Task LoadAll()
        {
            if(Spotify.Client is null)
            {
                return;
            }
            else
            {
                var currentPage = await Spotify.Client.Library.GetAlbums(new LibraryAlbumsRequest { Limit = 50, Offset = 0 });
              
                while (currentPage.Next != null)
                {
                    currentPage = await Spotify.Client.Library.GetAlbums(new LibraryAlbumsRequest { Limit = 50, Offset = Albums.Count });

                    foreach (var item in currentPage.Items)
                    {
                        Albums.Add(item);
                    }
                }
            }
        }
    }
}

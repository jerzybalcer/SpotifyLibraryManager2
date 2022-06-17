using Microsoft.EntityFrameworkCore;
using SpotifyAPI.Web;
using SpotifyLibraryManager.Database;
using SpotifyLibraryManager.Helpers;
using SpotifyLibraryManager.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyLibraryManager
{
    public static class AlbumsProvider
    {
        private static async Task<List<SavedAlbum>> LoadAllFromSpotify()
        {
            var albums = new List<SavedAlbum>();

            if (Spotify.Client is null)
            {
                return albums;
            }
            else
            {
                var currentPage = await Spotify.Client.Library.GetAlbums(new LibraryAlbumsRequest { Limit = 50, Offset = 0 });

                albums.Concat(currentPage.Items);
              
                while (currentPage.Next != null)
                {
                    currentPage = await Spotify.Client.Library.GetAlbums(new LibraryAlbumsRequest { Limit = 50, Offset = albums.Count });

                    foreach (var item in currentPage.Items)
                    {
                        albums.Add(item);
                    }
                }
            }

            return albums;
        }

        public static Album ConvertToDbAlbum(SavedAlbum spotifyAlbum, ref List<Artist> allArtists)
        {
            var artists = new List<Artist>();

            foreach(var artist in spotifyAlbum.Album.Artists)
            {
                var existingArtist = allArtists.FirstOrDefault(ar => ar.Name == artist.Name);

                if (existingArtist == null)
                {
                    Artist newArtist = new Artist { Name = artist.Name };
                    artists.Add(newArtist);
                    allArtists.Add(newArtist);
                }
                else
                {
                    artists.Add(existingArtist);
                }
            }

            return new Album
            {
                Title = spotifyAlbum.Album.Name,
                Artists = artists,
                CoverUrl = spotifyAlbum.Album.Images[0].Url,
                ExternalUrl = spotifyAlbum.Album.ExternalUrls["spotify"],
                SpotifyUri = spotifyAlbum.Album.Uri,
                TotalTracks = spotifyAlbum.Album.TotalTracks,
                ReleaseDate = DateTimeCreator.CreateFromString(spotifyAlbum.Album.ReleaseDate),
                LikeDate = spotifyAlbum.AddedAt
            };
        }

        public static async Task<List<Album>> SyncAlbums()
        {
            var albumsFromSpotify = new List<SavedAlbum>();

            if (Spotify.Client is not null)
            {
                albumsFromSpotify = await LoadAllFromSpotify();
            }

            using (var db = new LibraryContext())
            {
                var albumsFromDb = await db.Albums
                    .Include(album => album.Artists)
                    .Include(album => album.Tags)
                    .ToListAsync();

                var allAritsts = await db.Artists.ToListAsync();

                foreach (var albumFromSpotify in albumsFromSpotify)
                {
                    if (albumsFromDb.FirstOrDefault(a => a.Title == albumFromSpotify.Album.Name
                         && a.Artists.FirstOrDefault()!.Name == albumFromSpotify.Album.Artists[0].Name) is null)
                    {
                        db.Albums.Add(ConvertToDbAlbum(albumFromSpotify, ref allAritsts));
                    }
                }

                foreach (var albumFromDb in albumsFromDb)
                {
                    if (albumsFromSpotify.FirstOrDefault(a => a.Album.Name == albumFromDb.Title
                         && a.Album.Artists[0].Name == albumFromDb.Artists.FirstOrDefault()!.Name) is null)
                    {
                        db.Albums.Remove(albumFromDb);
                    }
                }

                await db.SaveChangesAsync();
                return await db.Albums.OrderByDescending(album => album.LikeDate).ToListAsync();
            }
        }

        public static async Task<List<Album>> GetAlbumsFromDb()
        {
            using (var db = new LibraryContext())
            {
                var albumsFromDb = await db.Albums
                    .Include(album => album.Artists)
                    .Include(album => album.Tags)
                    .OrderByDescending(album => album.LikeDate)
                    .ToListAsync();

                return albumsFromDb;
            }
        }
    }
}

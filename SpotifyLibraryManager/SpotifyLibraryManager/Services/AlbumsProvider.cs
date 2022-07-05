using Microsoft.EntityFrameworkCore;
using SpotifyAPI.Web;
using SpotifyLibraryManager.Database;
using SpotifyLibraryManager.Helpers;
using SpotifyLibraryManager.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyLibraryManager.Services
{
	public static class AlbumsProvider
	{
		private static async Task<List<SavedAlbum>> LoadAllFromSpotify()
		{
			var allAlbums = new List<SavedAlbum>();

			if (Spotify.Client is null)
			{
				return allAlbums;
			}
			else
			{
				var currentAlbumsFragment = await Spotify.Client.Library.GetAlbums(new LibraryAlbumsRequest { Limit = 50, Offset = 0 });

				allAlbums.Concat(currentAlbumsFragment.Items);

				while (currentAlbumsFragment.Next != null)
				{
					currentAlbumsFragment = await Spotify.Client.Library.GetAlbums(new LibraryAlbumsRequest { Limit = 50, Offset = allAlbums.Count });

					foreach (var item in currentAlbumsFragment.Items)
					{
						allAlbums.Add(item);
					}
				}
			}

			return allAlbums;
		}

		public static Album ConvertToDbAlbum(SavedAlbum spotifyAlbum, ref List<Artist> allArtists)
		{
			var artists = new List<Artist>();

			foreach (var artist in spotifyAlbum.Album.Artists)
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
			var albumsSpotify = new List<SavedAlbum>();

			if (Spotify.Client is not null)
			{
				albumsSpotify = await LoadAllFromSpotify();
			}

			using (var db = new LibraryContext())
			{
				var albumsDatabase = await db.Albums
					.Include(album => album.Artists)
					.Include(album => album.Tags)
					.ToListAsync();

				var artistsDatabase = await db.Artists.ToListAsync();

				// Add albums that are missing in the database.
				foreach (var albumSpotify in albumsSpotify)
				{
					if (albumsDatabase.FirstOrDefault(a => a.Title == albumSpotify.Album.Name
						 && a.Artists.FirstOrDefault()!.Name == albumSpotify.Album.Artists[0].Name) is null)
					{
						db.Albums.Add(ConvertToDbAlbum(albumSpotify, ref artistsDatabase));
					}
				}

				// Remove albums that are not in the Spotify library anymore.
				foreach (var albumDatabase in albumsDatabase)
				{
					if (albumsSpotify.FirstOrDefault(a => a.Album.Name == albumDatabase.Title
						 && a.Album.Artists[0].Name == albumDatabase.Artists.FirstOrDefault()!.Name) is null)
					{
						db.Albums.Remove(albumDatabase);
					}
				}

				await db.SaveChangesAsync();
				return await db.Albums.Include(a => a.Artists).Include(a => a.Tags).ToListAsync();
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
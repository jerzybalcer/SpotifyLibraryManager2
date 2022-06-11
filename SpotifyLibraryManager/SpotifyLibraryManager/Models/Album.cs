using System;
using System.Collections.Generic;

namespace SpotifyLibraryManager.Models
{
    public class Album
    {
        public int AlbumId { get; set; }
        public string Title { get; set; }
        public string CoverUrl { get; set; }
        public string ExternalUrl { get; set; }
        public string SpotifyUri { get; set; }
        public DateTime ReleaseDate { get; set; }
        public DateTime LikeDate { get; set; }
        public int TotalTracks { get; set; }
        public List<Artist> Artists { get; set; }
        public List<Tag> Tags { get; set; }
    }
}

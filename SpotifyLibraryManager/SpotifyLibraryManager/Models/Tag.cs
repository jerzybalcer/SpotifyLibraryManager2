using System.Collections.Generic;

namespace SpotifyLibraryManager.Models
{
    public class Tag
    {
        public int TagId { get; set; }
        public string Name { get; set; }
        public string ColorHex { get; set; }
        public List<Album> Albums { get; set; }

        public Tag()
        {

        }
    }
}

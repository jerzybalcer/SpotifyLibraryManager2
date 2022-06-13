using System;
using System.Linq;

namespace SpotifyLibraryManager.Helpers
{
    public static class RandomHexGenerator
    {
        public static string GenerateRandomHex()
        {
            Random random = new Random();

            byte[] buffer = new byte[3];
            random.NextBytes(buffer);
            string result = String.Concat(buffer.Select(x => x.ToString("X2")).ToArray());

            return "#" + result;
        }
    }
}

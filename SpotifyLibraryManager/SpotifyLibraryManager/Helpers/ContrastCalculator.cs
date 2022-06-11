using System;
using System.Windows.Media;

namespace SpotifyLibraryManager.Helpers
{
    public static class ContrastCalculator
    {
        private const double RedBrightnessConst = 0.241;
        private const double GreenBrightnessConst = 0.691;
        private const double BlueBrightnessConst = 0.068;

        private const int AlphaContrastThreshold = 110;
        private const int BrightnessContrastThreshold = 130;

        public static SolidColorBrush GetContrastingBrush(Color color)
        {
            var brightness = Math.Sqrt(color.R * color.R * RedBrightnessConst + color.G * color.G * GreenBrightnessConst + color.B * color.B * BlueBrightnessConst);
            if (brightness > BrightnessContrastThreshold || color.A < AlphaContrastThreshold) return new SolidColorBrush(Colors.Black);
            else return new SolidColorBrush(Colors.White);
        }
    }
}

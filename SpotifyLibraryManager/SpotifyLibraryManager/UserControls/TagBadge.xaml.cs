using SpotifyLibraryManager.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SpotifyLibraryManager.UserControls
{
    /// <summary>
    /// Interaction logic for TagBadge.xaml
    /// </summary>
    public partial class TagBadge : UserControl, INotifyPropertyChanged
    {
        private string _tagName;

        public string TagName
        {
            get { return _tagName; }
            set 
            { 
                _tagName = value;
                OnPropertyChanged();
            }
        }

        private string _tagColor;

        public string TagColor
        {
            get { return _tagColor; }
            set
            {
                _tagColor = value;
                TagNameTxt.Foreground = ContrastCalculator.GetContrastingBrush((Color)ColorConverter.ConvertFromString(value));
                OnPropertyChanged();
            }
        }

        public TagBadge()
        {
            InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

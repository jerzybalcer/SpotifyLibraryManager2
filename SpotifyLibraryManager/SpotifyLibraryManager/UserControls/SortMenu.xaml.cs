using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for SortMenu.xaml
    /// </summary>
    public partial class SortMenu : UserControl
    {
        public static readonly DependencyProperty CheckedOptionProperty = DependencyProperty.Register("CheckedOption", typeof(string), typeof(UserControl));
        public string CheckedOption
        {
            get => (string)GetValue(CheckedOptionProperty);
            set => SetValue(CheckedOptionProperty, value);
        }
        public SortMenu()
        {
            InitializeComponent();
        }

        private void MenuHeader_Click(object sender, RoutedEventArgs e)
        {
            if (!Popup.IsOpen)
            {
                Popup.IsOpen = true;
                Popup.HorizontalOffset = -(PopupBorder.ActualWidth - MenuHeader.ActualWidth) / 2d;
            }
            else
            {
                Popup.IsOpen = false;
            }
        }

        private void Popup_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Popup.IsOpen)
            {
                Popup.IsOpen = false;
            }
        }

        private void ItemBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border border = (Border)sender;
            TextBlock txt = (TextBlock)border.Child;

            if(CheckedOption != txt.Text)
            {
                CheckedOption = txt.Text;
                border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#181818"));

                foreach(Border notCheckedOption in OptionsContainer.Children)
                {
                    if(notCheckedOption != border)
                    {
                        notCheckedOption.Background = Brushes.Transparent;
                    }
                }
            }
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            Border border = (Border)sender;
            TextBlock txt = (TextBlock)border.Child;

            if (txt.Text != CheckedOption)
            {
                border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#575757"));
            }
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            Border border = (Border)sender;
            TextBlock txt = (TextBlock)border.Child;

            if (txt.Text != CheckedOption)
            {
                border.Background = Brushes.Transparent;
            }
        }
    }
}

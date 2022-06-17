﻿using SpotifyLibraryManager.Models;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SpotifyLibraryManager.UserControls
{
    /// <summary>
    /// Interaction logic for FilterMenu.xaml
    /// </summary>
    public partial class FilterMenu : UserControl
    {
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(IList), typeof(UserControl));
        public IList Items
        {
            get => (IList)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        public static readonly DependencyProperty FiltersProperty = DependencyProperty.Register("Filters", typeof(ObservableCollection<Tag>), typeof(UserControl), 
            new FrameworkPropertyMetadata(new ObservableCollection<Tag>(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public ObservableCollection<Tag> Filters
        {
            get => (ObservableCollection<Tag>)GetValue(FiltersProperty);
            set => SetValue(FiltersProperty, value);
        }

        public string MenuHeaderText { get; set; }

        public FilterMenu()
        {
            InitializeComponent();
            ItemsControl.DataContext = this;
            Filters = new ObservableCollection<Tag>();
        }

        private void MenuHeader_Click(object sender, RoutedEventArgs e)
        {
            if (!Popup.IsOpen)
            {
                Popup.IsOpen = true;
                Popup.HorizontalOffset = -(PopupBorder.ActualWidth-MenuHeader.ActualWidth) / 2d;
            }
            else
            {
                Popup.IsOpen = false;
            }
        }

        private void ThisControl_Loaded(object sender, RoutedEventArgs e)
        {
            MenuHeader.Content = MenuHeaderText;
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
            Tag tag = (Tag)border.DataContext;

            TextBlock checkMark = (TextBlock)border.FindName("CheckMark");
            checkMark.Visibility = checkMark.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;

            if (Filters.Contains(tag))
            {
                Filters.Remove(tag);
            }
            else
            {
                Filters.Add(tag);
            }
        }
    }
}

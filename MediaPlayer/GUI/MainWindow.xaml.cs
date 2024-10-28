using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void AddPlaylist_Click(object sender, RoutedEventArgs e)
        {
            // Logic to add a new playlist
        }

        private void DeletePlaylist_Click(object sender, RoutedEventArgs e)
        {
            // Logic to delete the selected playlist
        }

        private void AddSong_Click(object sender, RoutedEventArgs e)
        {
            // Logic to add a song/video to the selected playlist
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            // Logic for previous track
        }

        private void PlayPause_Click(object sender, RoutedEventArgs e)
        {
            // Logic to play or pause the current track
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            // Logic for next track
        }

        private void Shuffle_Click(object sender, RoutedEventArgs e)
        {
            // Logic to shuffle the playlist
        }

        private void Repeat_Click(object sender, RoutedEventArgs e)
        {
            // Logic to toggle repeat mode
        }
    }
}
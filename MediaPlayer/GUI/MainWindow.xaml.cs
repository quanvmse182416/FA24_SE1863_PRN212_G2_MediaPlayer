using GUI.Models;
using Microsoft.Win32;
using System.Collections.ObjectModel;
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
using System.Windows.Threading;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<Playlist> playlists;
        private int playlistCounter = 1;
        private bool isPlaying = false;
        private bool isShuffleEnabled = false;
        private bool isRepeatEnabled = false;
        private Random random = new Random();
        private Song currentSong;
        MediaPlayer player = new();
        private DispatcherTimer mediaTimer;

        public MainWindow()
        {
            InitializeComponent();
            playlists = new ObservableCollection<Playlist>();
            PlaylistListBox.ItemsSource = playlists;

            // Initialize the MediaPlayer
            InitializeMediaPlayer();

            // Add playlist selection changed handler
            PlaylistListBox.SelectionChanged += PlaylistListBox_SelectionChanged;
        }

        private void InitializeMediaPlayer()
        {
            // Set up timer for media updates
            mediaTimer = new DispatcherTimer();
            mediaTimer.Interval = TimeSpan.FromSeconds(1);
            mediaTimer.Tick += MediaTimer_Tick;

            // Add event handlers using MediaPlayer's events
            player.MediaOpened += (s, e) =>
            {
                mediaTimer.Start();
                isPlaying = true;
            };

            player.MediaFailed += (s, e) =>
            {
                MessageBox.Show("Error playing media: " + e.ErrorException.Message);
                mediaTimer.Stop();
                isPlaying = false;
            };

            player.MediaEnded += (s, e) =>
            {
                mediaTimer.Stop();
                if (isRepeatEnabled)
                {
                    PlayCurrentSong();
                }
                else
                {
                    Next_Click(null, null);
                }
            };
        }

        private void MediaTimer_Tick(object sender, EventArgs e)
        {
            // Update any UI elements that show playback progress
            if (currentSong != null && isPlaying)
            {
                // Update your progress bar or time display here if you have one
            }
        }

        private void PlaylistListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedPlaylist = PlaylistListBox.SelectedItem as Playlist;
            if (selectedPlaylist != null)
            {
                UpdateSongDisplay(selectedPlaylist);
            }
        }

        private void UpdateSongDisplay(Playlist playlist)
        {
            // Update your UI to show the songs in the selected playlist
            UpdatePlaylistDisplay();
        }

        private void PlayCurrentSong()
        {
            if (currentSong != null)
            {
                try
                {
                    player.Stop();
                    player.Open(new Uri(currentSong.FilePath));
                    player.Play();
                    isPlaying = true;
                    NowPlayingText.Text = $"Now Playing: {currentSong.Title}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error playing file: {ex.Message}", "Playback Error");
                    isPlaying = false;
                }
            }
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            var selectedPlaylist = PlaylistListBox.SelectedItem as Playlist;
            if (selectedPlaylist?.Songs == null || !selectedPlaylist.Songs.Any()) return;

            var currentIndex = selectedPlaylist.Songs.IndexOf(currentSong);
            if (currentIndex > 0)
            {
                currentSong = selectedPlaylist.Songs[currentIndex - 1];
            }
            else if (isRepeatEnabled)
            {
                currentSong = selectedPlaylist.Songs[selectedPlaylist.Songs.Count - 1];
            }
            else
            {
                return;
            }

            PlayCurrentSong();
        }

        private void PlayPause_Click(object sender, RoutedEventArgs e)
        {
            var selectedPlaylist = PlaylistListBox.SelectedItem as Playlist;
            if (selectedPlaylist?.Songs == null || !selectedPlaylist.Songs.Any())
            {
                MessageBox.Show("Please select a song to play.", "No Song Selected");
                return;
            }

            if (currentSong == null)
            {
                currentSong = selectedPlaylist.Songs.First();
                PlayCurrentSong();
                return;
            }

            if (isPlaying)
            {
                player.Pause();
                isPlaying = false;
                mediaTimer.Stop();
            }
            else
            {
                player.Play();
                isPlaying = true;
                mediaTimer.Start();
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            var selectedPlaylist = PlaylistListBox.SelectedItem as Playlist;
            if (selectedPlaylist?.Songs == null || !selectedPlaylist.Songs.Any()) return;

            if (isShuffleEnabled)
            {
                var currentIndex = selectedPlaylist.Songs.IndexOf(currentSong);
                int nextIndex;
                do
                {
                    nextIndex = random.Next(selectedPlaylist.Songs.Count);
                } while (nextIndex == currentIndex && selectedPlaylist.Songs.Count > 1);

                currentSong = selectedPlaylist.Songs[nextIndex];
            }
            else
            {
                var currentIndex = selectedPlaylist.Songs.IndexOf(currentSong);
                if (currentIndex < selectedPlaylist.Songs.Count - 1)
                {
                    currentSong = selectedPlaylist.Songs[currentIndex + 1];
                }
                else if (isRepeatEnabled)
                {
                    currentSong = selectedPlaylist.Songs[0];
                }
                else
                {
                    return;
                }
            }

            PlayCurrentSong();
        }

        private void Shuffle_Click(object sender, RoutedEventArgs e)
        {
            isShuffleEnabled = !isShuffleEnabled;
            var button = sender as Button;
            if (button != null)
            {
                button.Background = isShuffleEnabled ? Brushes.LightBlue : null;
            }
        }

        private void Repeat_Click(object sender, RoutedEventArgs e)
        {
            isRepeatEnabled = !isRepeatEnabled;
            var button = sender as Button;
            if (button != null)
            {
                button.Background = isRepeatEnabled ? Brushes.LightBlue : null;
            }
        }

        // Keep your existing methods (AddPlaylist_Click, DeletePlaylist_Click, AddSong_Click, UpdatePlaylistDisplay)

        private void AddPlaylist_Click(object sender, RoutedEventArgs e)
        {
            string playlistName = $"Playlist {playlistCounter++}";
            var newPlaylist = new Playlist(playlistName);
            playlists.Add(newPlaylist);
            PlaylistListBox.SelectedItem = newPlaylist;
        }

        private void DeletePlaylist_Click(object sender, RoutedEventArgs e)
        {
            var selectedPlaylist = PlaylistListBox.SelectedItem as Playlist;
            if (selectedPlaylist != null)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete '{selectedPlaylist.Name}'?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    playlists.Remove(selectedPlaylist);
                }
            }
            else
            {
                MessageBox.Show("Please select a playlist to delete.", "No Playlist Selected");
            }
        }

        private void AddSong_Click(object sender, RoutedEventArgs e)
        {
            var selectedPlaylist = PlaylistListBox.SelectedItem as Playlist;
            if (selectedPlaylist == null)
            {
                MessageBox.Show("Please select a playlist first.", "No Playlist Selected");
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "MP3 Files (*.mp3)|*.mp3",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filePath in openFileDialog.FileNames)
                {
                    try
                    {
                        // Using Path.GetFileNameWithoutExtension with explicit namespace
                        string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
                        var song = new Song(fileName, filePath);
                        selectedPlaylist.Songs.Add(song);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error adding file {filePath}: {ex.Message}", "Error");
                    }
                }

                // Update the ListBox if it's bound to the Songs collection
                UpdatePlaylistDisplay();
            }
        }

        private void UpdatePlaylistDisplay()
        {
            // Force the ListBox to refresh its display
            PlaylistListBox.Items.Refresh();
        }
    }
}


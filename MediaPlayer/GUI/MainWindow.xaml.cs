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
using MediaPlayer;
using System.Diagnostics;
using MediaPlayer.DAL.Models;
namespace MediaPlayer.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Media.MediaPlayer _mediaPlayer;
        private DispatcherTimer _timer;
        private BLL.MediaPlayer _mediaLogic;
        public MainWindow()
        {
            InitializeComponent();
            InitializePlayer();
        }

        private void InitializePlayer()
        {
            // Initialize MediaPlayer
            _mediaPlayer = new System.Windows.Media.MediaPlayer();
            _mediaPlayer.MediaEnded += MediaPlayer_MediaEnded!;

            // Initialize playlists
            _mediaLogic = new();
            playlistsListBox.SelectedItem = _mediaLogic.Playlists;

            // Initialize timer for progress updates
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick!;

            // Set initial volume
            _mediaPlayer.Volume = volumeSlider.Value;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_mediaPlayer.Source == null)
                return;
            if (_mediaPlayer.NaturalDuration.HasTimeSpan == false)
                return;

            if (!progressSlider.IsMouseCaptureWithin)
            {
                progressSlider.Value = (_mediaPlayer.Position.TotalSeconds / _mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds) * 100;
            }
            currentTimeTxt.Text = _mediaPlayer.Position.ToString(@"hh\:mm\:ss");
            totalTimeTxt.Text = _mediaPlayer.NaturalDuration.TimeSpan.ToString(@"hh\:mm\:ss");
        }

        private void MediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            PlayNextSong();
        }

        private void btnAddSong_Click(object sender, RoutedEventArgs e)
        {
            int currentPlaylistIndex = playlistsListBox.SelectedIndex;
            if (currentPlaylistIndex == -1)
            {
                MessageBox.Show("Please select playlist before adding song(s).", "Where?", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "MP3 files (*.mp3)|*.mp3",
                Multiselect = true
            };
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    _mediaLogic.addSong(filename, currentPlaylistIndex);
                }
                playlistsListBox.ItemsSource = _mediaLogic.Playlists;
            }
        }

        private void btnNewPlaylist_Click(object sender, RoutedEventArgs e)
        {
            string playlistName = Microsoft.VisualBasic.Interaction.InputBox("Enter playlist name:", "New Playlist", "");
            if (string.IsNullOrEmpty(playlistName))
            {
                MessageBox.Show("Playlist name cannot be empty", "Empty name!!!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            _mediaLogic.AddPlayList(playlistName);
            playlistsListBox.ItemsSource = _mediaLogic.Playlists;
            
        }

        private void songsListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PlaySelectedSong();
        }

        private void PlaySelectedSong()
        {
            if (songsListBox.SelectedItem != null)
            {
                var selectedSong = (Song)songsListBox.SelectedItem;

                _mediaPlayer.Open(new Uri(selectedSong.FilePath));
                _mediaPlayer.Play();
                _mediaLogic.IsPlaying = true;
                btnPlayPause.Content = "⏸";

                // Update song details display
                songTitleTxt.Text = selectedSong.Title;
                songArtistTxt.Text = selectedSong.Artist;
                songAlbumTxt.Text = selectedSong.Album;

                currentTimeTxt.Text = @"00\:00\:00";
                totalTimeTxt.Text = selectedSong.Duration;
                _timer.Start();
            }
        }

        private void btnPlayPause_Click(object sender, RoutedEventArgs e)
        {
            //if( )

            //if (isPlaying)
            //{
            //    _mediaPlayer.Pause();
            //    btnPlayPause.Content = "▶";
            //    _timer.Stop();
            //}
            //else
            //{
            //    _mediaPlayer.Play();
            //    btnPlayPause.Content = "⏸";
            //    _timer.Start();
            //}
            //isPlaying = !isPlaying;
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            //if (songsListBox.Items.Count == 0) return;

            //if (isShuffleEnabled)
            //{
            //    if (currentShuffleIndex <= 0)
            //    {
            //        if (isLoopEnabled)
            //        {
            //            // Go to the end of the current shuffle history
            //            currentShuffleIndex = shuffleHistory.Count - 1;
            //        }
            //        else return;
            //    }
            //    else
            //    {
            //        currentShuffleIndex--;
            //    }
            //    songsListBox.SelectedIndex = shuffleHistory[currentShuffleIndex];
            //}
            //else
            //{
            //    if (songsListBox.SelectedIndex > 0)
            //    {
            //        songsListBox.SelectedIndex--;
            //    }
            //    else if (isLoopEnabled)
            //    {
            //        songsListBox.SelectedIndex = songsListBox.Items.Count - 1;
            //    }
            //    else return;
            //}

            //PlaySelectedSong();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            //PlayNextSong();
        }

        private void PlayNextSong()
        {
            //if (songsListBox.Items.Count == 0) return;

            //if (isShuffleEnabled)
            //{
            //    if (shuffleHistory.Count == 0 || currentShuffleIndex >= shuffleHistory.Count - 1)
            //    {
            //        // Generate new shuffle history if we're at the end or don't have one
            //        shuffleHistory.Clear();
            //        for (int i = 0; i < songsListBox.Items.Count; i++)
            //        {
            //            shuffleHistory.Add(i);
            //        }
            //        // Shuffle the list
            //        for (int i = shuffleHistory.Count - 1; i > 0; i--)
            //        {
            //            int j = random.Next(i + 1);
            //            int temp = shuffleHistory[i];
            //            shuffleHistory[i] = shuffleHistory[j];
            //            shuffleHistory[j] = temp;
            //        }

            //        // If loop is enabled, start from beginning of new shuffle
            //        if (isLoopEnabled)
            //        {
            //            currentShuffleIndex = 0;
            //        }
            //        else
            //        {
            //            return; // Don't play if we're at the end and loop is disabled
            //        }
            //    }
            //    else
            //    {
            //        currentShuffleIndex++;
            //    }

            //    songsListBox.SelectedIndex = shuffleHistory[currentShuffleIndex];
            //}
            //else
            //{
            //    if (songsListBox.SelectedIndex < songsListBox.Items.Count - 1)
            //    {
            //        songsListBox.SelectedIndex++;
            //    }
            //    else if (isLoopEnabled)
            //    {
            //        songsListBox.SelectedIndex = 0;
            //    }
            //    else return;
            //}

            //PlaySelectedSong();
        }

        private void volumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //if (_mediaPlayer != null)
            //{
            //    _mediaPlayer.Volume = volumeSlider.Value;
            //}
        }

        private void progressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //if (_mediaPlayer.Source != null && _mediaPlayer.NaturalDuration.HasTimeSpan && progressSlider.IsMouseCaptureWithin)
            //{
            //    TimeSpan newPosition = TimeSpan.FromSeconds((progressSlider.Value / 100) * _mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds);
            //    _mediaPlayer.Position = newPosition;
            //    currentTimeTxt.Text = newPosition.ToString(@"mm\:ss");
            //}
        }

        private void progressSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            //_timer.Stop(); // Pause updates while dragging
        }

        private void progressSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            //if (_mediaPlayer.Source != null && _mediaPlayer.NaturalDuration.HasTimeSpan)
            //{
            //    TimeSpan newPosition = TimeSpan.FromSeconds((progressSlider.Value / 100) * _mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds);
            //    _mediaPlayer.Position = newPosition;
            //    currentTimeTxt.Text = newPosition.ToString(@"mm\:ss");
            //}
            //_timer.Start(); // Resume updates
        }

        private void btnShuffle_Click(object sender, RoutedEventArgs e)
        {
            //isShuffleEnabled = !isShuffleEnabled;
            //btnShuffle.Background = isShuffleEnabled ? new SolidColorBrush(Colors.LightBlue) : new SolidColorBrush(Colors.Transparent);

            //if (isShuffleEnabled)
            //{
            //    // Reset shuffle history
            //    shuffleHistory.Clear();
            //    for (int i = 0; i < songsListBox.Items.Count; i++)
            //    {
            //        shuffleHistory.Add(i);
            //    }
            //    // Shuffle the list
            //    for (int i = shuffleHistory.Count - 1; i > 0; i--)
            //    {
            //        int j = random.Next(i + 1);
            //        int temp = shuffleHistory[i];
            //        shuffleHistory[i] = shuffleHistory[j];
            //        shuffleHistory[j] = temp;
            //    }

            //    // Set current index based on currently playing song
            //    currentShuffleIndex = -1;
            //    if (songsListBox.SelectedIndex != -1)
            //    {
            //        currentShuffleIndex = shuffleHistory.IndexOf(songsListBox.SelectedIndex);
            //    }
            //}
        }

        private void btnLoop_Click(object sender, RoutedEventArgs e)
        {
            //isLoopEnabled = !isLoopEnabled;
            //btnLoop.Background = isLoopEnabled ? new SolidColorBrush(Colors.LightBlue) : new SolidColorBrush(Colors.Transparent);
        }
    }
}


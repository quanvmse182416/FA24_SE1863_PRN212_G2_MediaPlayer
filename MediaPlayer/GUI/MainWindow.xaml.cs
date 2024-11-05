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
using MediaPlayer.BLL;
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
            _mediaLogic.Load();
            playlistsListBox.ItemsSource = _mediaLogic.Playlists;

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
            //PlayNextSong();
            string result = _mediaLogic.NextSong();
            if( result == "Ok" )
                PlayCurrentSong();
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
            if (openFileDialog.ShowDialog() == false)
                return;
            
            BLL.MediaPlayer temp = _mediaLogic.ShallowCopy();
            foreach (string filename in openFileDialog.FileNames)
            {
                string result = _mediaLogic.addSong(filename, currentPlaylistIndex);
                if( result.Contains("File exist"))
                {
                    MessageBoxResult button = MessageBox.Show("Do you want to overwrite " + filename, "Song conflict", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                    if (button == MessageBoxResult.OK)
                        _mediaLogic.addSong(filename, currentPlaylistIndex, true);
                    else
                    {
                        _mediaLogic = temp;
                        return;
                    }
                }
            }
            playlistsListBox.ItemsSource = _mediaLogic.Playlists;
        }

        private void btnNewPlaylist_Click(object sender, RoutedEventArgs e)
        {
            string playlistName = Microsoft.VisualBasic.Interaction.InputBox("Enter playlist name:", "New Playlist", @"");
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
            //PlaySelectedSong();
        }

        private void prepareSong()
        {
            var selectedSong = _mediaLogic.CurrentSong;
            _mediaLogic.IsPlaying = false;
            _mediaPlayer.Open(new Uri(selectedSong.FilePath));
            btnPlayPause.Content = "▶";

            // Update song details display
            songTitleTxt.Text = selectedSong.Title;
            songArtistTxt.Text = selectedSong.Artist;
            songAlbumTxt.Text = selectedSong.Album;

            currentTimeTxt.Text = @"00:00:00";
            totalTimeTxt.Text = selectedSong.Duration;
        }

        private void btnPlayPause_Click(object sender, RoutedEventArgs e)
        {
            string result;
            Playlist currentSelected = (Playlist)playlistsListBox.SelectedItem;
            if ( _mediaLogic.CurrentSong == null && currentSelected == null)
            {
                result = _mediaLogic.DefaultPlay();
                if (result != "Ok")
                {
                    MessageBox.Show(result, "Error!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                prepareSong();
            }
            else if (_mediaLogic.CurrentSong == null && currentSelected != null)
            {
                if (currentSelected.Songs.Count == 0)
                {
                    MessageBox.Show("PLaylist is empty", "Empty playlist", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                _mediaLogic.CurrentSong = currentSelected.Songs[0];
                prepareSong();
            }
            
            _mediaLogic.TogglePlay();
            if (_mediaLogic.IsPlaying == false)
            {
                _mediaPlayer.Pause();
                btnPlayPause.Content = "▶";
                _timer.Stop();
            } else
            {
                _mediaPlayer.Play();
                btnPlayPause.Content = "⏸";
                _timer.Start();
            }
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            string result = _mediaLogic.PrevSong( true );
            if( result == "Ok" )
                PlayCurrentSong();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            string result = _mediaLogic.NextSong( true );
            if( result == "Ok" )
                PlayCurrentSong();
        }

        private void volumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_mediaPlayer != null)
            {
                _mediaPlayer.Volume = volumeSlider.Value;
            }
        }

        private void progressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_mediaPlayer.Source != null && _mediaPlayer.NaturalDuration.HasTimeSpan && progressSlider.IsMouseCaptureWithin)
            {
                TimeSpan newPosition = TimeSpan.FromSeconds((progressSlider.Value / 100) * _mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds);
                _mediaPlayer.Position = newPosition;
                currentTimeTxt.Text = newPosition.ToString(@"hh\:mm\:ss");
            }
        }

        private void progressSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            _timer.Stop(); // Pause updates while dragging
        }

        private void progressSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (_mediaPlayer.Source != null && _mediaPlayer.NaturalDuration.HasTimeSpan)
            {
                TimeSpan newPosition = TimeSpan.FromSeconds((progressSlider.Value / 100) * _mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds);
                _mediaPlayer.Position = newPosition;
                currentTimeTxt.Text = newPosition.ToString(@"mm\:ss");
            }
            _timer.Start(); // Resume updates
        }

        private void btnShuffle_Click(object sender, RoutedEventArgs e)
        {
            //isShuffleEnabled = !isShuffleEnabled;
            int selected = playlistsListBox.SelectedIndex;
            if (selected < 0)
                return;

            _mediaLogic.ToggleShuffle(selected);
            Color enabled = new Color() { R = 29, B = 84, G = 185, A = 255 };
            btnShuffle.Foreground = _mediaLogic.IsShuffleEnabled ? new SolidColorBrush(enabled) : new SolidColorBrush(Colors.White);
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
            //btnLoop.Background = isLoopEnabled ? new SolidColorBrush(Colors.LightBlue) : new SolidColorBrush(Colors.Transparent); 🔂
            if (_mediaLogic.IsRepeatAllEnabled)
            {
                _mediaLogic.ToggleRepeat();
                btnLoop.Content = "🔁";
            }
            else if (_mediaLogic.IsRepeatSingleEnabled)
            {
                _mediaLogic.ToggleAllRepeat();
                btnLoop.Content = "🔁";
            }
            else
            {
                _mediaLogic.ToggleSingleRepeat();
                btnLoop.Content = "🔂";
            }
            Color enabled = new Color() { R = 29, B = 84, G = 185, A = 255 };
            btnLoop.Foreground = _mediaLogic.IsRepeatEnabled() ? new SolidColorBrush(enabled) : new SolidColorBrush(Colors.White);
        }

        private void PlayCurrentSong()
        {
            if (_mediaLogic.CurrentSong == null)
                return;

            _mediaPlayer.Open(new Uri(_mediaLogic.CurrentSong.FilePath));
            _mediaPlayer.Play();
            _mediaLogic.IsPlaying = true;
            btnPlayPause.Content = "⏸";

            // Update song details display
            songTitleTxt.Text = _mediaLogic.CurrentSong.Title;
            songArtistTxt.Text = _mediaLogic.CurrentSong.Artist;
            songAlbumTxt.Text = _mediaLogic.CurrentSong.Album;

            currentTimeTxt.Text = "00:00";
            totalTimeTxt.Text = _mediaLogic.CurrentSong.Duration;
            _timer.Start();
            
        }
    }
}


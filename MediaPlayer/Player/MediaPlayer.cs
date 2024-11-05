using System.Collections.ObjectModel;
using System.Reflection.Metadata.Ecma335;
using MediaPlayer.DAL.Models;
using MediaPlayer.DAL.Repositories;
using System.IO;
using System.Text.Json;
namespace MediaPlayer.BLL;
public class MediaPlayer
{
    private PlaylistRepository _playlistRepo;
    private SongRepository _songRepo;
    public ObservableCollection<Playlist> Playlists { get; set; }
    public int PlaylistCounter { get; set; }
    public bool IsPlaying { get; set; }
    public bool IsShuffleEnabled { get; set; }
    public List<int> ShuffleRemainIndex { get; set; }
    public List<int> PlayedShuffleSong { get; set; }
    public bool IsRepeatSingleEnabled { get; set; }
    public bool IsRepeatAllEnabled { get; set; }
    public Random Random { get; set; }
    public Song CurrentSong { get; set; }

    public MediaPlayer()
    {
        _playlistRepo = new();
        _songRepo = new();
        Random = new();
        Playlists = _playlistRepo.LoadPlaylists();
    }

    public bool ExistSongInPlaylist( int playlistIndex, Song song = null)
    {
        if (song == null)
            return false;
        foreach( Song temp in Playlists[playlistIndex].Songs)
        {
            if (temp.IsEquals(song))
                return true;
        }
        return false;
    }
    public string addSong(string sourceDir, int playListIndex, bool overwrite = false)
    {
        string[] paths = sourceDir.Split(@"\");
        string result = _songRepo.CopySong(sourceDir, paths[paths.Length - 1], overwrite);
        if (result != "Ok")
            return result;
        Song temp = _songRepo.GetInfo(sourceDir);
        if(ExistSongInPlaylist(playListIndex, temp) == false )
            Playlists[playListIndex].Songs.Add(temp);
        Save();
        return "Ok";
    }
    public MediaPlayer ShallowCopy()
    {
        return (MediaPlayer)MemberwiseClone();
    }
    public void TogglePlay()
    {
        IsPlaying = !IsPlaying;
    }

    public void ResetShuffleRemain(int playlistIndex)
    {
        if (ShuffleRemainIndex == null)
            ShuffleRemainIndex = new List<int>();
        if (PlayedShuffleSong == null)
            PlayedShuffleSong = new List<int>();
        for (int i = 0; i < Playlists[playlistIndex].Songs.Count; i++)
        {
            ShuffleRemainIndex.Add(i);
        }
        PlayedShuffleSong.Clear();
    }

    public void ToggleShuffle(int playlistIndex)
    {
        IsShuffleEnabled = !IsShuffleEnabled;
        if( IsShuffleEnabled )
            ResetShuffleRemain(playlistIndex);
        else
        {
            ShuffleRemainIndex = null;
            PlayedShuffleSong = null;
        }
    }

    public void ToggleSingleRepeat()
    {
        IsRepeatSingleEnabled = !IsRepeatSingleEnabled;
        IsRepeatAllEnabled = false;
    }

    public void ToggleAllRepeat()
    {
        IsRepeatAllEnabled = !IsRepeatAllEnabled;
        IsRepeatSingleEnabled = false;
    }

    public void ToggleRepeat()
    {
        IsRepeatAllEnabled = false;
        IsRepeatSingleEnabled = false;
    }

    public bool IsRepeatEnabled()
    {
        return IsRepeatSingleEnabled || IsRepeatAllEnabled;
    }

    public int PlaylistIndex( Song target )
    {
        if (Playlists.Count == 0 || target == null)
            return -1;
        for( int index = 0; index < Playlists.Count; ++index )
        {
            if (Playlists[index].Songs.Contains(target))
                return index;
        }
        return -1;
    }

    internal int GetNextShuffleSongIndex( int playlistIndex)
    {
        if (ShuffleRemainIndex.Count == 0)
        {
            if (IsRepeatAllEnabled == false)
                return -2;
            ResetShuffleRemain(playlistIndex);
        }
        int result = Random.Next(0, ShuffleRemainIndex.Count - 1);
        int temp = ShuffleRemainIndex[result];
        ShuffleRemainIndex.Remove(temp);
        PlayedShuffleSong.Add(temp);
        return temp;
    }

    /// <summary>
    /// Get next index of the current song inside a playlist for repeatAll, shuffle and vice versa
    /// </summary>
    /// <param name="playlistIndex"></param>
    /// <returns>-1: Error<br/>-2: End</returns>
    public int GetNextSongIndex( int playlistIndex )
    {
        if (playlistIndex < 0 || playlistIndex >= Playlists.Count )
            return -1;
        if( IsShuffleEnabled )
        {
            return GetNextShuffleSongIndex(playlistIndex);
        }
        if (CurrentSong == null)
        {
            if (Playlists[playlistIndex].Songs.Count <= 0)
                return -1;
            return 0;
        }
        for ( int index = 0; index < Playlists[playlistIndex].Songs.Count; ++index )
        {
            if (Playlists[playlistIndex].Songs[index].IsEquals(CurrentSong))
            {
                if (index == Playlists[playlistIndex].Songs.Count - 1)
                {
                    if (IsRepeatAllEnabled)
                        return 0;
                    return -2;
                }
                else return index + 1;
            }
        }
        return -1;
    }

    internal int GetPrevShuffleSongIndex(int playlistIndex)
    {
        if (PlayedShuffleSong.Count == 0)
        {
            if (IsRepeatAllEnabled == false)
                return -2;
            ResetShuffleRemain(playlistIndex);
        }
        int result = Random.Next(0, PlayedShuffleSong.Count - 1);
        int temp = PlayedShuffleSong[result];
        PlayedShuffleSong.Remove(temp);
        ShuffleRemainIndex.Add(temp);
        return temp;
    }

    public int GetPrevSongIndex(int playlistIndex)
    {
        if (playlistIndex < 0 || playlistIndex >= Playlists.Count)
            return -1;
        if (IsShuffleEnabled)
        {
            return GetPrevShuffleSongIndex(playlistIndex);
        }
        if (CurrentSong == null)
        {
            if (Playlists[playlistIndex].Songs.Count <= 0)
                return -1;
            return 0;
        }
        for (int index = 0; index < Playlists[playlistIndex].Songs.Count; ++index)
        {
            if (Playlists[playlistIndex].Songs[index].IsEquals(CurrentSong))
            {
                if (index == 0)
                {
                    if (IsRepeatAllEnabled)
                        return Playlists[playlistIndex].Songs.Count - 1;
                    return -2;
                }
                else return index - 1;
            }
        }
        return -1;
    }

    public string NextSong(bool isUserInput = false , Song song = null)
    {
        if (Playlists.Count == 0)
            return "There is no playlist available";
        if (isUserInput == false && IsRepeatSingleEnabled)
            return "Ok";
        if (song != null)
            CurrentSong = song;
        int playlistIndex = PlaylistIndex( CurrentSong );
        if (playlistIndex == -1)
            return "Invalid";

        int songIndex = GetNextSongIndex(playlistIndex);
        if (songIndex == -1 )
            return "Invalid";
        if (songIndex == -2)
            return "Stop";
        CurrentSong = Playlists[playlistIndex].Songs[songIndex];
        return "Ok";
    }

    public string PrevSong(bool isUserInput = false, Song song = null)
    {
        if (Playlists.Count == 0)
            return "There is no playlist available";
        if (isUserInput == false && IsRepeatSingleEnabled)
            return "Ok";
        if (song != null)
            CurrentSong = song;
        int playlistIndex = PlaylistIndex(CurrentSong);
        if (playlistIndex == -1)
            return "Invalid";

        int songIndex = GetPrevSongIndex(playlistIndex);
        if (songIndex == -1)
            return "Invalid";
        if (songIndex == -2)
            return "Stop";
        CurrentSong = Playlists[playlistIndex].Songs[songIndex];
        return "Ok";
    }

    public string AddPlayList(string name)
    {
        if (string.IsNullOrEmpty(name))
            return "Name cannot be empty";
        foreach (Playlist playlist in Playlists)
        {
            if (playlist.Name == name)
                return "There's already plaaylist with that name";
        }
        Playlist temp = new Playlist(name);
        Playlists.Add(temp);
        Save();
        return "Ok";
    }

    public string DefaultPlay(Playlist temp = null)
    {
        if( temp != null && temp.Songs.Count > 0)
        {
            CurrentSong = temp.Songs[0];
            return "Ok";
        }

        foreach( Playlist playlist in Playlists)
        {
            if( playlist.Songs.Count > 0)
            {
                CurrentSong = playlist.Songs[0];
                return "Ok";
            }
        }
        return "No songs can be played";
    }

    public void Save()
    {
        const string file = "save.bin";
        FileRepository fileManager = new();
        fileManager.WriteFile(this, file);
    }
    public bool Load()
    {
        const string file = "save.bin";
        FileRepository fileManager = new();
        string json = fileManager.ReadFile(file);
        if (json == null)
            return false;
        MediaPlayer temp = JsonSerializer.Deserialize<MediaPlayer>(json);
        if (temp == null)
            return false;
        this.Playlists = temp.Playlists;
        return true;
    }
}

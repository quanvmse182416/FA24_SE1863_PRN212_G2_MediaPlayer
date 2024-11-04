using System.Collections.ObjectModel;
using System.Reflection.Metadata.Ecma335;
using MediaPlayer.DAL.Models;
using MediaPlayer.DAL.Repositories;

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
    public void addSong(string sourceDir, int playListIndex)
    {
        string[] paths = sourceDir.Split(@"\");
        _songRepo.CopySong(sourceDir, paths[paths.Length - 1]);
        Song temp = _songRepo.GetInfo(sourceDir);
        Playlists[playListIndex].Songs.Add(temp);
    }

    public void TogglePlay()
    {
        IsPlaying = !IsPlaying;
    }

    public void ResetShuffleRemain(int playlistIndex)
    {
        if (ShuffleRemainIndex == null)
            ShuffleRemainIndex = new List<int>();
        for (int i = 0; i < Playlists[playlistIndex].Songs.Count; i++)
        {
            ShuffleRemainIndex.Add(i);
        }
    }

    public void ToggleShuffle(int playlistIndex)
    {
        IsShuffleEnabled = !IsShuffleEnabled;
        ResetShuffleRemain(playlistIndex);
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
        ShuffleRemainIndex.Remove(result);
        return result;
    }

    /// <summary>
    /// Get next index of the current song inside a playlist for repeatAll, shuffle and vice versa
    /// </summary>
    /// <param name="playlistIndex"></param>
    /// <returns>-1: Error<br/>-2: End</returns>
    public int GetNextSongIndex( int playlistIndex )
    {
        if (playlistIndex < 0 || playlistIndex >= Playlists.Count || CurrentSong == null )
            return -1;
        if( IsShuffleEnabled )
        {
            return GetNextShuffleSongIndex(playlistIndex);
        }
        for( int index = 0; index < Playlists[playlistIndex].Songs.Count; ++index )
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
    
    public string NextSong(bool isUserInput = false , Song song = null)
    {
        if (Playlists.Count == 0)
            return "There is no playlist available";
        if (isUserInput == false && IsRepeatSingleEnabled)
            return "Repeat";
        if (song != null)
            CurrentSong = song;
        int playlistIndex = PlaylistIndex( CurrentSong );
        if (playlistIndex == -1)
            return "Nothing";

        int songIndex = GetNextSongIndex(playlistIndex);
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
        return "Ok";
    }
}

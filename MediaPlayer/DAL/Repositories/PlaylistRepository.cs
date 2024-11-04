using MediaPlayer.DAL.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPlayer.DAL.Repositories
{
    public class PlaylistRepository
    {
        public ObservableCollection<Playlist> LoadPlaylists()
        {
            return new ObservableCollection<Playlist>();
        }
    }
}

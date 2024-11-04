using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPlayer.DAL.Models;

public class Playlist
{
    public string Name { get; set; }
    public ObservableCollection<Song> Songs { get; set; }

    public Playlist(string name)
    {
        Name = name;
        Songs = new ObservableCollection<Song>();
    }

    public override string ToString()
    {
        return Name;
    }
}

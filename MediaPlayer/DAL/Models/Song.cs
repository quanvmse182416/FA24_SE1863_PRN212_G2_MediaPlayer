#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPlayer.DAL.Models;

public class Song
{
    public string Title { get; set; }
    public string FilePath { get; set; }
    public string Duration { get; set; }
    public string Artist { get; set; }
    public string Album { get; set; }

    public Song()
    {
    }

    public override string ToString()
    {
        return Title;
    }

    public bool IsEquals( Song song)
    {
        int condition = 0;
        if (this.Title == song.Title) ++condition;
        if (this.FilePath == song.FilePath) ++condition;
        if (condition == 2)
            return true;
        return false;
    }

}

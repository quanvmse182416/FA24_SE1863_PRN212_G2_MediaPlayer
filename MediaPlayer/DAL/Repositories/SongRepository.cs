using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaPlayer.DAL.Models;
using TagLib;

namespace MediaPlayer.DAL.Repositories;

public class SongRepository : FileRepository
{
    public string CopySong(string sourceDir, string fileName, bool overwrite = false)
    {
        return base.CopyFile(sourceDir, fileName, overwrite);
    }
    private Song GetSongInfo(string filePath)
    {
        var file = TagLib.File.Create(filePath);
        return new Song()
        {
            FilePath = filePath,
            Title = string.IsNullOrEmpty(file.Tag.Title) ?
                System.IO.Path.GetFileNameWithoutExtension(filePath) : file.Tag.Title,
            Artist = string.IsNullOrEmpty(file.Tag.FirstPerformer) ?
                "Unknown Artist" : file.Tag.FirstPerformer,
            Album = string.IsNullOrEmpty(file.Tag.Album) ?
                "Unknown Album" : file.Tag.Album,
            Duration = file.Properties.Duration.ToString(@"hh\:mm\:ss"),
        };
    }
}
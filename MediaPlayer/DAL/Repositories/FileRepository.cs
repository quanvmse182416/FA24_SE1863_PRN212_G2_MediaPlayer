using MediaPlayer.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPlayer.DAL.Repositories
{
    public class FileRepository
    {
        internal void CopyFile( string SourceDir, string fileName)
        {
            string destDirectory = System.IO.Directory.GetCurrentDirectory() + @"\data\";
            if (Directory.Exists(destDirectory) == false)
                Directory.CreateDirectory(destDirectory);
            File.Copy(SourceDir, destDirectory + fileName);
        }
        public Song GetInfo(string filePath)
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
}

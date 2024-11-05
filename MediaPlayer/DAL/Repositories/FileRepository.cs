using MediaPlayer.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
namespace MediaPlayer.DAL.Repositories
{
    public class FileRepository
    {
        internal string CopyFile( string SourceDir, string fileName, bool overwrite = false)
        {
            string destDirectory = System.IO.Directory.GetCurrentDirectory() + @"\data\";
            if (Directory.Exists(destDirectory) == false)
                Directory.CreateDirectory(destDirectory);
            string fileDest = destDirectory + fileName;
            if( File.Exists( fileDest ) && !overwrite)
                return "File exist";
            File.Copy(SourceDir, destDirectory + fileName, overwrite);
            return "Ok";
        }

        public void WriteFile(Object obj, string file)
        {
            string jsonString = JsonSerializer.Serialize(obj);
            File.WriteAllText(file, jsonString);
        }

        public string ReadFile(string file)
        {
            if (File.Exists(file) == false)
                return null;
            return File.ReadAllText(file);
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

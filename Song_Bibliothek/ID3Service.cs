using Microsoft.AspNetCore.Mvc.RazorPages;
using Song_Bibliothek.Pages.Album;
using Song_Bibliothek.Pages.Artists;
using Song_Bibliothek.Pages.Songs;

namespace Song_Bibliothek
{
    public class ID3Service
    {
        public SongInfo GetMetaDataSong(HttpRequest request, SongInfo songInfo)
        {
            try
            {
                //Recieve request
                var mp3File = request.Form.Files["file"];

                string filePath = "";

                if (mp3File == null)
                {
                    throw new FileNotFoundException();
                }

                //Create file for server to save
                filePath = Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "AudioFiles"), mp3File.FileName);

                songInfo.fileFormat = filePath;


                //Get Metadata from file
                var metaData = TagLib.File.Create(filePath);

                songInfo.title = metaData.Tag.Title ?? "Unknown";
                songInfo.artist = metaData.Tag.AlbumArtists.FirstOrDefault("Unknown");
                songInfo.album = metaData.Tag.Album ?? "Unknown";
                songInfo.year = metaData.Tag.Year.ToString() ?? "Unknown";
                songInfo.track = metaData.Tag.Track.ToString() ?? "Unknown";

                string replaceTitle = songInfo.title.Replace("'", "´");
                string replaceAlbum = songInfo.album.Replace("'", "´");
                string replaceLyrics = songInfo.lyrics.Replace("'", "´");
                string replaceArtist = songInfo.artist.Replace("'", "´");
                string replaceFileFormat = songInfo.fileFormat.Replace("'", "´");

                songInfo.title = replaceTitle;
                songInfo.artist = replaceArtist;
                songInfo.album = replaceAlbum;
                songInfo.lyrics = replaceLyrics;
                songInfo.fileFormat = replaceFileFormat;

                //Copy file for application to use
                using (var fileStreamCreate = new FileStream(songInfo.fileFormat, FileMode.Create))
                {
                    mp3File.CopyTo(fileStreamCreate);
                }

                if (mp3File == null || mp3File.Length == 0)
                    throw new FileNotFoundException();

                //Convert to byte to save in database
                var fileStreamOpen = new FileStream(songInfo.fileFormat, FileMode.Open);

                songInfo.data = new byte[fileStreamOpen.Length];

                fileStreamOpen.Read(songInfo.data, 0, (int)fileStreamOpen.Length);

                return songInfo;

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

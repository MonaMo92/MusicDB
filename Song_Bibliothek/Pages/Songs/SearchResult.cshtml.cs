using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using NAudio.Wave;

namespace Song_Bibliothek.Pages.Songs
{
    public class SearchResultModel : PageModel
    {
        public List<SongInfo> SongList = new List<SongInfo>();
        public Dictionary<string, string> songs;
        public string title;
        public string AudioPath { get; set; }

        public void OnGet()
        {
            try
            {
                string connectionString = "server=host.docker.internal;uid=root;pwd=root;database=musicdb";
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    if (connection.State == System.Data.ConnectionState.Closed)
                    {
                        connection.Open();
                    }

                    title = Request.Query["title"];
                    string sql = "";
                    if (!string.IsNullOrEmpty(title))
                    {
                        sql = "SELECT * FROM songs " +
                        "JOIN album ON album.album_id = songs.album_id " +
                        "JOIN artists ON album.artist_id = artists.artist_id " +
                        "WHERE song_title=@title";
                    }

                    songs = new Dictionary<string, string>
                    {
                        { "American Idiot", "Tracks/American Idiot.mp3" },
                        { "Chop Suey", "Tracks/Chop Suey.mp3" },
                        { "Fly Away", "Tracks/Fly Away.mp3" },
                        { "Hit The Floor", "Tracks/Hit The Floor.mp3" },
                        { "I Dont Like Metal", "Tracks/I Dont Like Metal.mp3" },
                        { "Lost In Hollywood", "Tracks/Lost In Hollywood.mp3" },
                        { "Radio Video", "Tracks/Radio Video.mp3" },
                        { "Runaway", "Tracks/Runaway.mp3" },
                        { "Tears Don't Fall", "Tracks/Tears Dont Fall.mp3" },
                        { "The Kill", "Tracks/The Kill.mp3" },
                        { "Wessi Girl", "Tracks/Wessi Girl.mp3" }
                    };

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@title", title);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                SongInfo songInfo = new SongInfo();
                                songInfo.id = "" + reader.GetInt32(0);
                                songInfo.album = reader.GetString(9);
                                songInfo.title = reader.GetString(2);
                                songInfo.track = reader.GetString(3);
                                songInfo.artist = reader.GetString(13);
                                songInfo.lyrics = reader.GetString(4);
                                songInfo.year = reader.GetString(14);

                                SongList.Add(songInfo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
            }
            Index();
        }

        public enum ContentType
        {
            mp3
        }

        public IActionResult Index()
        {
            string audioPath = "";
            
            foreach (var song in songs)
            {
                if (song.Key == title)
                {
                    audioPath = song.Value;
                    AudioPath = audioPath;
                }
            }

            // funktioniert theoretisch, aber der Pfad wird aus unbekannten Gründen nicht erkannt, daher kann die Audiodatei nicht abgespielt werden
            /*byte[] mpegAudioBytes = ConvertMp3ToMpeg(audioPath);

            if (mpegAudioBytes != null)
                return File(mpegAudioBytes, "audio/mpeg", "converted_audio.mpeg");
            else
            {
                return BadRequest("Konvertierung fehlgeschlagen.");
            }*/

            return Content(audioPath);
        }

        private byte[] ConvertMp3ToMpeg(string mp3FilePath)
        {
            using (var mp3Reader = new Mp3FileReader(mp3FilePath))
            {
                using (var mpegStream = new MemoryStream())
                {
                    var waveFormat = new WaveFormat(44100, 2);
                    using (var mpegWriter = new WaveFileWriter(mpegStream, waveFormat))
                    {
                        var buffer = new byte[4096];
                        int bytesRead;

                        while ((bytesRead = mp3Reader.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            mpegWriter.Write(buffer, 0, bytesRead);
                        }
                    }

                    return mpegStream.ToArray();
                }
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

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
                        { "American Idiot", "~/Songs/American Idiot.mp3" },
                        { "Chop Suey", "~/Songs/Chop Suey.mp3" },
                        { "Fly Away", "~/Songs/Fly Away.mp3" },
                        { "Hit The Floor", "~/Songs/Hit The Floor.mp3" },
                        { "I Dont Like Metal", "~/Songs/I Dont Like Metal.mp3" },
                        { "Lost In Hollywood", "~/Songs/Lost In Hollywood.mp3" },
                        { "Radio Video", "~/Songs/Radio Video.mp3" },
                        { "Runaway", "~/Songs/Runaway.mp3" },
                        { "Tears Don't Fall", "~/Songs/Tears Dont Fall.mp3" },
                        { "The Kill", "./Songs/The Kill.mp3" },
                        { "Wessi Girl", "~/Songs/Wessi Girl.mp3" }
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
                                songInfo.album = reader.GetString(7);
                                songInfo.title = reader.GetString(2);
                                songInfo.track = reader.GetString(3);
                                songInfo.artist = reader.GetString(11);
                                songInfo.lyrics = reader.GetString(4);
                                songInfo.year = reader.GetString(8);

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
            return Content(audioPath);
        }
    }
}

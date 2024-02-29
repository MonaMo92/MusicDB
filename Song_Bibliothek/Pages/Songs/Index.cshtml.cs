using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Media;
using System.Reflection.PortableExecutable;

namespace Song_Bibliothek.Pages.Songs
{
    public class IndexModel : PageModel
    {
        public List<SongInfo> SongList = new List<SongInfo>();

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

                    string title = Request.Query["title"];
                    string sql = "SELECT * FROM songs " +
                        "JOIN album ON album.album_id = songs.album_id " +
                        "JOIN artists ON album.artist_id = artists.artist_id";
                
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
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
        }
    }

    public class SongInfo
    {
        public string? id;
        public string? album;
        public string? title;
        public string? track;
        public string? artist;
        public string? year;
        public string? lyrics;
    }
}

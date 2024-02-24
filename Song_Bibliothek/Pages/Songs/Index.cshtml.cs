using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
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
                string connectionString = "server=localhost;uid=root;pwd=root;database=musicdb";    // data source
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    if (connection.State == System.Data.ConnectionState.Closed)
                    {
                        connection.Open();  // open SQL connection, if it's not open already
                    }

                    // SQL query
                    string sql = "SELECT * FROM songs " +
                        "JOIN album ON album.album_id = songs.album_id " +
                        "JOIN artists ON album.artist_id = artists.artist_id";

                    // execute the SQL query
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // add the data to their objects
                                SongInfo songInfo = new SongInfo();
                                songInfo.id = "" + reader.GetInt32(0);
                                songInfo.album = reader.GetString(7);
                                songInfo.title = reader.GetString(2);
                                songInfo.track = reader.GetString(3);
                                songInfo.artist = reader.GetString(11);
                                songInfo.lyrics = reader.GetString(4);
                                songInfo.year = reader.GetString(8);

                                SongList.Add(songInfo);   // store the data in a list for the landing page
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

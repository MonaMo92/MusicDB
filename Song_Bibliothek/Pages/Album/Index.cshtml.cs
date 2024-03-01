using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace Song_Bibliothek.Pages.Album
{
    public class IndexModel : PageModel
    {
        public List<AlbumInfo> AlbumList = new List<AlbumInfo>();
        
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

                    string sql = "SELECT * FROM album " +
                        "JOIN artists ON album.artist_id = artists.artist_id " +
                        "ORDER BY album.album_id";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                AlbumInfo albumInfo = new AlbumInfo();
                                albumInfo.id = "" + reader.GetInt32(1);
                                albumInfo.artist = reader.GetString(6);
                                albumInfo.title = reader.GetString(2);
                                albumInfo.year = reader.GetString(3);
                                albumInfo.label = reader.GetString(4);

                                AlbumList.Add(albumInfo);
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
    public class AlbumInfo
    {
        public string? id;
        public string? artist;
        public string? title;
        public string? year;
        public string? label;
    }
}

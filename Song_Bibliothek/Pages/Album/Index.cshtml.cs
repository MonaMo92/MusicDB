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
                string connectionString = "server=localhost;uid=root;pwd=root;database=musicdb";    // data source

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    if (connection.State == System.Data.ConnectionState.Closed)
                    {
                        connection.Open();  // open SQL connection, if it's not open already
                    }

                    // SQL query
                    string sql = "SELECT * FROM album " +
                        "JOIN artists ON album.artist_id = artists.artist_id " +
                        "ORDER BY album.album_id";

                    // execute the SQL query
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // add the data to their objects
                                AlbumInfo albumInfo = new AlbumInfo();
                                albumInfo.id = "" + reader.GetInt32(1);
                                albumInfo.artist = reader.GetString(6);
                                albumInfo.title = reader.GetString(2);
                                albumInfo.year = reader.GetString(3);
                                albumInfo.label = reader.GetString(4);

                                AlbumList.Add(albumInfo);   // store the data in a list for the landing page
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

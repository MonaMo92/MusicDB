using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace Song_Bibliothek.Pages.Artists
{
    public class IndexModel : PageModel
    {
        public List<ArtistInfo> ArtistsList = new List<ArtistInfo>();
        
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

                    string sql = "SELECT * FROM artists " +
                        "JOIN genre ON genre.genre_id = artists.genre " +
                        "ORDER BY artists.artist_id";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ArtistInfo artistInfo = new ArtistInfo();
                                artistInfo.id = "" + reader.GetInt32(0);
                                artistInfo.name = reader.GetString(1);
                                artistInfo.year = reader.GetString(2);
                                artistInfo.origin = reader.GetString(3);
                                artistInfo.genre = reader.GetString(6);

                                ArtistsList.Add(artistInfo);
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
    public class ArtistInfo
    {
        public string? id;
        public string? name;
        public string? year;
        public string? origin;
        public string? genre;
        public string? title;
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;

namespace Song_Bibliothek.Pages.Artists
{
    public class EditModel : PageModel
    {
        public ArtistInfo artistInfo = new ArtistInfo();
        public string errorMessage = "";
        public string successMessage = "";
        private string connectionString = "server=host.docker.internal;uid=root;pwd=root;database=musicdb";
        public void OnGet()
        {
            string id = Request.Query["artist_id"];

            try
            {                
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    if (connection.State == System.Data.ConnectionState.Closed)
                    {
                        connection.Open();
                    }

                    string sql = "SELECT * FROM artists";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ArtistInfo artistInfo = new ArtistInfo();
                                artistInfo.id = "" + reader.GetInt32(0);
                                artistInfo.name = reader.GetString(1);
                                artistInfo.year = reader.GetString(2);
                                artistInfo.origin = reader.GetString(3);
                                artistInfo.genre = reader.GetString(4);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        public void OnPost()
        {
            artistInfo.id = Request.Query["id"];
            artistInfo.name = Request.Form["name"];
            artistInfo.year = Request.Form["year"];
            artistInfo.origin = Request.Form["origin"];
            artistInfo.genre = Request.Form["genre"];

            if (artistInfo.name.Length == 0 || artistInfo.year.Length == 0
                || artistInfo.origin.Length == 0 || artistInfo.genre.Length == 0)
            {
                errorMessage = "Please enter all of the fields";
                return;
            }

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    if (connection.State == System.Data.ConnectionState.Closed)
                    {
                        connection.Open();
                    }

                    string sql = "UPDATE artists " +
                        "SET artist_name=@name, year=@year, origin=@origin, genre=(SELECT genre_id FROM genre WHERE genre_name = @genre) " +
                        "WHERE artist_name=@name";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        // add the data to the command
                        command.Parameters.AddWithValue("@name", artistInfo.name);
                        command.Parameters.AddWithValue("@year", artistInfo.year);
                        command.Parameters.AddWithValue("@origin", artistInfo.origin);
                        command.Parameters.AddWithValue("@genre", artistInfo.genre);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            Response.Redirect("/Artists/Index");
        }
    }
}

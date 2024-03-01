using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace Song_Bibliothek.Pages.Artists
{
    public class CreateModel : PageModel
    {
        public ArtistInfo artistInfo = new ArtistInfo();
        public string errorMessage = "";
        public string successMessage = "";

        public void OnGet()
        {
        }

        public void OnPost() 
        {
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
                string connectionString = "server=host.docker.internal;uid=root;pwd=root;database=musicdb";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    if (connection.State == System.Data.ConnectionState.Closed)
                    {
                        connection.Open();
                    }

                    string stmt = "SELECT COUNT(*) FROM genre";
                    int count = 0;

                    using (MySqlCommand cmd = new MySqlCommand(stmt, connection))
                    {
                        count = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    //Check if genre exists
                    stmt = "SELECT COUNT(*) FROM genre WHERE genre_name = '" + artistInfo.genre + "'";
                    int genreCount = 0;

                    using (MySqlCommand cmd = new MySqlCommand(stmt, connection))
                    {
                        genreCount = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // SQL query
                    string sql = "";

                    if (count == 0 || genreCount == 0)
                    {
                        sql = "INSERT INTO genre (genre_name)" +
                            "VALUES (@genre);" +
                            "INSERT INTO artists (artist_name, year, origin, genre)" +
                            "VALUES (@name, @year, @origin, (SELECT genre_id FROM genre WHERE genre_name = @genre))";
                    }
                    else
                    {
                        sql = "INSERT INTO artists (artist_name, year, origin, genre)" +

                        "VALUES (@name, @year, @origin, (SELECT genre_id FROM genre WHERE genre_name = @genre))";
                    }    
                    

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@name", artistInfo.name);
                        command.Parameters.AddWithValue("@year", artistInfo.year);
                        command.Parameters.AddWithValue("@origin", artistInfo.origin);
                        command.Parameters.AddWithValue("@genre", artistInfo.genre);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch(Exception ex)
            { 
                errorMessage = ex.Message;
                return;
            }

            artistInfo.name = "";
            artistInfo.year = "";
            artistInfo.origin = "";
            artistInfo.genre = "";
            successMessage = "New Artist added";

            Response.Redirect("/Artists/Index");
        }
    }
}

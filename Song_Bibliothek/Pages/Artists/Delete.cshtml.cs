using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace Song_Bibliothek.Pages.Artists
{
    public class DeleteModel : PageModel
    {
        public ArtistInfo artistInfo = new ArtistInfo();
        public string errorMessage = "";
        public string successMessage = "";
        private string connectionString = "server=localhost;uid=root;pwd=root;database=musicdb";    // data source

        public void OnGet()
        {
            string id = Request.Query["artist_id"];

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    if (connection.State == System.Data.ConnectionState.Closed)
                    {
                        connection.Open();  // open SQL connection, if it's not open already
                    }

                    // SQL query
                    string sql = "SELECT * FROM artists";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // add the data to their objects
                                ArtistInfo artistInfo = new ArtistInfo();
                                artistInfo.name = reader.GetString(1);
                                artistInfo.year = reader.GetString(2);
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
            try
            {
                // save the data entered by the user
                artistInfo.name = Request.Form["name"];
                artistInfo.year = Request.Form["year"];

                // check whether all fields have content
                if (artistInfo.name.Length == 0 || artistInfo.year.Length == 0)
                {
                    errorMessage = "Please enter all of the fields";
                    return;
                }

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    if (connection.State == System.Data.ConnectionState.Closed)
                    {
                        connection.Open();  // open SQL connection, if it's not open already
                    }

                    // SQL query
                    string sql = "DELETE FROM artists " +
                                 "WHERE artist_name=@name AND year=@year";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        // add the data to the command
                        command.Parameters.AddWithValue("@name", artistInfo.name);
                        command.Parameters.AddWithValue("@year", artistInfo.year);

                        command.ExecuteNonQuery();  // execute the SQL query
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            artistInfo.name = "";
            artistInfo.year = "";
            successMessage = "Artist deleted";

            Response.Redirect("/Artists/Index");  // redirect to arists landing page
        }
    }
}

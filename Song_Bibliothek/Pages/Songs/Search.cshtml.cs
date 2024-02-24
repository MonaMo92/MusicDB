using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;

namespace Song_Bibliothek.Pages.Songs
{
    public class SearchModel : PageModel
    {
        public SongInfo songInfo = new SongInfo();
        public string errorMessage = "";
        public string successMessage = "";
        private string connectionString = "server=localhost;uid=root;pwd=root;database=musicdb";    // data source
        public void OnGet()
        {
            string id = Request.Query["song_id"];

            try
            {                
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    if (connection.State == System.Data.ConnectionState.Closed)
                    {
                        connection.Open();  // open SQL connection, if it's not open already
                    }

                    // SQL query
                    string sql = "SELECT * FROM songs";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // add the data to their objects
                                SongInfo songInfo = new SongInfo();
                                songInfo.id = "" + reader.GetInt32(0);
                                songInfo.title = reader.GetString(1);
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
            // save the data entered by the user
            songInfo.id = Request.Query["id"];
            songInfo.title = Request.Form["title"];

            // check whether all fields have content
            if (songInfo.title.Length == 0)
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
                        connection.Open();  // open SQL connection, if it's not open already
                    }

                    // SQL query
                    string sql = "SELECT * FROM songs " +
                        "WHERE song_title=@title";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        // add the data to the command
                        command.Parameters.AddWithValue("@title", songInfo.title);

                        command.ExecuteNonQuery();  // execute the SQL query
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            Response.Redirect("/Songs/SearchResult");  // redirect to songs search result page
        }
    }
}

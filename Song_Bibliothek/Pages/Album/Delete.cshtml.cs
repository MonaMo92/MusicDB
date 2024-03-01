using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace Song_Bibliothek.Pages.Album
{
    public class DeleteModel : PageModel
    {
        public AlbumInfo albumInfo = new AlbumInfo();
        public string errorMessage = "";
        public string successMessage = "";
        private string connectionString = "server=host.docker.internal;uid=root;pwd=root;database=musicdb";    // data source

        public void OnGet()
        {
            string id = Request.Query["album_id"];

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    if (connection.State == System.Data.ConnectionState.Closed)
                    {
                        connection.Open();  // open SQL connection, if it's not open already
                    }

                    // SQL query
                    string sql = "SELECT * FROM album";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // add the data to their objects
                                AlbumInfo albumInfo = new AlbumInfo();
                                albumInfo.id = "" + reader.GetInt32(0);
                                albumInfo.artist = reader.GetString(1);
                                albumInfo.title = reader.GetString(2);
                                albumInfo.year = reader.GetString(3);
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
                albumInfo.id = Request.Form["id"];
                albumInfo.artist = Request.Form["artist"];
                albumInfo.title = Request.Form["title"];
                albumInfo.year = Request.Form["year"];

                // check whether all fields have content
                if (albumInfo.artist.Length == 0 || albumInfo.title.Length == 0 || albumInfo.year.Length == 0)
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
                    string sql = "DELETE FROM album " +
                        "WHERE artist_id = (SELECT artist_id FROM artists WHERE artist_name = @artist) " +
                        "AND album_title = @title " +
                        "AND year = @year";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        // add the data to the command
                        command.Parameters.AddWithValue("@artist", albumInfo.artist);
                        command.Parameters.AddWithValue("@title", albumInfo.title);
                        command.Parameters.AddWithValue("@year", Convert.ToInt32(albumInfo.year));
                        command.Parameters.AddWithValue("@label", albumInfo.label);

                        command.ExecuteNonQuery();  // execute the SQL query
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            albumInfo.title = "";
            albumInfo.title = "";
            albumInfo.artist = "";
            successMessage = "Album deleted";

            Response.Redirect("/Album/Index");  // redirect to album landing page
        }
    }
}

using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace Song_Bibliothek.Pages.Album
{
    public class CreateModel : PageModel
    {
        public AlbumInfo albumInfo = new AlbumInfo();
        public string errorMessage = "";
        public string successMessage = "";

        public void OnGet()
        {
        }

        public void OnPost() 
        {
            albumInfo.artist = Request.Form["artist"];
            albumInfo.title = Request.Form["title"];
            albumInfo.year = Request.Form["year"];
            albumInfo.label = Request.Form["label"];

            if (albumInfo.artist.Length == 0 || albumInfo.title.Length == 0
                || albumInfo.year.Length == 0 || albumInfo.label.Length == 0)
            {
                errorMessage = "Please enter all of the fields";
                return;
            }

            // save the data in the database
            try
            {
                string connectionString = "server=host.docker.internal;uid=root;pwd=root;database=musicdb";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    if (connection.State == System.Data.ConnectionState.Closed)
                    {
                        connection.Open();
                    }

                    // SQL query
                    string sql = "INSERT INTO album (artist_id, album_title, year, label) " +
                        "SELECT artist_id, @title, @year, @label " +
                        "FROM artists " +
                        "WHERE artist_name = @artist";

                    
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@artist", albumInfo.artist);
                        command.Parameters.AddWithValue("@title", albumInfo.title);
                        command.Parameters.AddWithValue("@year", albumInfo.year);
                        command.Parameters.AddWithValue("@label", albumInfo.label);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch(Exception ex)
            { 
                errorMessage = ex.Message;
                return;
            }

            albumInfo.artist = "";
            albumInfo.title = "";
            albumInfo.year = "";
            albumInfo.label = "";
            successMessage = "New Album added";

            Response.Redirect("/Album/Index");
        }
    }
}

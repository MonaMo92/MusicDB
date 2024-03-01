using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace Song_Bibliothek.Pages.Songs
{
    public class DeleteModel : PageModel
    {
        public SongInfo songInfo = new SongInfo();
        public string errorMessage = "";
        public string successMessage = "";
        private string connectionString = "server=host.docker.internal;uid=root;pwd=root;database=musicdb";

        public void OnGet()
        {
            string id = Request.Query["song_id"];

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    if (connection.State == System.Data.ConnectionState.Closed)
                    {
                        connection.Open();
                    }

                    string sql = "SELECT * FROM songs";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                SongInfo songInfo = new SongInfo();
                                songInfo.id = "" + reader.GetInt32(0);
                                songInfo.album = reader.GetString(1);
                                songInfo.title = reader.GetString(2);
                                songInfo.track = reader.GetString(3);
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
                songInfo.id = Request.Query["id"];
                songInfo.album = Request.Form["album"];
                songInfo.title = Request.Form["title"];
                songInfo.track = Request.Form["track"];

                if (songInfo.title.Length == 0 || songInfo.album.Length == 0 || songInfo.track.Length == 0)
                {
                    errorMessage = "Please enter all of the fields";
                    return;
                }

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    if (connection.State == System.Data.ConnectionState.Closed)
                    {
                        connection.Open();
                    }

                    string sql = "DELETE FROM songs " +
                                 "WHERE album_id=(SELECT album_id FROM album WHERE album_title = @album) " +
                                 "AND song_title=@title " +
                                 "AND track=@track"; 

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@album", songInfo.album);
                        command.Parameters.AddWithValue("@title", songInfo.title);
                        command.Parameters.AddWithValue("@track", Convert.ToInt32(songInfo.track));

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            songInfo.album = "";
            songInfo.title = "";
            songInfo.track = "";
            successMessage = "Song deleted";

            Response.Redirect("/Songs/Index");
        }
    }
}

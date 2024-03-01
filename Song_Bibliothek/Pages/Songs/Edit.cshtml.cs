using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace Song_Bibliothek.Pages.Songs
{
    public class EditModel : PageModel
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
                                songInfo.lyrics = reader.GetString(4);
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
            songInfo.id = Request.Query["id"];
            songInfo.album = Request.Form["album"];
            songInfo.title = Request.Form["title"];
            songInfo.track = Request.Form["track"];
            songInfo.lyrics = Request.Form["lyrics"];

            if (songInfo.album.Length == 0 || songInfo.title.Length == 0
                || songInfo.track.Length == 0 || songInfo.lyrics.Length == 0)
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

                    string sql = "UPDATE songs " +
                        "SET song_title=@title, album_id=(SELECT album_id FROM album WHERE album_title = @album), lyrics=@lyrics, track=@track " +
                        "WHERE song_title=@title AND album_id=(SELECT album_id FROM album WHERE album_title = @album)";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@album", songInfo.album);
                        command.Parameters.AddWithValue("@title", songInfo.title);
                        command.Parameters.AddWithValue("@track", Convert.ToInt32(songInfo.track));
                        command.Parameters.AddWithValue("@lyrics", songInfo.lyrics);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            Response.Redirect("/Songs/Index");
        }
    }
}

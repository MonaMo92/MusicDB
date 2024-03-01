using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace Song_Bibliothek.Pages.Songs
{
    public class CreateModel : PageModel
    {
        public SongInfo songInfo = new SongInfo();
        public string errorMessage = "";
        public string successMessage = "";
        
        public void OnGet()
        {
        }

        public void OnPost() 
        {
            songInfo.title = Request.Form["title"];
            songInfo.artist = Request.Form["artist"];
            songInfo.album = Request.Form["album"];
            songInfo.track = Request.Form["track"];
            songInfo.year = Request.Form["year"];
            songInfo.lyrics = Request.Form["lyrics"];

            if (songInfo.title.Length == 0 || songInfo.artist.Length == 0 || songInfo.lyrics.Length == 0
                || songInfo.album.Length == 0 || songInfo.track.Length == 0 || songInfo.year.Length == 0)
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

                    string sql = "INSERT INTO songs (album_id, song_title, track, lyrics)" +
                        "VALUES(@album, @title, @track, @lyrics);";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@title", songInfo.title);
                        command.Parameters.AddWithValue("@artist", songInfo.artist);
                        command.Parameters.AddWithValue("@album", songInfo.album);
                        command.Parameters.AddWithValue("@track", songInfo.track);
                        command.Parameters.AddWithValue("@year", songInfo.year);
                        command.Parameters.AddWithValue("@lyrics", songInfo.lyrics);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch(Exception ex)
            { 
                errorMessage = ex.Message;
                return;
            }

            songInfo.title = "";
            songInfo.artist = "";
            songInfo.album = "";
            songInfo.track = "";
            songInfo.year = "";
            songInfo.lyrics = "";
            successMessage = "New Song added";

            Response.Redirect("/Songs/Index");
        }
    }
}

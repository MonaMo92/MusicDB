using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using Song_Bibliothek.Pages.Album;
using Song_Bibliothek.Pages.Artists;

namespace Song_Bibliothek.Pages.Songs
{
    public class CreateModel : PageModel
    {
        private ID3Service? _id3Service;
        public SongInfo songInfo = new SongInfo();
        public string errorMessage = "";
        public string successMessage = "";
        
        public void OnGet()
        {
        }

        public void OnPost()
        {
            songInfo.lyrics = Request.Form["lyrics"];

            _id3Service = new ID3Service();

            _id3Service.GetMetaDataSong(Request, songInfo);

            try
            {
                string connectionString = "server=host.docker.internal;uid=root;pwd=root;database=musicdb";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    if (connection.State == System.Data.ConnectionState.Closed)
                    {
                        connection.Open();
                    }

                    string sql = "INSERT INTO songs (album_id, song_title, track, lyrics, file_format, data)" +
                        "VALUES((SELECT album_id FROM album WHERE album_title = '" + songInfo.album + "'), '" +
                        songInfo.title + "', '" + songInfo.track + "', '" +
                        songInfo.lyrics + "', '" + songInfo.fileFormat + "', @data)";

                    int songCount = 0;
                    string stmt = "SELECT COUNT(*) FROM songs Where song_title = '" + songInfo.title + "'";

                    using (MySqlCommand cmd = new MySqlCommand(stmt, connection))
                    {
                        songCount = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    if (songCount > 0)
                    {
                        successMessage = "Song already in List";

                        Thread.Sleep(1000);
                        return;
                    }

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@title", songInfo.title);
                        command.Parameters.AddWithValue("@artist", songInfo.artist);
                        command.Parameters.AddWithValue("@album", songInfo.album);
                        command.Parameters.AddWithValue("@track", songInfo.track);
                        command.Parameters.AddWithValue("@year", songInfo.year);
                        command.Parameters.AddWithValue("@lyrics", songInfo.lyrics);
                        command.Parameters.AddWithValue("@file_format", songInfo.fileFormat);
                        command.Parameters.AddWithValue("@data", songInfo.data);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
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

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace Song_Bibliothek.Pages.Songs
{
    public class CreateModel : PageModel, ID3Service
    {

        public SongInfo songInfo = new SongInfo();
        public string errorMessage = "";
        public string successMessage = "";
        
        public void OnGet()
        {
        }

        public void OnPost() 
        {
            // save the data entered by the user
            songInfo.fileFormat = Request.Form["file"];
            songInfo.title = Request.Form["title"];
            songInfo.artist = Request.Form["artist"];
            songInfo.album = Request.Form["album"];
            songInfo.track = Request.Form["track"];
            songInfo.year = Request.Form["year"];
            songInfo.lyrics = Request.Form["lyrics"];
            
            GetMetaData(songInfo.fileFormat);

            // check whether all fields have content
            if (songInfo.title.Length == 0 || songInfo.artist.Length == 0 || songInfo.lyrics.Length == 0
                || songInfo.album.Length == 0 || songInfo.track.Length == 0 || songInfo.year.Length == 0)
            {
                errorMessage = "Please enter all of the fields";
                return;
            }

            // save the data in the database
            try
            {
                string connectionString = "server=localhost;uid=root;pwd=root;database=musicdb";    // data source

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    if (connection.State == System.Data.ConnectionState.Closed)
                    {
                        connection.Open();  // open SQL connection, if it's not open already
                    }

                    // SQL query
                    string sql = "INSERT INTO songs (album_id, song_title, track, lyrics, file_format, data)" +
                        "VALUES(@album, @title, @track, @lyrics, @file_format, @data);";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        // add the data to the command
                        command.Parameters.AddWithValue("@title", songInfo.title);
                        command.Parameters.AddWithValue("@artist", songInfo.artist);
                        command.Parameters.AddWithValue("@album", songInfo.album);
                        command.Parameters.AddWithValue("@track", songInfo.track);
                        command.Parameters.AddWithValue("@year", songInfo.year);
                        command.Parameters.AddWithValue("@lyrics", songInfo.lyrics);
                        command.Parameters.AddWithValue("@file_format", songInfo.fileFormat);
                        command.Parameters.AddWithValue("@data", songInfo.data);

                        command.ExecuteNonQuery();  // execute the SQL query
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

            Response.Redirect("/Songs/Index");  // redirect to songs landing page
        }

        public void GetMetaData(string file)
        {
            var mp3File = Request.Form.Files["file"];

            string filePath = "";

            if (mp3File == null)
            {
                NotFound();
                return;
            }

            filePath = Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "AudioFiles"), mp3File.FileName);

            songInfo.fileFormat = filePath;

            try
            {

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    mp3File.CopyTo(fileStream);
                }

                string connectionString = "server=localhost;uid=root;pwd=root;database=musicdb";    // data source

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    if (connection.State == System.Data.ConnectionState.Closed)
                    {
                        connection.Open();  // open SQL connection, if it's not open already
                    }

                    if (mp3File == null || mp3File.Length == 0)
                        return;


                    FileStream fileStream = new FileStream(filePath, FileMode.Open);

                    songInfo.data = new byte[fileStream.Length];

                    fileStream.Read(songInfo.data, 0, (int)fileStream.Length);

                }
            }
            catch (Exception ex)
            {
                NotFound(ex.Message);
            }

        }
    }
}

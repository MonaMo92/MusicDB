using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using Song_Bibliothek.Pages.Album;
using Song_Bibliothek.Pages.Artists;

namespace Song_Bibliothek.Pages.Songs
{
    public class CreateModel : PageModel, ID3Service
    {

        public SongInfo songInfo = new SongInfo();
        public ArtistInfo artistInfo = new ArtistInfo();
        public AlbumInfo albumInfo = new AlbumInfo();
        public string errorMessage = "";
        public string successMessage = "";
        
        public void OnGet()
        {
        }

        public void OnPost() 
        {
            // save the data entered by the user
            songInfo.lyrics = Request.Form["lyrics"];
            
            GetMetaData();

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

                    int songCount = 0;
                    string stmt = "SELECT COUNT(*) FROM songs Where song_title = '" + songInfo.title + "'"; 

                    using (MySqlCommand cmd = new MySqlCommand(stmt, connection))
                    {
                        songCount = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    if (songCount >= 0)
                    {
                        successMessage = "Song already in List";

                        Thread.Sleep(1000);

                        //Response.Redirect("/Songs/Index");  // redirect to songs landing page

                        return;
                    }

                    // SQL query
                    string sql = "INSERT INTO songs (album_id, song_title, track, lyrics, file_format, data)" +
                        "VALUES((SELECT album_id FROM album WHERE album_title = '" + songInfo.album + "'), '" + 
                        songInfo.title + "', '" + songInfo.track + "', '" +
                        songInfo.lyrics + "', '" + songInfo.fileFormat + "', @data)";

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

        public void GetMetaData()
        {
            //Recieve request
            var mp3File = Request.Form.Files["file"];

            string filePath = "";

            if (mp3File == null)
            {
                NotFound();
                return;
            }

            //Create file for server to save
            filePath = Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "AudioFiles"), mp3File.FileName);

            songInfo.fileFormat = filePath;


            //Get Metadata from file
            var metaData = TagLib.File.Create(filePath);

            songInfo.title = metaData.Tag.Title ?? "Unknown";
            songInfo.artist = metaData.Tag.AlbumArtists.FirstOrDefault("Unknown");
            songInfo.album = metaData.Tag.Album ?? "Unknown";
            songInfo.year = metaData.Tag.Year.ToString() ?? "Unknown";
            songInfo.track = metaData.Tag.Track.ToString() ?? "Unknown";

            string replaceTitle = songInfo.title.Replace("'", "�");
            string replaceAlbum = songInfo.album.Replace("'", "�");
            string replaceLyrics = songInfo.lyrics.Replace("'", "�");
            string replaceArtist = songInfo.artist.Replace("'", "�");
            string replaceFileFormat = songInfo.fileFormat.Replace("'", "�");

            songInfo.title = replaceTitle;
            songInfo.artist = replaceArtist;
            songInfo.album = replaceAlbum;
            songInfo.lyrics = replaceLyrics;
            songInfo.fileFormat = replaceFileFormat;

            //artistInfo.name = metaData.Tag.AlbumArtists.FirstOrDefault();
            //artistInfo.title = metaData.Tag.Title;
            //artistInfo.genre = metaData.Tag.Genres.FirstOrDefault();
            //artistInfo.year = metaData.Tag.Year.ToString();
            //artistInfo.origin = "Unknown";
            //albumInfo.title = metaData.Tag.Album;
            //albumInfo.year = metaData.Tag.Year.ToString();
            //albumInfo.artist = metaData.Tag.AlbumArtists.FirstOrDefault();
            //albumInfo.label = metaData.Tag.Publisher;


            try
            {
                //Copy file for application to use
                using (var fileStreamCreate = new FileStream(songInfo.fileFormat, FileMode.Create))
                {
                    mp3File.CopyTo(fileStreamCreate);
                }

                if (mp3File == null || mp3File.Length == 0)
                    return;

                //Convert to byte to save in database
                var fileStreamOpen = new FileStream(songInfo.fileFormat, FileMode.Open);

                songInfo.data = new byte[fileStreamOpen.Length];

                fileStreamOpen.Read(songInfo.data, 0, (int)fileStreamOpen.Length);

            }
            catch (Exception ex)
            {
                NotFound(ex.Message);
            }

        }
    }
}

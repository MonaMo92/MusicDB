using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Media;
using System.Reflection.PortableExecutable;

namespace Song_Bibliothek.Pages.Songs
{
    public class SearchResultModel : PageModel
    {
        public List<SongInfo> SongList = new List<SongInfo>();
        public string title;
        public SoundPlayer sound;
        public Dictionary<string, string> songs;

        public void OnGet()
        {
            try
            {
                string connectionString = "server=host.docker.internal;uid=root;pwd=root;database=musicdb";
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    if (connection.State == System.Data.ConnectionState.Closed)
                    {
                        connection.Open();
                    }

                    title = Request.Query["title"];
                    string sql = "";
                    if (!string.IsNullOrEmpty(title))
                    {
                        sql = "SELECT * FROM songs " +
                        "JOIN album ON album.album_id = songs.album_id " +
                        "JOIN artists ON album.artist_id = artists.artist_id " +
                        "WHERE song_title=@title";
                    }

                    songs = new Dictionary<string, string>
                    {
                        { "Song 1", "~/Songs/American Idiot.mp3" },
                        { "Song 2", "~/Songs/Chop Suey.mp3" },
                        { "Song 3", "~/Songs/Fly Away.mp3" },
                        { "Song 4", "~/Songs/Hit The Floor.mp3" },
                        { "Song 5", "~/Songs/I Dont Like Metal.mp3" },
                        { "Song 6", "~/Songs/Lost In Hollywood.mp3" },
                        { "Song 7", "~/Songs/Radio Video.mp3" },
                        { "Song 8", "~/Songs/Runaway.mp3" },
                        { "Song 9", "~/Songs/Tears Dont Fall.mp3" },
                        { "The Kill", "~/Songs/The Kill.mp3" },
                        { "Song 11", "~/Songs/Wessi Girl.mp3" }
                    };

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@title", title);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                SongInfo songInfo = new SongInfo();
                                songInfo.id = "" + reader.GetInt32(0);
                                songInfo.album = reader.GetString(7);
                                songInfo.title = reader.GetString(2);
                                songInfo.track = reader.GetString(3);
                                songInfo.artist = reader.GetString(11);
                                songInfo.lyrics = reader.GetString(4);
                                songInfo.year = reader.GetString(8);

                                SongList.Add(songInfo);
                                /*foreach(var song in songs)
                                {
                                    if(song.Key == songInfo.title)
                                        songTitle = new SoundPlayer(songInfo.title);
                                }*/
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
            }
            OnPostButtonClicked();
        }
        public IActionResult OnPostButtonClicked()
        {
            foreach(var song in songs)
            {
                if(song.Key == title)
                {

                }

            }
            //if(sound != null)
                //sound.Play();
            return Content("Play song");
        }
    }
}

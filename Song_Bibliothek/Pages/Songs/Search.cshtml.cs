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
            songInfo.id = Request.Query["id"];
            songInfo.title = Request.Form["title"];

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
                        connection.Open();
                    }

                    string sql = "SELECT song_title=@title FROM songs " +
                        "WHERE song_title=@title";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@title", songInfo.title);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            Response.Redirect("/Songs/SearchResult?title=" + songInfo.title);
        }
    }
}

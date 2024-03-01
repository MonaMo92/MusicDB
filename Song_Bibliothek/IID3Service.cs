using Song_Bibliothek.Pages.Songs;

namespace Song_Bibliothek
{
    public interface IID3Service
    {
        SongInfo GetMetaDataSong(HttpRequest request, SongInfo songInfo);
    }
}

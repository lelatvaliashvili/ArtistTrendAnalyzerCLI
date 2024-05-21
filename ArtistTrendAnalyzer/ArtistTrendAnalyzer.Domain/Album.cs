namespace ArtistTrendAnalyzer.Domain.Entities;

public class Album
{
    public string Name { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
    public int TotalTracks { get; set; }
    public List<string> Genres { get; set; } = new List<string>();
}

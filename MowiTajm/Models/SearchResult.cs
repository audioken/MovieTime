namespace MowiTajm.Models
{
    public class SearchResult
    {
        public List<MovieLite> Search { get; set; } = new List<MovieLite>();
        public string TotalResults { get; set; } = "";
        public string Response { get; set; } = "";
    }
}

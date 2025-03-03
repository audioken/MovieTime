namespace MowiTajm.Models
{
    public class SearchResult
    {
        public List<MovieLite> Search { get; set; }
        public string TotalResults { get; set; } = "";
        public string Response { get; set; } = "";
    }
}

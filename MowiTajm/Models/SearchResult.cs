namespace MowiTajm.Models
{
    public class SearchResult
    {
        public List<MovieLite> Movies { get; set; }
        public string TotalResults { get; set; } = "";
        public string Response { get; set; } = "";
    }
}

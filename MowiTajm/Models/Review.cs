using System.ComponentModel.DataAnnotations;

namespace MowiTajm.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public string ImdbID { get; set; } = "";

        [Required]
        public string Username { get; set; } = "";

        [Required]
        public string MovieTitle { get; set; } = "";

        [Required]
        public string Title { get; set; } = "";

        [Required]
        public string Text { get; set; } = "";

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; } = 1;

        [Required]
        public DateTime DateTime { get; set; } = DateTime.Now;
    }
}

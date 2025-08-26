namespace Trabajo_API_NET.Models
{
    public class Snippet
    {
        public string Id { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string Language { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}

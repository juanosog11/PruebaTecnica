namespace Back.Domain.Models
{
    public class Post
    {
        public int PostId { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }

        // Foreign Key
        public int UsuarioId { get; set; }
    }
}

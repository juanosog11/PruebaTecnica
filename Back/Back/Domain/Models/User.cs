using System.Collections.Generic;

namespace Back.Domain.Models
{
    public class User
    {
        public int UsuarioId { get; set; }
        public string Usuario { get; set; }
        public string Email { get; set; }
        public string Numero { get; set; }
        public string Clave { get; set; }

        public ICollection<User> Followers { get; set; } = new List<User>();
        public ICollection<User> Followees { get; set; } = new List<User>();
        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetworkConsoleApp.Core.Models
{
    public class UsuarioConsole
    {
        public string Usuarioname { get; set; }
        public List<PostConsole> Posts { get; set; }
        public List<UsuarioConsole> Following { get; set; }

        public UsuarioConsole(string usuarioname)
        {
            Usuarioname = usuarioname;
            Posts = new List<PostConsole>();
            Following = new List<UsuarioConsole>();
        }

        public void AddPost(string content, DateTime timestamp)
        {
            Posts.Add(new PostConsole(content, this, timestamp));
        }

        public void Follow(UsuarioConsole usuario)
        {
            if (!Following.Contains(usuario))
            {
                Following.Add(usuario);
            }
        }
    }
}

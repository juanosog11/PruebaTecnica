using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetworkConsoleApp.Core.Models
{
    public class PostConsole
    {
        public string Content { get; }
        public UsuarioConsole Author { get; }
        public DateTime Timestamp { get; }

        public PostConsole(string content, UsuarioConsole author, DateTime timestamp)
        {
            Content = content;
            Author = author;
            Timestamp = timestamp;
        }

        public override string ToString()
        {
            return $"{Content} @{Author.Usuarioname} @{Timestamp:HH:mm}";
        }
    }
}

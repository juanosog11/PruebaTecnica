using SocialNetworkConsoleApp.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SocialNetworkConsoleApp.Core.Services
{
    public class SocialNetworkConsole
    {
        private Dictionary<string, UsuarioConsole> Usuarios;

        public SocialNetworkConsole()
        {
            Usuarios = new Dictionary<string, UsuarioConsole>();
        }

        public UsuarioConsole GetUsuario(string Usuarioname)
        {
            if (!Usuarios.ContainsKey(Usuarioname))
            {
                Usuarios[Usuarioname] = new UsuarioConsole(Usuarioname);
            }
            return Usuarios[Usuarioname];
        }

        public void PostMessage(string Usuarioname, string content, DateTime timestamp)
        {
            var Usuario = GetUsuario(Usuarioname);
            Usuario.AddPost(content, timestamp);
            Console.WriteLine($"{Usuarioname} posted -> \"{content}\" @{timestamp:HH:mm}");
        }

        public void Follow(string follower, string followee)
        {
            var followerUsuario = GetUsuario(follower);
            var followeeUsuario = GetUsuario(followee);

            if (followerUsuario == followeeUsuario)
            {
                Console.WriteLine($"{follower} cannot follow themselves.");
            }
            else
            {
                followerUsuario.Follow(followeeUsuario);
                Console.WriteLine($"{follower} empezó a seguir a {followee}");
            }
        }

        public void ShowDashboard(string Usuarioname)
        {
            var Usuario = GetUsuario(Usuarioname);
            var posts = Usuario.Following
                            .SelectMany(f => f.Posts)
                            .OrderBy(p => p.Timestamp)
                            .ToList();

            foreach (var post in posts)
            {
                Console.WriteLine(post);
            }
        }
    }
}

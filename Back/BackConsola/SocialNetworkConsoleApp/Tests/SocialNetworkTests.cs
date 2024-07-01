using Xunit;
using SocialNetworkConsoleApp.Core.Models;
using SocialNetworkConsoleApp.Core.Services;

namespace SocialNetworkConsoleApp.Tests
{
    public class SocialNetworkTests
    {
        private readonly SocialNetworkConsole _network;

        public SocialNetworkTests()
        {
            _network = new SocialNetworkConsole();
        }

        [Fact]
        public void UsuarioCanPostMessage()
        {
            _network.PostMessage("Alfonso", "Hola Mundo", DateTime.Now);
            var usuario = _network.GetUsuario("Alfonso");

            Assert.Single(usuario.Posts);
            Assert.Equal("Hola Mundo", usuario.Posts[0].Content);
        }

        [Fact]
        public void UsuarioCanFollowAnotherUsuario()
        {
            _network.Follow("Alicia", "Ivan");
            var alicia = _network.GetUsuario("Alicia");
            var ivan = _network.GetUsuario("Ivan");

            Assert.Contains(ivan, alicia.Following);
        }

        [Fact]
        public void DashboardShowsPostsFromFollowing()
        {
            var timestamp1 = DateTime.Now.AddMinutes(-10);
            var timestamp2 = DateTime.Now.AddMinutes(-5);
            _network.PostMessage("Alfonso", "Hola Mundo", timestamp1);
            _network.PostMessage("Ivan", "Hoy puede ser un gran día", timestamp2);
            _network.Follow("Alicia", "Alfonso");
            _network.Follow("Alicia", "Ivan");

            var output = new StringWriter();
            Console.SetOut(output);
            _network.ShowDashboard("Alicia");
            var result = output.ToString().Trim();

            Assert.Contains("Hola Mundo @Alfonso", result);
            Assert.Contains("Hoy puede ser un gran día @Ivan", result);
        }


    }
}
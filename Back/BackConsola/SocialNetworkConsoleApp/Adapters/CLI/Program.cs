using System;
using SocialNetworkConsoleApp.Core.Services;

class Program
{
    static void Main()
    {
        var network = new SocialNetworkConsole();

        while (true)
        {
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) continue;

            var parts = input.Split(' ', 3);
            var command = parts[0];
            var username = parts[1].TrimStart('@');

            switch (command.ToLower())
            {
                case "post":
                    var postContent = parts[2];
                    var timestamp = DateTime.Now;
                    network.PostMessage(username, postContent, timestamp);
                    break;

                case "follow":
                    var followee = parts[2].TrimStart('@');
                    network.Follow(username, followee);
                    break;

                case "wall":
                    Console.WriteLine(username);
                    network.ShowDashboard(username);
                    break;

                default:
                    Console.WriteLine("Comando no reconocido.");
                    break;
            }
        }
    }
}

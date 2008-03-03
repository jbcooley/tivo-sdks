using System;
using System.Collections.Generic;
using System.Text;
using Tivo.Hme;
using System.Drawing;
using Tivo.Hme.Host;
using System.Threading;

namespace Tivo.Hme.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            List<HmeServer> servers = new List<HmeServer>();

            servers.Add(new HmeServer<Animate>("Animate", new Uri("http://localhost:9688/Animate/")));
            servers.Add(new HmeServer<Clock>("Clock", new Uri("http://localhost:9688/Clock/")));
            servers.Add(new HmeServer<Effects>("Effects", new Uri("http://localhost:9688/Effects/")));
            servers.Add(new HmeServer<FontInfo>("Font Info", new Uri("http://localhost:9688/FontInfo/")));
            servers.Add(new HmeServer<Fractal>("Fractal", new Uri("http://localhost:9688/Fractal/")));
            servers.Add(new HmeServer<Pictures>("Pictures", new Uri("http://localhost:9688/Pictures/")));
            servers.Add(new HmeServer<TicTacToe>("Tic Tac Toe", new Uri("http://localhost:9688/TicTacToe/")));
            servers.Add(new HmeServer<Music>("Music", new Uri("http://localhost:9688/Music/")));
            HmeServer helloWorld = new HmeServer("Hello World", new Uri("http://localhost:9688/HelloWorld/"));
            helloWorld.ApplicationConnected += new EventHandler<HmeApplicationConnectedEventArgs>(server_ApplicationConnected);
            servers.Add(helloWorld);

            servers.ForEach(delegate(HmeServer server)
            {
                server.Start();
            });

            Console.WriteLine("Sample applications started.  Press enter to exit.");
            Console.ReadLine();
        }

        static void server_ApplicationConnected(object sender, HmeApplicationConnectedEventArgs e)
        {
            TextStyle font = new TextStyle("default", System.Drawing.FontStyle.Bold, 36);
            e.Application.Root = new TextView("Hello World", font, System.Drawing.Color.Aquamarine);
            e.Application.KeyPress += new EventHandler<KeyEventArgs>(Application_KeyPress);
        }

        static void Application_KeyPress(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == KeyCode.Left)
            {
                ((Application)sender).Close();
            }
        }
    }
}

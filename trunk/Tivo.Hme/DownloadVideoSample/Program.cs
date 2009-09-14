using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tivo.Hmo;

namespace DownloadVideoSample
{
    class Program
    {
        /// <summary>
        /// Name of tivo.  See Tivo Desktop or My Account on tivo.com to see name of tivo.
        /// </summary>
        static string tivoName = "DVR A9F1";
        /// <summary>
        /// Media access key.
        /// </summary>
        static string mak = "5314672594";
        static void Main(string[] args)
        {
            string videoName = null;
            if (args.Length != 0)
                videoName = args[0];

            DiscoveryBeacon.Start();
            string hmoServer = DiscoveryBeacon.GetServer(tivoName, TimeSpan.FromSeconds(65));

            using (TivoConnection connection = new TivoConnection(hmoServer, mak))
            {
                connection.Open();
                var query = connection.CreateContainerQuery("/NowPlaying")
                    .Recurse();
                var container = query.Execute();

                if(videoName == null) // no name, so just download the first video that can be downloaded
                {
                    var video = (from v in container.TivoItems.OfType<TivoVideo>()
                                 where v.CustomIcon == null || v.CustomIcon.Uri.AbsoluteUri != "urn:tivo:image:in-progress-recording"
                                 select v).First();
                    connection.GetDownloader(video).DownloadFile("downloaded.tivo");
                }
                else
                {
                    ContentDownloader downloader = null;
                    while(downloader == null)
                    {
                        var namedVideos = from video in container.TivoItems.OfType<TivoVideo>()
                                          where video.Name == videoName
                                          select video;
                        if (namedVideos.Any())
                        {
                            downloader = connection.GetDownloader(namedVideos.First());
                        }
                        else
                        {
                            query = query.Skip(container.ItemStart + container.ItemCount);
                            container = query.Execute();
                        }
                    }
                    downloader.DownloadFile("downloaded.tivo");
                }
            }
        }
    }
}

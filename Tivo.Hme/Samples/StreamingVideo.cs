using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Tivo.Hme.Samples
{
    class StreamingVideo : HmeApplicationHandler
    {
        private static int _initialized;

        private static void InitializeVideoHandler(IServiceProvider hostServices)
        {
            // if not initialized - set = 1
            // if equal to 1, then already initialized
            if (System.Threading.Interlocked.Exchange(ref _initialized, 1) != 1)
            {
                var registry = (Tivo.Hme.Host.Services.IHttpHandlerRegistryService)hostServices.GetService(typeof(Tivo.Hme.Host.Services.IHttpHandlerRegistryService));
                string registeredUri = "tivoStartup.mp4";
                registry.RegisterHandler(registeredUri, VideoRequestHandler);
            }
        }

        private static void VideoRequestHandler(Tivo.Hme.Host.Services.HttpHandlerArgs args)
        {
            var stream = args.Response.GetResponseStream(200, null);
            byte[] trackBytes = System.IO.File.ReadAllBytes("tivoStartup.mp4");
            stream.Write(trackBytes, 0, trackBytes.Length);
            stream.Close();
        }

        private IServiceProvider _hostServices;
        private StreamedVideoView _videoView;

        public override void OnApplicationStart(HmeApplicationStartArgs e)
        {
            _hostServices = e.HostServices;
            InitializeVideoHandler(_hostServices);
            _videoView = new StreamedVideoView();
            e.Application.Root = _videoView;
            Uri uri = new Uri(BaseUri, "tivoStartup.mp4");
            _videoView.Play(uri, "video/mp4");
            e.Application.KeyPress += new EventHandler<KeyEventArgs>(Application_KeyPress);
            e.Application.ResourceStateChanged += new EventHandler<ResourceStateChangedArgs>(Application_ResourceStateChanged);
        }

        void Application_ResourceStateChanged(object sender, ResourceStateChangedArgs e)
        {
            if (e.Resource == _videoView && e.Status == ResourceStatus.Complete)
                _videoView.Stop();
            if (e.Resource == _videoView && e.Status > ResourceStatus.Error)
            {
                int x = 0;
            }
        }

        int _speedIndex = 3;
        float[] _speeds = { -60.0f, -18.0f, -3.0f, 1.0f, 3.0f, 18.0f, 60.0f };
        void Application_KeyPress(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case KeyCode.Play:
                    _videoView.Play();
                    break;
                case KeyCode.Pause:
                    _videoView.Pause();
                    break;
                case KeyCode.Forward:
                    ++_speedIndex;
                    if (_speedIndex == 7) _speedIndex = 3;
                    _videoView.Forward(_speeds[_speedIndex]);
                    break;
                case KeyCode.Reverse:
                    --_speedIndex;
                    if (_speedIndex == -1) _speedIndex = 3;
                    _videoView.Reverse(_speeds[_speedIndex]);
                    break;
            }
        }

        public override void OnApplicationEnd()
        {
        }
    }
}

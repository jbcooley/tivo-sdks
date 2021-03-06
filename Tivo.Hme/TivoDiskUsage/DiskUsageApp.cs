﻿// Copyright (c) 2008 Josh Cooley

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Text;
using Tivo.Hme;
using Tivo.Hmo;

namespace TivoDiskUsage
{
    [ApplicationIcon(typeof(Properties.Resources), "pieiconpng")]
    class DiskUsageApp : HmeApplicationHandler
    {
        private TivoConnection _connection;
        private TivoContainerQuery _query;
        private string _tivoName;
        private WaitingView _waitingView;

        static DiskUsageApp()
        {
            DiscoveryBeacon.Start();
        }

        public override void OnApplicationStart(HmeApplicationStartArgs e)
        {
            Application app = e.Application;
            app.KeyPress += new EventHandler<KeyEventArgs>(app_KeyPress);
            // get application responsive as quickly as possible
            // query disk usage once we have the host name
            app.DeviceConnected += new EventHandler<DeviceConnectedArgs>(app_DeviceConnected);
            app.SupportedResolutionsChanged += new EventHandler<EventArgs>(app_SupportedResolutionsChanged);

            _waitingView = new WaitingView();
            app.Root.Children.Add(_waitingView);
        }

        public override void OnApplicationEnd()
        {
        }

        void app_KeyPress(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == KeyCode.Left)
            {
                Application app = (Application)sender;
                app.Close();
            }
        }

        void app_DeviceConnected(object sender, DeviceConnectedArgs e)
        {
            // TODO: provide better feedback
            try
            {
                _tivoName = e.Host;
                string server = DiscoveryBeacon.GetServer(_tivoName, TimeSpan.FromMinutes(1));
                GetNowPlaying(server, Properties.Settings.Default.MediaAccessKey, (Application)sender);
            }
            catch (TimeoutException)
            {
                _waitingView.DisplayFailure("Unable to connect connect back to tivo. Please be sure tivo to go is enabled");
            }
        }

        void app_SupportedResolutionsChanged(object sender, EventArgs e)
        {
            Application app = (Application)sender;
            app.CurrentResolution = app.SupportedResolutions[0];
        }

        private void GetNowPlaying(string hmoServer, string mediaAccessKey, Application app)
        {
            _connection = new TivoConnection(hmoServer, mediaAccessKey);
            _connection.Open();
            _query = _connection.CreateContainerQuery("/NowPlaying").Recurse();
            _query.BeginExecute(QueryUsage, app);
            //_connection.BeginQueryContainer("/NowPlaying", true, QueryUsage, app);
        }

        private List<TivoContainer> _containers = new List<TivoContainer>();
        public void QueryUsage(IAsyncResult result)
        {
            Application app = (Application)result.AsyncState;

            TivoContainer container = _query.EndExecute(result);
            _containers.Add(container);

            if (container.ItemStart + container.ItemCount < container.TotalItems)
            {
                _query = _query.Skip(container.ItemStart + container.ItemCount);
                _query.BeginExecute(QueryUsage, app);
            }
            else
            {
                _connection.Dispose();
                CategoryPieView pieView = new CategoryPieView(DiskUsageCalculator.Calculate(_containers, _tivoName));
                View previousView = app.Root.Children[0];
                app.Root.Children.RemoveAt(0);
                app.Root.Children.Add(pieView);
                previousView.Dispose();
            }
        }
    }
}

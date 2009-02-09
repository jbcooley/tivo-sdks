using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tivo.Hme.Host.Http;
using Tivo.Hme.Host.Services;

namespace Tivo.Has
{
    sealed class HmeServer : Tivo.Hme.Host.HmeServer
    {
        private HmeApplicationPump _pump;
        private IHmeApplicationDriver _driver;
        private IHmeApplicationIdentity _identity;
        private string _webPath;

        public HmeServer(IHmeApplicationIdentity identity, IHmeApplicationDriver driver)
            : base(identity.Name, identity.EndPoint, Tivo.Hme.Host.HmeServerOptions.AdvertiseOnLocalNetwork, null)
        {
            _pump = new HmeApplicationPump(driver);
            _driver = driver;
            _identity = identity;
        }

        protected override void OnHmeApplicationRequestReceived(HttpListenerContext context)
        {
            HmeApplicationHttpResponse.BeginResponse(context);
            HmeStream hmeInStream = new HmeStream(context.Request.InputStream);
            HmeStream hmeOutStream = new HmeStream(context.Response.OutputStream);
            IHmeConnection connection = _driver.CreateHmeConnection(_identity, context.Request.Url.OriginalString, hmeInStream, hmeOutStream);
            _pump.AddHmeConnection(connection);
        }

        protected override void OnHmeApplicationIconRequested(Tivo.Hme.Host.HmeApplicationIconRequestedArgs e)
        {
            e.Icon = _identity.Icon;
            e.ContentType = "image/png";
            base.OnHmeApplicationIconRequested(e);
        }

        protected override void OnNonApplicationRequestReceived(Tivo.Hme.Host.HttpConnectionEventArgs e)
        {
            if (_identity.UsesHostHttpServices)
            {
                if (_webPath == null)
                    _webPath = _driver.GetWebPath(_identity);
                var host = ((IHttpApplicationHostPool)GetService(typeof(IHttpApplicationHostPool))).GetHost(_webPath);
                host.ProcessRequest(ApplicationPrefix, e.Context);
            }
            else
            {
                base.OnNonApplicationRequestReceived(e);
            }
        }
    }
}

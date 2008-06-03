using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tivo.Hme.Host.Http;

namespace Tivo.Has
{
    sealed class HmeServer : Tivo.Hme.Host.HmeServer
    {
        private HmeApplicationPump _pump;
        private IHmeApplicationDriver _driver;
        private IHmeApplicationIdentity _identity;

        public HmeServer(IHmeApplicationIdentity identity, IHmeApplicationDriver driver)
            : base(identity.Name, identity.EndPoint, Tivo.Hme.Host.HmeServerOptions.AdvertiseOnLocalNetwork, null)
        {
            _pump = new HmeApplicationPump(driver);
            _driver = driver;
            _identity = identity;
        }

        protected override void OnHmeApplicationRequestReceived(HttpRequestReceivedArgs e)
        {
            e.HttpRequest.WriteResponse(new HmeApplicationHttpResponse());
            HmeStream hmeStream = new HmeStream(e.HttpRequest.Stream);
            IHmeConnection connection = _driver.CreateHmeConnection(_identity, hmeStream, hmeStream);
            _pump.AddHmeConnection(connection);
        }

        protected override void OnNonApplicationRequestReceived(Tivo.Hme.Host.NonApplicationRequestReceivedArgs e)
        {
            byte[] icon = _identity.Icon;
            if (icon != null)
            {
                e.HttpResponse = new ApplicationIconHttpResponse(icon);
            }
            base.OnNonApplicationRequestReceived(e);
        }
    }
}

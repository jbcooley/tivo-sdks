using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tivo.Hme.Host.Services
{
    public interface IHttpApplicationHostPool
    {
        IHttpApplicationHost GetHost(string appPath);
        void Release(string appPath);
    }

    class HttpApplicationHostPool : IHttpApplicationHostPool
    {
        private Dictionary<string, IHttpApplicationHost> _applicationHostPool = new Dictionary<string, IHttpApplicationHost>();

        #region IHttpApplicationHostPool Members

        public IHttpApplicationHost GetHost(string appPath)
        {
            IHttpApplicationHost host;
            if (!_applicationHostPool.TryGetValue(appPath, out host))
            {
                host = new HttpApplicationHost(appPath);
                _applicationHostPool.Add(appPath, host);
            }
            return host;
        }

        public void Release(string appPath)
        {
            _applicationHostPool.Remove(appPath);
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.AddIn.Hosting;

namespace Tivo.Has
{
    class HmeServersController
    {
        private List<HmeServerAndData> _servers = new List<HmeServerAndData>();

        public void Add(HmeServer server, IHmeApplicationIdentity identity, string path)
        {
            _servers.Add(new HmeServerAndData { HmeServer = server, Identity = identity, ServerPath = path });
        }

        public void Remove(Predicate<HmeServerAndData> predicate)
        {
            _servers.RemoveAll(predicate);
        }

        public void Clear()
        {
            _servers.Clear();
        }

        public void StopAllServers()
        {
            _servers.ForEach(server => server.HmeServer.Stop());
        }

        public IEnumerable<HmeServerAndData> Servers { get { return _servers; } }

        public void LoadApplications(params string[] applicationDirectories)
        {
            LoadApplications(null, applicationDirectories);
        }

        public void LoadApplications(Action<HmeServer> serverAction, params string[] applicationDirectories)
        {
            string[] results = AddInStore.Rebuild(PipelineStoreLocation.ApplicationBase);
            string[] addinResults = AddInStore.RebuildAddIns(applicationDirectories[0]);
            var addInTokens = AddInStore.FindAddIns(typeof(IHmeApplicationDriver), PipelineStoreLocation.ApplicationBase,//);
                applicationDirectories);
            foreach (AddInToken token in addInTokens)
            {
                IHmeApplicationDriver driver = token.Activate<IHmeApplicationDriver>(AddInSecurityLevel.Host);
                Uri assemblyCodeBase = new Uri(driver.GetType().Assembly.GetName().CodeBase);
                System.IO.FileInfo assemblyFileInfo = new System.IO.FileInfo(assemblyCodeBase.LocalPath);
                foreach (var identity in driver.ApplicationIdentities)
                {
                    HmeServer server = new HmeServer(identity, driver);
                    if (serverAction != null)
                        serverAction(server);
                    Add(server, identity, assemblyFileInfo.DirectoryName);
                }
            }
        }
    }

    class HmeServerAndData
    {
        public HmeServer HmeServer { get; set; }
        public IHmeApplicationIdentity Identity { get; set; }
        public string ServerPath { get; set; }
    }
}

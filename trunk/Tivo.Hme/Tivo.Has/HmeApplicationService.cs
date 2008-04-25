using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace Tivo.Has
{
    public partial class HmeApplicationService : ServiceBase
    {
        public HmeApplicationService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: start a new thread
        }

        protected override void OnStop()
        {
            // TODO: abort worker thread
        }
    }
}

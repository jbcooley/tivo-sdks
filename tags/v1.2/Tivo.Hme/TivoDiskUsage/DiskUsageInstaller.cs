// Copyright (c) 2008 Josh Cooley

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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;


namespace TivoDiskUsage.Install
{
    [RunInstaller(true)]
    public partial class DiskUsageInstaller : Installer
    {
        public DiskUsageInstaller()
        {
            InitializeComponent();
        }

        public override void Commit(IDictionary savedState)
        {
            string assemblypath = Context.Parameters["assemblypath"];
            string mak = Context.Parameters["MAK"];
            string configFile = assemblypath + ".config";
            ExeConfigurationFileMap configurationMap = new ExeConfigurationFileMap();
            configurationMap.ExeConfigFilename = configFile;
            Configuration appconfig = ConfigurationManager.OpenMappedExeConfiguration(configurationMap, ConfigurationUserLevel.None);
            ConfigurationSectionGroup appSettings = appconfig.GetSectionGroup("applicationSettings");
            if (appSettings != null)
            {
                ClientSettingsSection mySettings = appSettings.Sections["TivoDiskUsage.Properties.Settings"] as ClientSettingsSection;
                if (mySettings != null)
                {
                    SettingElement makElement = mySettings.Settings.Get("MediaAccessKey");
                    if (makElement != null)
                    {
                        makElement.Value.ValueXml.InnerText = mak;
                        appconfig.Save(ConfigurationSaveMode.Minimal, true);
                    }
                }
            }
            base.Commit(savedState);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.IO;

namespace Tivo.Has.AddIn
{
    class HasApplicationConfigurator : IHasApplicationConfigurator
    {
        private ApplicationConfigurationCollection _applicationConfigurations = new ApplicationConfigurationCollection();

        #region IHasApplicationConfigurator Members

        public string Name
        {
            get { return Properties.Settings.Default.ApplicationPoolName; }
            set { Properties.Settings.Default.ApplicationPoolName = value; }
        }

        public IList<HasApplicationConfiguration> ApplicationConfigurations
        {
            get { return _applicationConfigurations; }
        }

        public IList<string> GetAccessableAssemblies()
        {
            return new ReadOnlyCollection<string>((
                (from file in new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).GetFiles()
                 where file.Extension == ".dll" || file.Extension == ".exe"
                 select file.FullName)
                .Concat
                (from file in
                     (from directoryInfo in
                          (from directoryName in AppDomain.CurrentDomain.RelativeSearchPath.Split(';')
                           select new DirectoryInfo(directoryName))
                      select directoryInfo.GetFiles()).SelectMany(fileInfo => fileInfo)
                 where file.Extension == ".dll" || file.Extension == ".exe"
                 select file.FullName)).ToList());
        }

        public IList<string> GetApplications(string assemblyPath)
        {
            return new ReadOnlyCollection<string>(
                (from t in System.Reflection.Assembly.ReflectionOnlyLoadFrom(assemblyPath).GetTypes()
                 where typeof(Tivo.Hme.HmeApplicationHandler).IsAssignableFrom(t)
                 select t.AssemblyQualifiedName).ToList());
        }

        public void CommitChanges()
        {
            Properties.Settings.Default.Save();
        }

        public void RollbackChanges()
        {
            Properties.Settings.Default.Reload();
        }

        #endregion
    }

    class ApplicationConfigurationCollection : Collection<HasApplicationConfiguration>
    {
        public ApplicationConfigurationCollection()
        {
            int count = Math.Min(Math.Min(
                Properties.Settings.Default.ApplicationName.Count,
                Properties.Settings.Default.EndPoint.Count),
                Properties.Settings.Default.ApplicationType.Count);
            for (int i = 0; i < count; ++i)
            {
                Items.Add(new HasApplicationConfiguration(
                    Properties.Settings.Default.ApplicationName[i],
                    new Uri(Properties.Settings.Default.EndPoint[i]),
                    Properties.Settings.Default.ApplicationType[i]));
            }
        }

        protected override void ClearItems()
        {
            Properties.Settings.Default.ApplicationName.Clear();
            Properties.Settings.Default.EndPoint.Clear();
            Properties.Settings.Default.ApplicationType.Clear();
            base.ClearItems();
        }

        protected override void InsertItem(int index, HasApplicationConfiguration item)
        {
            Properties.Settings.Default.ApplicationName.Insert(index, item.Name);
            Properties.Settings.Default.EndPoint.Insert(index, item.EndPoint.OriginalString);
            Properties.Settings.Default.ApplicationType.Insert(index, item.AssemblyQualifiedTypeName);
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            Properties.Settings.Default.ApplicationName.RemoveAt(index);
            Properties.Settings.Default.EndPoint.RemoveAt(index);
            Properties.Settings.Default.ApplicationType.RemoveAt(index);
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, HasApplicationConfiguration item)
        {
            Properties.Settings.Default.ApplicationName[index] = item.Name;
            Properties.Settings.Default.EndPoint[index] = item.EndPoint.OriginalString;
            Properties.Settings.Default.ApplicationType[index] = item.AssemblyQualifiedTypeName;
            base.SetItem(index, item);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Tivo.Hmo
{
    public enum TivoContainerSortCriteria
    {
        Type,
        Title,
        CreationDate,
        LastChangeDate,
    }

    public class TivoContainerQuery
    {
        private TivoConnection _connection;
        private string _container;
        private bool _recurse;
        private bool _doGenres;
        private List<string> _filter = new List<string>();
        private List<string> _sort = new List<string>();
        private int? _randomSeed;
        private int? _skipCount;
        private int? _limitCount;

        internal TivoContainerQuery(TivoConnection connection, string container)
        {
            _connection = connection;
            _container = container;
        }

        internal TivoContainerQuery(TivoConnection connection, Uri container)
        {
            _connection = connection;
            // TODO: make this work when the uri is already partially formed
        }

        private TivoContainerQuery Clone()
        {
            var clone = new TivoContainerQuery(_connection, _container);
            clone._recurse = _recurse;
            clone._filter.AddRange(_filter);
            clone._sort.AddRange(_sort);
            return clone;
        }

        public TivoContainerQuery Recurse()
        {
            var clone = Clone();
            clone._recurse = true;
            return clone;
        }

        public TivoContainerQuery DoGenres()
        {
            var clone = Clone();
            clone._doGenres = true;
            return clone;
        }

        // filter (matches mime type)
        TivoContainerQuery Filter(params string[] mimeTypes)
        {
            if (mimeTypes == null || mimeTypes.Length == 0)
                return this;
            var clone = Clone();
            clone._filter.AddRange(mimeTypes);
            return clone;
        }

        // sort
        // Type, Title, CreationDate, LastChangeDate
        TivoContainerQuery OrderBy(TivoContainerSortCriteria sortCriteria)
        {
            var clone = Clone();
            clone._sort.Clear();
            clone._sort.Add(sortCriteria.ToString());
            return clone;
        }

        TivoContainerQuery ThenBy(TivoContainerSortCriteria sortCriteria)
        {
            var clone = Clone();
            clone._sort.Add(sortCriteria.ToString());
            return clone;
        }

        TivoContainerQuery OrderByDescending(TivoContainerSortCriteria sortCriteria)
        {
            var clone = Clone();
            clone._sort.Clear();
            clone._sort.Add("!" + sortCriteria.ToString());
            return clone;
        }

        TivoContainerQuery ThenByDescending(TivoContainerSortCriteria sortCriteria)
        {
            var clone = Clone();
            clone._sort.Add("!" + sortCriteria.ToString());
            return clone;
        }

        TivoContainerQuery Randomize(int seed)
        {
            var clone = Clone();
            clone._sort.Clear();
            clone._sort.Add("Random");
            clone._randomSeed = seed;
            return clone;
        }

        public TivoContainerQuery Skip(int count)
        {
            var clone = Clone();
            clone._skipCount = count;
            return clone;
        }

        public TivoContainerQuery Take(int count)
        {
            var clone = Clone();
            clone._limitCount = count;
            return clone;
        }

        TivoContainerQuery Anchor(string url)
        {
            return Anchor(url, 0);
        }

        TivoContainerQuery Anchor(string url, int offset)
        {
            return null;
        }

        public TivoContainer Execute()
        {
            if (_connection.State != TivoConnectionState.Open)
                throw new InvalidOperationException();
            Uri uri;
            PrepareQueryContainer(_connection.WebClient, out uri);

            using (var stream = _connection.WebClient.OpenRead(uri))
            using (var reader = new System.IO.StreamReader(stream))
            {
                var doc = System.Xml.Linq.XDocument.Load(reader);
                return (TivoContainer)doc;
            }
        }

        public IAsyncResult BeginExecute(AsyncCallback asyncCallback, object asyncState)
        {
            if (_connection.State != TivoConnectionState.Open)
                throw new InvalidOperationException();
            Uri uri;
            PrepareQueryContainer(_connection.WebClient, out uri);
            _connection.WebClient.OpenReadCompleted += new OpenReadCompletedEventHandler(client_OpenReadCompleted);
            WebClientAsyncResult asyncResult = new WebClientAsyncResult(asyncCallback, asyncState);
            _connection.WebClient.OpenReadAsync(uri, asyncResult);
            return asyncResult;
        }

        void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            // unregister so query and connection can be reused.
            _connection.WebClient.OpenReadCompleted -= new OpenReadCompletedEventHandler(client_OpenReadCompleted);
            // return object
            WebClientAsyncResult asyncResult = (WebClientAsyncResult)e.UserState;
            try
            {
                using (var stream = e.Result)
                using (var reader = new System.IO.StreamReader(stream))
                {
                    var doc = System.Xml.Linq.XDocument.Load(reader);
                    asyncResult.Result = (TivoContainer)doc;
                }
            }
            catch (Exception ex)
            {
                asyncResult.Error = ex;
            }
            asyncResult.AsyncCallback(asyncResult);
            // TODO: what to do with errors and cancelled?
        }

        public TivoContainer EndExecute(IAsyncResult asyncResult)
        {
            WebClientAsyncResult webClientAsyncResult = asyncResult as WebClientAsyncResult;
            if (webClientAsyncResult == null)
                throw new ArgumentException("IAsyncResult did not come from BeginQueryContainer", "asyncResult");
            if (webClientAsyncResult.Error != null)
                throw webClientAsyncResult.Error;
            return (TivoContainer)webClientAsyncResult.Result;
        }

        public System.Threading.Tasks.Task<TivoContainer> ExecuteAsync()
        {
            Func<AsyncCallback, object, IAsyncResult> begin = BeginExecute;
            Func<IAsyncResult, TivoContainer> end = EndExecute;
            return System.Threading.Tasks.Task.Factory.FromAsync(begin, end, null);
        }

        private void PrepareQueryContainer(WebClient client, out Uri uri)
        {
            //ServicePointManager.ServerCertificateValidationCallback = TrustAllCertificatePolicy.TrustAllCertificateCallback;
            client.QueryString.Clear();
            client.QueryString.Add("Command", "QueryContainer");
            client.QueryString.Add("Container", _container);
            if (_recurse) // default is No
                client.QueryString.Add("Recurse", "Yes");
            if (_sort.Count != 0)
            {
                client.QueryString.Add("SortOrder", string.Join(",", _sort.ToArray()));
            }
            if (_randomSeed != null)
            {
                client.QueryString.Add("RandomSeed", _randomSeed.ToString());
            }
            if (_skipCount != null)
            {
                client.QueryString.Add("AnchorOffset", _skipCount.ToString());
            }
            if (_limitCount != null)
            {
                client.QueryString.Add("ItemCount", _limitCount.ToString());
            }
            if (_doGenres) // default is 0.
                client.QueryString.Add("DoGenres", "1");
            uri = new Uri("https://" + _connection.HmoServer + "/TiVoConnect");
        }
    }

    // tivo operations on urls
    // QueryItem
    // download

    //public class TivoConnection1 : IQueryable<TiVoContainerItem>
    //{
    //    #region IEnumerable<TiVoContainerItem> Members

    //    public IEnumerator<TiVoContainerItem> GetEnumerator()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    #endregion

    //    #region IEnumerable Members

    //    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    //    {
    //        return GetEnumerator();
    //    }

    //    #endregion

    //    #region IQueryable Members

    //    public Type ElementType
    //    {
    //        get { return typeof(TiVoContainerItem); }
    //    }

    //    public System.Linq.Expressions.Expression Expression
    //    {
    //        get { throw new NotImplementedException(); }
    //    }

    //    public IQueryProvider Provider
    //    {
    //        get { throw new NotImplementedException(); }
    //    }

    //    #endregion
    //}

    //public class TivoQueryProvider : IQueryProvider
    //{
    //    #region IQueryProvider Members

    //    public IQueryable<TElement> CreateQuery<TElement>(System.Linq.Expressions.Expression expression)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public IQueryable CreateQuery(System.Linq.Expressions.Expression expression)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public TResult Execute<TResult>(System.Linq.Expressions.Expression expression)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public object Execute(System.Linq.Expressions.Expression expression)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    #endregion
    //}
}

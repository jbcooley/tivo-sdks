using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.ComponentModel;

namespace Tivo.Hmo
{
    public class ContentDownloader
    {
        private TivoConnection _connection;
        private Uri _contentUri;

        public ContentDownloader(TivoConnection connection, Uri contentUri)
        {
            _connection = connection;
            _contentUri = contentUri;
        }

        public bool IsBusy
        {
            get
            {
                if (_connection.State == TivoConnectionState.Closed)
                    throw new InvalidOperationException();
                return _connection.WebClient.IsBusy;
            }
        }

        public void CancelAsync()
        {
            if (_connection.State == TivoConnectionState.Downloading)
                _connection.WebClient.CancelAsync();
        }

        public byte[] DownloadData()
        {
            ConnectEvents();
            return _connection.WebClient.DownloadData(_contentUri);
        }

        public void DownloadDataAsync()
        {
            ConnectEvents();
            _connection.WebClient.DownloadDataAsync(_contentUri);
        }

        public void DownloadDataAsync(object userToken)
        {
            ConnectEvents();
            _connection.WebClient.DownloadDataAsync(_contentUri, userToken);
        }

        public void DownloadFile(string fileName)
        {
            ConnectEvents();
            _connection.WebClient.DownloadFile(_contentUri, fileName);
        }

        public void DownloadFileAsync(string fileName)
        {
            ConnectEvents();
            _connection.WebClient.DownloadFileAsync(_contentUri, fileName);
        }

        public void DownloadFileAsync(string fileName, object userToken)
        {
            ConnectEvents();
            _connection.WebClient.DownloadFileAsync(_contentUri, fileName, userToken);
        }

        public System.IO.Stream OpenRead()
        {
            ConnectEvents();
            return _connection.WebClient.OpenRead(_contentUri);
        }

        public void OpenReadAsync()
        {
            ConnectEvents();
            _connection.WebClient.OpenReadAsync(_contentUri);
        }

        public void OpenReadAsync(object userToken)
        {
            ConnectEvents();
            _connection.WebClient.OpenReadAsync(_contentUri, userToken);
        }

        private void ConnectEvents()
        {
            if (_connection.State != TivoConnectionState.Open)
                throw new InvalidOperationException();
            _connection.WebClient.QueryString.Clear();
            _connection.WebClient.DownloadDataCompleted += new DownloadDataCompletedEventHandler(_client_DownloadDataCompleted);
            _connection.WebClient.DownloadFileCompleted += new AsyncCompletedEventHandler(_client_DownloadFileCompleted);
            _connection.WebClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(_client_DownloadProgressChanged);
            _connection.WebClient.OpenReadCompleted += new OpenReadCompletedEventHandler(_client_OpenReadCompleted);
        }

        private void DisconnectEvents()
        {
            _connection.WebClient.DownloadDataCompleted -= new DownloadDataCompletedEventHandler(_client_DownloadDataCompleted);
            _connection.WebClient.DownloadFileCompleted -= new AsyncCompletedEventHandler(_client_DownloadFileCompleted);
            _connection.WebClient.DownloadProgressChanged -= new DownloadProgressChangedEventHandler(_client_DownloadProgressChanged);
            _connection.WebClient.OpenReadCompleted -= new OpenReadCompletedEventHandler(_client_OpenReadCompleted);
        }

        void _client_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            DisconnectEvents();
            DownloadDataCompletedEventHandler handler = DownloadDataCompleted;
            if (handler != null)
                handler(this, e);
        }

        void _client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            DisconnectEvents();
            AsyncCompletedEventHandler handler = DownloadFileCompleted;
            if (handler != null)
                handler(this, e);
        }

        void _client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgressChangedEventHandler handler = DownloadProgressChanged;
            if (handler != null)
                handler(this, e);
        }

        void _client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            DisconnectEvents();
            OpenReadCompletedEventHandler handler = OpenReadCompleted;
            if (handler != null)
                handler(this, e);
        }

        public event DownloadDataCompletedEventHandler DownloadDataCompleted;
        public event AsyncCompletedEventHandler DownloadFileCompleted;
        public event DownloadProgressChangedEventHandler DownloadProgressChanged;
        public event OpenReadCompletedEventHandler OpenReadCompleted;
    }
}

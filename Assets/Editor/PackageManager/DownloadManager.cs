using System.Collections;
using System.Collections.Generic;
using System.Net;
using System;
using UnityEngine;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace Unity.CHIPS.DownloadManager.GitHub
{
    public class SecureWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
            request.ClientCertificates.Add(new X509Certificate());
            return request;
        }
    }

    public class DownloadManager
    {
        protected interface IDownloadRequest
        {
            void ProcessRequest( SecureWebClient client );
        }

        protected class DownloadFileRequest : IDownloadRequest
        {
            public string m_sourceFile;
            public string m_destination;

            public void ProcessRequest( SecureWebClient client )
            {
                client.DownloadFileAsync( new Uri(m_sourceFile), m_destination );
            }
        }
        
        protected class DownloadDataRequest : IDownloadRequest
        {
            public string         m_sourceFile;
            public Action<byte[]> m_downloadDataCompleted;

            public void ProcessRequest( SecureWebClient client )
            {
                client.DownloadDataCompleted += DataDownloadComplete;
                client.DownloadDataAsync( new Uri(m_sourceFile) );
                Debug.Log ("Trying to download data: " + m_sourceFile);
            }

            protected void DataDownloadComplete(object sender, DownloadDataCompletedEventArgs e)
            {
                SecureWebClient client = sender as SecureWebClient;
                client.DownloadDataCompleted -= DataDownloadComplete;
                m_downloadDataCompleted.Invoke(e.Result);
            }
        }

        protected IList<SecureWebClient>  m_connectionPool       = new List<SecureWebClient>();
        protected IList<IDownloadRequest> m_downloadRequestQueue = new List<IDownloadRequest>();

        public DownloadManager( int connectionPoolSize )
        {
            // HACK: This is not optimal. It overrides all HttpWebRequests and allows them to pass validation
            ServicePointManager.ServerCertificateValidationCallback += ValidateAllCertificates;
            // END HACK

            for ( int i = 0; i < connectionPoolSize; ++i )
            {
                var client = new SecureWebClient();
                client.DownloadFileCompleted += FileDownloadComplete;
                client.DownloadDataCompleted += DataDownloadComplete;
                m_connectionPool.Add( client );
            }
        }

        ~DownloadManager()
        {
            ServicePointManager.ServerCertificateValidationCallback -= ValidateAllCertificates;
            StopAllDownloads();
            for ( int i = 0; i < m_connectionPool.Count; ++i )
            {
                m_connectionPool[i].Dispose();
            }
        }

        public void DownloadFile( string sourceFile, string destination )
        {
            m_downloadRequestQueue.Add( new DownloadFileRequest() { m_sourceFile = sourceFile, m_destination = destination } );

            ProcessDownloadRequests();
        }

        public void DownloadData( string sourceFile, Action<byte[]> downloadCompleteDelegate )
        {
            m_downloadRequestQueue.Add( new DownloadDataRequest() { m_sourceFile = sourceFile, m_downloadDataCompleted = downloadCompleteDelegate } );
            
            ProcessDownloadRequests();
        }

        public void StopAllDownloads()
        {
            m_downloadRequestQueue.Clear();
            for ( int i = 0; i < m_connectionPool.Count; ++i )
            {
                m_connectionPool[i].CancelAsync();
            }
        }

        protected void FileDownloadComplete(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            ProcessDownloadRequests();
        }

        protected void DataDownloadComplete(object sender, DownloadDataCompletedEventArgs e)
        {
            ProcessDownloadRequests();
        }

        protected void ProcessDownloadRequests()
        {
            // If we have any pending file requests
            if ( m_downloadRequestQueue.Count > 0 )
            {
                // Find the next available connection and use it
                for ( int i = 0; i < m_connectionPool.Count; ++i )
                {
                    var client = m_connectionPool[i];
                    if ( !client.IsBusy )
                    {
                        // Check to make sure we still have a request, a previous pool may have taken one
                        if ( m_downloadRequestQueue.Count > 0 )
                        {
                            m_downloadRequestQueue[0].ProcessRequest( client );
                            m_downloadRequestQueue.RemoveAt(0);
                        }
                    }
                }
            }
        }

        protected bool ValidateAllCertificates(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
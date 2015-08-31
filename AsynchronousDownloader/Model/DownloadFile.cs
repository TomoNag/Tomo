using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AsynchronousDownloader.ViewModel;

using MyUtil;
using IPCTestServer;

namespace AsynchronousDownloader.Model
{
    public class DownloadFile : INotifyPropertyChanged
    {
        public CustomWebClient client;
        public DownloadData Data;

        private string protocol;
        public string Protocol
        {
            get { return protocol; }
            set
            {
                protocol = value;
                OnPropertyChanged("Protocol");
            }
        }

        private string extension;
        public string Extension
        {
            get { return extension; }
            set
            {
                extension = value;
                OnPropertyChanged("Extension");
            }
        }

        private string filename;
        public string Filename
        {
            get { return filename; }
            set
            {
                filename = value;
                OnPropertyChanged("Filename");
            }
        }

        private string downloadLocation;
        public string DownloadLocation
        {
            get { return downloadLocation; }
            set
            {
                downloadLocation = value;
                OnPropertyChanged("DownloadLocation");
            }
        }

        public string Folder { get; set; }
        public string Title { get; set; }

        private string uri;
        public string Uri
        {
            get { return uri; }
            set
            {
                uri = value;               
                OnPropertyChanged("Uri");
            }
        }

        private string downloadPercentageString;
        public string DownloadPercentageString
        {
            get { return downloadPercentageString; }
            set
            {
                downloadPercentageString = value;
                OnPropertyChanged("DownloadPercentageString");
            }
        }

        private int downloadPercentage;
        public int DownloadPercentage
        {
            get { return downloadPercentage; }
            set
            {
                downloadPercentage = value;
                OnPropertyChanged("DownloadPercentage");
            }
        }

        private string downloadTime;
        public string DownloadTime
        {
            get { return downloadTime; }
            set
            {
                downloadTime = value;
                OnPropertyChanged("DownloadTime");
            }
        }

        private string downloadSpeed;
        public string DownloadSpeed
        {
            get { return downloadSpeed; }
            set
            {
                downloadSpeed = value;
                OnPropertyChanged("DownloadSpeed");
            }
        }

        private string fileSize;
        public string FileSize
        {
            get { return fileSize; }
            set
            {
                fileSize = value;
                OnPropertyChanged("FileSize");
            }
        }

        private DownloadStatus downloadStatus;
        public DownloadStatus DownloadStatus
        {
            get { return downloadStatus; }
            set
            {
                downloadStatus = value;
                DownloadFileViewModel.CanDownload(this);
                OnPropertyChanged("DownloadStatus");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void CalculateFileSize()
        {
            FileSize = CalculateFileSize(Uri);
        }

        private string CalculateFileSize(string uri)
        {
            //HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
            //req.Method = "HEAD";
            //req.Timeout = 200;

            //try
            //{
            //    HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            //    return string.Format("{0} MB", Math.Round((double)resp.ContentLength / 1048576, 2));
            //}
            //catch
            //{
            //    req.Abort();
            //    return "";
            //}

            return "";
           
          

        }
    }

    public enum DownloadStatus
    {
        NotDownloaded,
        LiveDownloading,
        RTMPDownloading,
        SilverlightDownloading,
        Downloading,
        Downloaded,
        FileExists,
        Error,
        Canceled
    }
}

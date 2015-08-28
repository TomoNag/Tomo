using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AsynchronousDownloader.Commands;
using AsynchronousDownloader.Model;
using System.Collections.ObjectModel;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using IPCTestServer;
using MyUtil;

namespace AsynchronousDownloader.ViewModel
{
    public class DownloadFileViewModel : INotifyPropertyChanged
    {
        private static DownloadFileViewModel _viewModel;

    






        private string downloadFolder;
        public string DownloadFolder
        {
            get { return downloadFolder; }
            set
            {
                downloadFolder = value;
                OnPropertyChanged("DownloadFolder");
            }
        }



        private ObservableCollection<DownloadFile> downloadFiles;
        public ObservableCollection<DownloadFile> DownloadFiles
        {
            get { return downloadFiles; }
            set
            {
                downloadFiles = value;
                OnPropertyChanged("DownloadFiles");
            }
        }

        public void AddFile(DownloadData data)
        {
            string extension = "";

            if (data.Extension != "")
            {
                extension = data.Extension;
            }
            else
            {
                extension = Util.GetExtension(data.URL);
            }

            var file = new DownloadFile { Uri = data.URL, Folder = data.Folder, Title = data.Title, Protocol = data.Protocol, Extension = extension };


            downloadFiles.Add(file);


            Console.WriteLine("Success");
            OnPropertyChanged("DownloadFiles");

        }

        public void ClearFile()
        {
            //tolistでコピーを作る。。。
            foreach (var item in downloadFiles.ToList())
            {
                if (item.DownloadStatus != DownloadStatus.Downloading)
                {
                    downloadFiles.Remove(item);
                }
            }

            OnPropertyChanged("DownloadFiles");
        }

       

     

        public RelayCommand ClearCommand { get; set; }
        public RelayCommand DownloadCommand { get; set; }
        public RelayCommand OpenFileCommand { get; set; }

        public static DownloadFileViewModel SharedViewModel()
        {
            return _viewModel ?? (_viewModel = new DownloadFileViewModel());
        }

        private DownloadFileViewModel()
        {
            DownloadCommand = new RelayCommand(Download, CanDownload);
            OpenFileCommand = new RelayCommand(OpenFile, CanOpenFile);
            ClearCommand = new RelayCommand(Clear, CanClearFile);
            DownloadFiles = new ObservableCollection<DownloadFile>();
          

        }

        private bool CanClearFile(object obj)
        {
            return true;
        }

        private void Clear(object obj)
        {
            var file = obj as DownloadFile;
            if (file.DownloadStatus == DownloadStatus.Downloading)
            {
                if (MessageBox.Show("ダウンロードを中止してもよろしいですか?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    //do no stuff
                    return;
                }
                else
                {
                    //do yes stuff
                    file.client.CancelAsync();
                    DownloadFiles.Remove(file);
                }
            }
            else
            {
                DownloadFiles.Remove(file);
            }

        }

        private void OpenFile(object obj)
        {
            var file = obj as DownloadFile;
            Process.Start("explorer.exe", string.Format("/select,\"{0}\"", file.DownloadLocation));
        }

        private bool CanOpenFile(object obj)
        {
            return true;
        }

        public static bool CanDownload(object obj)
        {
            var file = obj as DownloadFile;
            return (obj != null && file.DownloadStatus != DownloadStatus.Downloaded && file.DownloadStatus != DownloadStatus.Downloading);
        }

        private void Download(object obj)
        {
            Download(obj, true);
        }


        private static string MakeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
        }

       
        private void Download(object obj, bool checkfile)
        {


            var file = obj as DownloadFile;

          

            string Folder = DownloadFolder;
            string Filename = file.Filename;
            if (file.Folder != "")
            {
                Folder = DownloadFolder + "\\" + file.Folder;
            }


            if (file.Title != "")
            {
                int max = 50;
                if(file.Title.Length > max)
                {
                    Filename = file.Title.Substring(0,max) + "-" + file.Filename;
                }
                else
                {
                    Filename = file.Title + "-" + file.Filename;
                }             
              
            }

            //Folder = MakeValidFileName(Folder);
            Filename = MakeValidFileName(Filename);


            if (!Directory.Exists(Folder))
            {
                try
                {
                    Directory.CreateDirectory(Folder);

                }
                catch
                {
                    if (checkfile == true)
                    {
                        MessageBox.Show("フォルダの作成に失敗しました。");
                    }

                    file.DownloadStatus = DownloadStatus.Error;
                    return;
                }
            }

            string location = string.Format("{0}\\{1}", Folder, Filename);
            if(file.Extension != "")
            {
                location += "." + file.Extension;
            }

            if(File.Exists(location))
            {
                if (checkfile == true)
                {
                    if (MessageBox.Show("同名ファイルが存在します。削除してもよろしいですか?", "確認", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    {
                        //do no stuff
                        file.DownloadStatus = DownloadStatus.FileExists;
                        return;
                    }
                    else
                    {
                        try {
                            File.Delete(location);
                            
                        }
                        catch
                        {
                            MessageBox.Show("ファイルの削除に失敗しました。");
                            file.DownloadStatus = DownloadStatus.FileExists;
                            return;
                        }
                    }

                }
                else
                {
                    file.DownloadStatus = DownloadStatus.FileExists;
                    return;
                }

            }

            if (file.Protocol == "LIVE")
            {
                Console.WriteLine(file.Uri);

                LiveRecorder recorder = new LiveRecorder();
                recorder.LiveRecord(location, file.Uri);
                file.DownloadStatus = DownloadStatus.Downloading;

                return;
            }



            file.DownloadStatus = DownloadStatus.Downloading;

            file.CalculateFileSize();

            var webClient = new CustomWebClient();
            webClient.Data = file;
            webClient.Clock = new Stopwatch();
            webClient.DownloadProgressChanged += webClient_DownloadProgressChanged;
            webClient.DownloadFileCompleted += webClient_DownloadFileCompleted;
            webClient.Clock.Start();


            webClient.Moving = true;
            webClient.Timer = new System.Windows.Threading.DispatcherTimer();
            webClient.Timer.Tag = webClient;
            webClient.Timer.Tick += Timer_Tick;
            webClient.Timer.Interval = new TimeSpan(0, 0, 10);
            webClient.Timer.Start();


            webClient.DownloadFileAsync(new Uri(file.Uri), file.DownloadLocation = location);

            file.client = webClient;
             

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var Timer = (DispatcherTimer)sender;
            var Client = Timer.Tag as CustomWebClient;

            if(Client.Moving == false)
            {
                Client.CancelAsync();
            }
            else
            {
                Client.Moving = false;
            }
        }

        public void AutoDownload(int num)
        {
            try {
                int count = 0;
                foreach (var item in DownloadFiles)
                {
                    if (item.DownloadStatus == DownloadStatus.Downloading)
                    {
                        count++;
                    }
                }

                foreach (var item in DownloadFiles)
                {
                    if (count >= num)
                    {
                        return;
                    }

                    if (item.DownloadStatus == DownloadStatus.NotDownloaded)
                    {
                        Download(item, false);
                        count++;
                    }
                }
            }
            catch
            {

            }

        }


        private void CancelDownload(object obj)
        {


            var file = obj as DownloadFile;
            file.client.CancelAsync();


        }


        void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            var customWebClient = sender as CustomWebClient;
            customWebClient.Moving = true;

            var downloadFile = (customWebClient.Data as DownloadFile);


            downloadFile.FileSize = string.Format("{0} MB", Math.Round((double)e.TotalBytesToReceive / 1048576, 2));

            downloadFile.DownloadPercentageString = string.Format("{0} %", int.Parse(Math.Truncate(((double.Parse(e.BytesReceived.ToString()) / double.Parse(e.TotalBytesToReceive.ToString())) * 100)).ToString()));
            downloadFile.DownloadPercentage = int.Parse(Math.Truncate(((double.Parse(e.BytesReceived.ToString()) / double.Parse(e.TotalBytesToReceive.ToString())) * 100)).ToString());
            downloadFile.DownloadTime = string.Format("{0} s", int.Parse(Math.Truncate(customWebClient.Clock.Elapsed.TotalSeconds).ToString()));
            downloadFile.DownloadSpeed = string.Format("{0} kb/s", (e.BytesReceived / 1024d / customWebClient.Clock.Elapsed.TotalSeconds).ToString("0.00"));
        }

        void webClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            var customWebClient = sender as CustomWebClient;
            customWebClient.Timer.Stop();

            var downloadFile = (customWebClient.Data as DownloadFile);
            if (e.Cancelled)
            {
                downloadFile.DownloadStatus = DownloadStatus.Canceled;
                return;
            }

            if (e.Error != null) // We have an error! Retry a few times, then abort.
            {
                downloadFile.DownloadStatus = DownloadStatus.Error;
                return;
            }
     

            downloadFile.DownloadPercentageString = string.Format("{0} %","100");
            downloadFile.DownloadPercentage = 100;
            downloadFile.DownloadTime = string.Format("{0} s", int.Parse(Math.Truncate(customWebClient.Clock.Elapsed.TotalSeconds).ToString()));



            downloadFile.DownloadStatus = DownloadStatus.Downloaded;
            customWebClient.DownloadFileCompleted -= webClient_DownloadFileCompleted;
            customWebClient.DownloadProgressChanged -= webClient_DownloadProgressChanged;
        }

      
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}

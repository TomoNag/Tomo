using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AsynchronousDownloader.ViewModel;
using AsynchronousDownloader.Model;

using MyUtil;
using System.IO;
using IPCTestServer;
using System.Threading;

using System.Windows.Threading;

namespace AsynchronousDownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DownloadFileViewModel viewmodel;
        ClipBoardWatcher cbw;

        private Dispatcher _dispatcher = Application.Current.Dispatcher;
     
        public void AddUrl(DownloadData data)
        {
            _dispatcher.BeginInvoke(new Action(() =>
            {
                viewmodel.AddFile(data);

            }), (DispatcherPriority)10);
           
        }

        public void AddUrl(string temp, string folder = "", string title = "")
        {
           

            var list = temp.Split('\n');

            foreach (var item in list)
            {
                Uri uriResult;


                var list2 = item.Split('\t');
                string url = list2[0];

                bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult);
                if (result == false)
                {
                    continue;
                }

                if (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)
                {
                    //string ext = Util.GetExtension(url);
                    //if (Util.IsMovie(ext) || Util.IsPicture(ext))
                    {
                        //Console.WriteLine(temp);
                        _dispatcher.BeginInvoke(new Action(() =>
                        {
                            string protocol = "HTTP";
                            string ext = Util.GetExtension(url);
                            if(Util.IsLive(ext))
                            {
                                protocol = "LIVE";
                            }
                            DownloadData data = new DownloadData
                            {
                                URL = url,
                                Folder = folder,
                                Title = title,
                                Protocol = protocol
                           
                            };


                            viewmodel.AddFile(data);

                        }), (DispatcherPriority)10);
                    }
                }


            }
           
        }

        public MainWindow()
        {

            InitializeComponent();
            viewmodel = DownloadFileViewModel.SharedViewModel(); ;
            this.DataContext = viewmodel;

            viewmodel.DownloadFolder = Directory.GetCurrentDirectory() + "\\Download";

            IPCTestServer.IPCTestServer.SetWindow(this);

           
            new Thread(() =>
            {
                var server = new MyServer();


            }).Start();

            if (false)
            {
                cbw = new ClipBoardWatcher();
                cbw.DrawClipBoard += (sender, e) =>
                {



                    if (Clipboard.ContainsText())
                    {
                        string temp = Clipboard.GetText();
                        AddUrl(temp);


                    }
                };
            }

            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

      


　　　　private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            viewmodel.AutoDownload(3);
            // code goes here
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //DownloadData data = new DownloadData()
            //{
            //    URL = "text",
            //    Folder = "",
            //    Extension = "",
            //    Protocol = "",
            //    Title = "",
            //};

            //var client = new MyClient();
            //client.Add(data);

            viewmodel.ClearFile();
          

        }
    }
}

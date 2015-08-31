using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.IO;
using IPCTestServer;
using MyUtil;

namespace CefSharp.MinimalExample.WinForms
{
    class SizeGettor
    {
        public WebClient webClient = new WebClient();
        public long size;
        public DownloadData data;
        public MyClient client;

        static string directory = "temp";

        public static void Clear()
        {
            Util.DeleteAllFiles(directory);
        }

        public void Get(string url)
        {
            Directory.CreateDirectory(directory);

           
            string Filename = url.Split('/').Last();

            Filename = Util.MakeValidFileName(Filename);
            webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
            webClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;        
            webClient.DownloadFileAsync(new Uri(url), directory + "/" + Filename);
        }

        private void WebClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (size > 1024 * 1024 * 5)
            {
                client.Add(data);
            }
        }

        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            var WebClient = sender as WebClient;

            size = e.TotalBytesToReceive;


            WebClient.CancelAsync();

        }
      
    }
}

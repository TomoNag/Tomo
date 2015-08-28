using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace AsynchronousDownloader.ViewModel
{
    [System.ComponentModel.DesignerCategory("")]
    public class CustomWebClient : WebClient
    {
        public object Data { get; set; }
        public Stopwatch Clock { get; set; }
        public bool Moving { get; set; }
        public DispatcherTimer Timer { get; set; } 
    }
}

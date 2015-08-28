// Copyright © 2010-2015 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using System;
using System.Windows.Forms;
using CefSharp.MinimalExample.WinForms.Controls;
using CefSharp.WinForms;

using System.Windows.Forms.Integration; //Not so Given.
using IPCTestServer;
using System.Threading;
using HtmlAgilityPack;
using System.Windows.Threading;
using MyUtil;
using System.Collections.Generic;
using System.Linq;

namespace CefSharp.MinimalExample.WinForms
{
    public partial class BrowserForm : Form, IRequestHandler
    {
        private readonly ChromiumWebBrowser browser;


        public string domain = "";
        public string fullurl = "";

      
        public bool GetAuthCredentials(IWebBrowser browser, bool isProxy, string host, int port, string realm, string scheme, ref string username, ref string password) { return true; }
        public bool OnBeforeBrowse(IWebBrowser browser, IRequest request, bool isRedirect, bool isMainFrame) { return false; }
        public bool OnBeforePluginLoad(IWebBrowser browser, string url, string policyUrl, WebPluginInfo info) { return false; }
        public CefReturnValue OnBeforeResourceLoad(IWebBrowser browser, IRequest request, bool isMainFrame)
        {
            string url = request.Url;

         

            string ext = Util.GetExtension(url);
            string newextension = "";
            bool found = false;
            string protocol = "HTTP";

            if (Util.IsLive(ext))
            {
                protocol = "LIVE";
                found = true;
            }

            if (Util.IsMovie(ext))
            {
                found = true;

                if (Util.IsLiveDomain(domain))
                {
                    found = false;
                }
            }


            if (domain == "twitcasting.tv" && url.Contains("/download/"))
            {
                newextension = "mp4";
                found = true;
            }

            if (found == true)
            {


                DownloadData data = new DownloadData
                {
                    URL = url,
                    Title = this.Text,
                    Folder = domain,
                    Protocol = protocol,
                    Extension = newextension
                };


                new Thread(() =>
                {

                    client.Add(data);

                }).Start();
            }
            return CefReturnValue.Continue;
        }

        public bool OnCertificateError(IWebBrowser browser, CefErrorCode errorCode, string requestUrl) { return true; }
        public void OnPluginCrashed(IWebBrowser browser, string pluginPath) { }
        public void OnRenderProcessTerminated(IWebBrowser browser, CefTerminationStatus status) { }

        MyClient client = new MyClient();



   

  

        public BrowserForm()
        {



            InitializeComponent();

            Text = "Movie Downloader";
            WindowState = FormWindowState.Maximized;

            browser = new ChromiumWebBrowser("http://twitcasting.tv/azuzuaazu/movie/196332866")
            {
                Dock = DockStyle.Fill,
            };

            browser.RequestHandler = this;

            //browser.ResourceHandlerFactory = new MyResourceHandlerFactory();
            //browser.RequestHandler = new HttpHandler();

            //toolStripContainer.ContentPanel.Controls.Add(browser);
            splitContainer1.Panel1.Controls.Add(browser);


            browser.NavStateChanged += OnBrowserNavStateChanged;
            //browser.LoadingStateChanged += Browser_LoadingStateChanged;

            browser.ConsoleMessage += OnBrowserConsoleMessage;
            browser.StatusMessage += OnBrowserStatusMessage;
            browser.TitleChanged += OnBrowserTitleChanged;
            browser.AddressChanged += OnBrowserAddressChanged;

            var bitness = Environment.Is64BitProcess ? "x64" : "x86";
            var version = String.Format("Chromium: {0}, CEF: {1}, CefSharp: {2}, Environment: {3}", Cef.ChromiumVersion, Cef.CefVersion, Cef.CefSharpVersion, bitness);
            DisplayOutput(version);


            new Thread(() =>
            {

                client.Connect();

            }).Start();

            //MainWindow wpfwindow = new MainWindow();
            //ElementHost.EnableModelessKeyboardInterop(wpfwindow);
            //wpfwindow.Show();
        }

        //private void Browser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs args)
        //{
        //    SetCanGoBack(args.CanGoBack);
        //    SetCanGoForward(args.CanGoForward);

        //    if (args.IsLoading == false)
        //    {
        //        NavigateCompleted();
        //    }


        //    this.InvokeOnUiThreadIfRequired(() => SetIsLoading(!args.CanReload));
        //}

        private void OnBrowserNavStateChanged(object sender, NavStateChangedEventArgs args)
        {
            SetCanGoBack(args.CanGoBack);
            SetCanGoForward(args.CanGoForward);

            if (args.IsLoading == false)
            {
                NavigateCompleted();
            }


            this.InvokeOnUiThreadIfRequired(() => SetIsLoading(!args.CanReload));
        }
        private void OnBrowserConsoleMessage(object sender, ConsoleMessageEventArgs args)
        {
            DisplayOutput(string.Format("Line: {0}, Source: {1}, Message: {2}", args.Line, args.Source, args.Message));
        }

        private void OnBrowserStatusMessage(object sender, StatusMessageEventArgs args)
        {
            this.InvokeOnUiThreadIfRequired(() => statusLabel.Text = args.Value);
        }

        

        private void OnBrowserTitleChanged(object sender, TitleChangedEventArgs args)
        {
            this.InvokeOnUiThreadIfRequired(() => Text = args.Title);
        }

        private void OnBrowserAddressChanged(object sender, AddressChangedEventArgs args)
        {
            Uri myUri = new Uri(args.Address);
            domain = myUri.Host;
            fullurl = args.Address;

            this.InvokeOnUiThreadIfRequired(() => urlTextBox.Text = args.Address);
        }

        private void SetCanGoBack(bool canGoBack)
        {
            this.InvokeOnUiThreadIfRequired(() => backButton.Enabled = canGoBack);
        }

        private void SetCanGoForward(bool canGoForward)
        {
            this.InvokeOnUiThreadIfRequired(() => forwardButton.Enabled = canGoForward);
        }

        private void SetIsLoading(bool isLoading)
        {
            goButton.Text = isLoading ?
                "Stop" :
                "Go";
            goButton.Image = isLoading ?
                Properties.Resources.nav_plain_red :
                Properties.Resources.nav_plain_green;

            HandleToolStripLayout();
        }

        public void DisplayOutput(string output)
        {
            this.InvokeOnUiThreadIfRequired(() => outputLabel.Text = output);
        }

        private void HandleToolStripLayout(object sender, LayoutEventArgs e)
        {
            HandleToolStripLayout();
        }

        private void HandleToolStripLayout()
        {
            var width = toolStrip1.Width;
            foreach (ToolStripItem item in toolStrip1.Items)
            {
                if (item != urlTextBox)
                {
                    width -= item.Width - item.Margin.Horizontal;
                }
            }
            urlTextBox.Width = Math.Max(0, width - urlTextBox.Margin.Horizontal - 18);
        }

        private void ExitMenuItemClick(object sender, EventArgs e)
        {
            browser.Dispose();
            Cef.Shutdown();
            Close();
        }

        private void GoButtonClick(object sender, EventArgs e)
        {
            LoadUrl(urlTextBox.Text);
        }

        private void BackButtonClick(object sender, EventArgs e)
        {
            browser.Back();
        }

        private void ForwardButtonClick(object sender, EventArgs e)
        {
            browser.Forward();
        }

        private void UrlTextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            LoadUrl(urlTextBox.Text);
        }

        private void LoadUrl(string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
            {
                browser.Load(url);
            }
        }

        bool executing = false;
        public void NavigateCompleted()
        {
            if(executing == true)
            {
                return;
            }

            executing = true;
            //GetLinks();
            Console.WriteLine("NC");

            this.Invoke(new Action(() => browser.ExecuteScriptAsync(richTextBox2.Text)));



            new Thread(() =>
            {
                Thread.Sleep(100);
                this.Invoke(new Action(() => GetLinks()));

            }).Start();

            new Thread(() =>
            {
                Thread.Sleep(100);
                executing = false;

            }).Start();

        }

        public void GetLinks()
        {
            try {

                var task = browser.GetSourceAsync();
                //task.Start();
                task.Wait(1000);

                if(task.IsCompleted == false)
                {
                    return;
                }


                string html = task.Result;
                Uri absolute = new Uri(fullurl);

                string anchor = "src= *\"(?<value>.*?)\"";
                var list = Util.Analyze2(html, anchor);
                foreach (var item in list)
                {
                    string url = item;

                    try {
                        Uri result = new Uri(absolute, url);
                        url = result.AbsoluteUri;
                    }
                    catch
                    {
                        Console.WriteLine(url);
                        continue;
                    }




                    bool found = false;
                    string protocol = "HTTP";
                    string ext = Util.GetExtension(url);

                    if (Util.IsLiveURL(url))
                    {
                        protocol = "LIVE";
                        found = true;
                    }

                    if (Util.IsMovie(ext))
                    {
                        found = true;
                    }

                    if (found == true)
                    {

                        DownloadData data = new DownloadData
                        {
                            URL = url,
                            Title = this.Text,
                            Folder = domain,
                            Protocol = protocol,
                            Extension = "",

                        };


                        new Thread(() =>
                        {

                            client.Add(data);

                        }).Start();
                    }

                  
                }

                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                string links = "";


                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]"))
                {
                    string url = link.Attributes["href"].Value;


                    try
                    {
                        Uri result = new Uri(absolute, url);
                        url = result.AbsoluteUri;
                    }
                    catch
                    {
                        Console.WriteLine(url);
                        continue;
                    }

                    string text = link.InnerText;

                    if (url.Contains(textBox1.Text))
                    {
                        text = text.Replace('\n', ' ');
                        text = text.Replace('\t', ' ');

                        links += url + "\t" + text + "\n";
                    }


                    string ext = Util.GetExtension(url);
                    if (Util.IsMovie(ext) || Util.IsPicture(ext))
                    {
                        DownloadData data = new DownloadData
                        {
                            URL = url,
                            Title = this.Text,
                            Folder = domain,
                            Protocol = "HTTP",
                            Extension = "",

                        };


                        new Thread(() =>
                        {

                            client.Add(data);

                        }).Start();
                    }
                }
                richTextBox1.Text = links;
            }
            catch
            {
               
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {

            GetLinks();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            browser.ExecuteScriptAsync(richTextBox2.Text);
        }


        List<string> DownloadList;
        int listindex = 0;
        bool autoloading = false;
        private void button3_Click(object sender, EventArgs e)
        {

            if (autoloading == false)
            {
                int interval = 0;
                Int32.TryParse(textBox2.Text, out interval);
                if (interval <= 0)
                {
                    MessageBox.Show("タイマー設定が不正です。");
                    return;
                }

                autoloading = true;

                DownloadList = new List<string>();
                foreach (var item in richTextBox1.Lines)
                {
                    if (item == "")
                    {
                        continue;
                    }

                    var url = item.Split('\t')[0];

                    if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
                    {
                        DownloadList.Add(url);

                    }
                }

                DownloadList = DownloadList.Distinct().ToList();

                listindex = 0;

                TimerTick();

                timer1.Interval = interval * 1000;
                timer1.Start();

                button3.Text = "ストップ";
            }
            else
            {
                autoloading = false;
                timer1.Stop();

                button3.Text = "一括ダウンロード";
            }


        }

        public void TimerTick()
        {
            if (listindex < DownloadList.Count)
            {
                string url = DownloadList[listindex];

                browser.Load(url);

                listindex++;
            }
            else
            {
                autoloading = false;
                timer1.Stop();

                button3.Text = "一括ダウンロード";

            }

        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            TimerTick();
        }
    }
}

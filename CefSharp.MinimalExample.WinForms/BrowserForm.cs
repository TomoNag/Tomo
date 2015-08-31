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
using System.Drawing;
using System.Text;
using System.Net;

namespace CefSharp.MinimalExample.WinForms
{
    public partial class BrowserForm : Form, IRequestHandler
    {
        private ChromiumWebBrowser browser;


        public string domain = "";
        public string fullurl = "";
        public string homeurl = "google.co.jp";
        public string SavedSilverLight = "";
        public DownloadData livedata = null;

        public void SetLiveData(DownloadData data)
        {
            livedata = data;
            button4.Enabled = true;
            button4.BackColor = Color.Crimson;

            if (checkBox1.Checked == true)
            {
                LiveDownload();
            }
        }

        public void DeleteLiveData()
        {
            livedata = null;
            button4.Enabled = false;
            button4.BackColor = SystemColors.Control;
        }

        public void Download(DownloadData data)
        {
            //####
            if (checkBox4.Checked)
            {
                data.URL = data.URL.Replace(textBox4.Text, textBox5.Text);
            }

            new Thread(() =>
            {

                client.Add(data);

            }).Start();
        }


        public bool GetAuthCredentials(IWebBrowser browser, bool isProxy, string host, int port, string realm, string scheme, ref string username, ref string password) { return true; }
        public bool OnBeforeBrowse(IWebBrowser browser, IRequest request, bool isRedirect, bool isMainFrame) { return false; }
        public bool OnBeforePluginLoad(IWebBrowser browser, string url, string policyUrl, WebPluginInfo info) { return false; }
        public CefReturnValue OnBeforeResourceLoad(IWebBrowser browser, IRequest request, bool isMainFrame)
        {


            string url = request.Url;
            //Console.WriteLine(url);

            string ext = Util.GetExtension(url);
            string newextension = "";
            bool found = false;
            string protocol = "HTTP";
            //bool forcedownload = true;

            //if (forcedownload == true)
            if (Util.IsLive(ext))
            {
                protocol = "LIVE";
                //newextension = "ts";
                found = true;
            }

            if (domain.Contains("ustream.tv"))
            {
                if (url.Contains(".flv?start="))
                {
                    Console.WriteLine(url);
                    //http://sjc-ucdn01-comcast-tcdn.ustream.tv/sjc-uhs14/streams/httpflv/ustreamVideo/3849993/streams/live_1_1440823221_1305193735.flv?start=334
                    string anchor = "ustreamVideo/(?<value>[^/]*?)/streams";

                    string ustream = "http://iphone-streaming.ustream.tv/uhls/[ID]/streams/live/iphone/playlist.m3u8";
                    string id = Util.Analyze(url, anchor);
                    ustream = ustream.Replace("[ID]", id);

                    url = ustream;
                    protocol = "LIVE";
                    found = true;
                }
                else if (url.Contains(".flv?e="))
                {
                    if (url.Contains("&fs="))
                    {
                        return CefReturnValue.Continue;
                    }
                }
                else if (url.Contains(".flv"))
                {
                    return CefReturnValue.Continue;
                }


            }

            if (checkBox3.Checked == true && found == false)
            {
                return CefReturnValue.Continue;
            }


            if (checkBox2.Checked)
            {
                new Thread(() =>
                {
                    DownloadData data = new DownloadData
                    {
                        URL = url,
                        Title = this.Text,
                        Folder = domain,
                        Protocol = protocol,
                        Extension = newextension
                    };

                    var gettor = new SizeGettor();
                    gettor.data = data;
                    gettor.client = client;
                    gettor.Get(url);



                }).Start();

                //return CefReturnValue.Continue;

            }




            if (Util.IsMovie(ext))
            {
                found = true;

                if (Util.IsLiveDomain(domain))
                {
                    found = false;
                }
            }



            if (domain.Contains("twitcasting.tv") && url.Contains("/download/"))
            {
                newextension = "mp4";
                found = true;
            }

            //if (domain.Contains("twitcasting.tv") && url.Contains("/streamserver.php?mode=view"))
            //{
            //    if (checkBox1.Checked == true)
            //    {
            //        LiveDownload();
            //        return CefReturnValue.Continue;
            //    }
            //}

            if (url.Contains("get_player_video_info"))
            {
                string json = Util.GetHtml(url, Encoding.UTF8);
                var infos = ULIZA.Uliza(json, domain);



                foreach (var item in infos)
                {
                    Download(item);
                }
                  
               
                return CefReturnValue.Continue;
            }

            if((url.Contains(".ism") && url.Contains("Fragments(video=0)")) || ext == "ism")
            {
                if (url.Contains("Fragments(video=0)"))
                {
                    url = Util.RemoveExtension(url);
                    url = url + ".ism/Manifest";
                }

              

                if (url == SavedSilverLight)
                {

                }
                else
                {
                    //http://azrvnsss.video.rakuten.co.jp/ondemand/storage001/X/Z/XZPLOPDHFS/DEhxkW_pc_sd.ism/QualityLevels(698571)/Fragments(video=0)
                    protocol = "ISM";
                    newextension = "mp4";
                    found = true;

                    Console.WriteLine(url);
                }

                SavedSilverLight = url;
            }
            //if (url.Contains("apiPassword="))
            //{
            //    //url.Contains("filedownload?") || 
            //    //letv filedownload
            //    //url.cm apiPassword
            //    found = true;
            //}


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


                if (protocol == "LIVE")
                {
                    this.InvokeOnUiThreadIfRequired(() => SetLiveData(data));
                }
                else
                {
                    Download(data);
                }

            }
            return CefReturnValue.Continue;
        }

        public bool OnCertificateError(IWebBrowser browser, CefErrorCode errorCode, string requestUrl) { return true; }
        public void OnPluginCrashed(IWebBrowser browser, string pluginPath) { }
        public void OnRenderProcessTerminated(IWebBrowser browser, CefTerminationStatus status) { }

       

        MyClient client = new MyClient();


        public void InitializeCef()
        {
            string firsturl = homeurl;
            firsturl = "http://www.useragentstring.com/";

            //firsturl = "http://www.tv-asahi.co.jp/douga/shinovani/22";
            //firsturl = "http://ja.twitcasting.tv/?genre=girls_jcjk_jp";
            //firsturl = "http://www.nhk.or.jp/ashita/hamahaha/anagodon.html";
            //firsturl = "http://www.ustream.tv/recorded/71903408";


            var settings = new CefSettings();
            //settings.UserAgent = "Mozilla/5.0 (iPhone; U; CPU iPhone OS 3_0 like Mac OS X; en-us) AppleWebKit/528.18 (KHTML, like Gecko) Version/4.0 Mobile/7A341 Safari/528.16";
            settings.CefCommandLineArgs.Add("debug-plugin-loading", "debug-plugin-loading");
            Cef.Initialize(settings);

         

            browser = new ChromiumWebBrowser(firsturl)
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

            //var bitness = Environment.Is64BitProcess ? "x64" : "x86";
            //var version = String.Format("Chromium: {0}, CEF: {1}, CefSharp: {2}, Environment: {3}", Cef.ChromiumVersion, Cef.CefVersion, Cef.CefSharpVersion, bitness);
            //DisplayOutput(version);
        }




        public BrowserForm()
        {

            this.FormClosing += BrowserForm_FormClosing;
            this.Load += BrowserForm_Load;

            InitializeComponent();

            Text = "Link Extractor";
            WindowState = FormWindowState.Maximized;

            InitializeCef();


            new Thread(() =>
            {

                client.Connect();

            }).Start();

            //MainWindow wpfwindow = new MainWindow();
            //ElementHost.EnableModelessKeyboardInterop(wpfwindow);
            //wpfwindow.Show();
        }

        private void BrowserForm_Load(object sender, EventArgs e)
        {
            Bookmark.LoadFile();
            toolStripComboBox1.ComboBox.DataSource = Bookmark.bookmarks;
            toolStripComboBox1.SelectedIndexChanged += ToolStripComboBox1_SelectedIndexChanged;

        }

        private void ToolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            var url = toolStripComboBox1.Text;
            url = Bookmark.GetUrl(url);
            LoadUrl(url);
        }

        private void BrowserForm_FormClosing(object sender, FormClosingEventArgs e)
        {

            Bookmark.SaveFile();
            SizeGettor.Clear();
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
            SavedSilverLight = "";



            this.InvokeOnUiThreadIfRequired(() => DeleteLiveData());


            //if (domain.Contains("ustream.tv"))
            //{
            //    DownloadData data = new DownloadData
            //    {
            //        URL = "",
            //        Title =  "",
            //        Folder = domain,
            //        Protocol = "LIVE",
            //        Extension = ""
            //    };

            //    this.InvokeOnUiThreadIfRequired(() => SetLiveData(data));
            //}


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


            if (executing == true)
            {
                return;
            }
            executing = true;

            try
            {


                this.Invoke(new Action(() => browser.ExecuteScriptAsync(richTextBox2.Text)));



                new Thread(() =>
                {
                    Thread.Sleep(100);
                    this.Invoke(new Action(() => GetLinks()));

                }).Start();


            }
            catch
            {

            }

            new Thread(() =>
            {
                Thread.Sleep(100);
                executing = false;

            }).Start();
        }

        public void GetLinks()
        {
            try
            {

                var task = browser.GetSourceAsync();
                //task.Start();
                task.Wait(1000);

                if (task.IsCompleted == false)
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




                    bool found = false;
                    string protocol = "HTTP";
                    string ext = Util.GetExtension(url);
                    string newextension = "";

                    if (Util.IsLiveURL(url))
                    {
                        protocol = "LIVE";
                        //newextension = "ts";
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
                            Extension = newextension,

                        };


                        if (protocol == "LIVE")
                        {
                            this.InvokeOnUiThreadIfRequired(() => SetLiveData(data));
                        }
                        else
                        {
                            if (checkBox3.Checked == false)
                            {
                                Download(data);
                            }
                        }

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
                    if ((Util.IsMovie(ext) || Util.IsPicture(ext)) && checkBox3.Checked == false)
                    {
                        DownloadData data = new DownloadData
                        {
                            URL = url,
                            Title = this.Text,
                            Folder = domain,
                            Protocol = "HTTP",
                            Extension = "",

                        };


                        Download(data);
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

        public void LiveDownload()
        {
            if (livedata != null)
            {
                livedata.Extension = textBox3.Text;
                livedata.Title = this.Text;

                new Thread(() =>
                {
                    client.Add(livedata);

                }).Start();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            LiveDownload();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var visitor = new CookieMonster(all_cookies =>
            {
                var sb = new StringBuilder();
                foreach (var nameValue in all_cookies)
                    sb.AppendLine(nameValue.Item1 + " = " + nameValue.Item2);
                BeginInvoke(new MethodInvoker(() =>
                {
                    MessageBox.Show(sb.ToString());
                }));
            });
            Cef.VisitAllCookies(visitor);
        }

        class CookieMonster : ICookieVisitor
        {
            readonly List<Tuple<string, string>> cookies = new List<Tuple<string, string>>();
            readonly Action<IEnumerable<Tuple<string, string>>> useAllCookies;

            public CookieMonster(Action<IEnumerable<Tuple<string, string>>> useAllCookies)
            {
                this.useAllCookies = useAllCookies;
            }

            public bool Visit(Cookie cookie, int count, int total, ref bool deleteCookie)
            {
                cookies.Add(new Tuple<string, string>(cookie.Name, cookie.Value));

                if (count == total - 1)
                    useAllCookies(cookies);

                return true;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            LoadUrl(Bookmark.GetUrl(toolStripComboBox1.Text));
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            LoadUrl(homeurl);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Bookmark.Add(browser.Title, fullurl);
        }
    }
}

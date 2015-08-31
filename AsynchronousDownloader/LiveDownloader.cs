using AsynchronousDownloader.Model;
using MyUtil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AsynchronousDownloader
{
 

    public partial class LiveDownloader : Form
    {
        public LiveDownloader()
        {
            InitializeComponent();

            this.FormClosing += Form1_FormClosing;
            timer1.Interval = 60 * 1000;
        }

        bool exited = false;
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            label1.Text = "動画ヘッダを処理しています。しばらくお待ちください。";
            if (downloadfile.DownloadStatus == DownloadStatus.LiveDownloading)
            {
                downloadfile.DownloadStatus = DownloadStatus.Downloaded;
            }

            new Thread(() =>
            {
                //MessageBox.Show("テスト");
               
                StopRecording();

                    Console.WriteLine("waiting...");

                try
                {
                    sw.Close();

                }
                catch
                {

                }

                try
                {
                    p.WaitForExit(60 * 1000);

                    //if (exited == false)
                    //{
                    //    MessageBox.Show("ffmpegが応答しません。強制終了させます。");
                    //}

                    p.Close();
                }
                catch
                {
                    //MessageBox.Show("プロセスの終了に失敗しました。");
                }



            }).Start();

        }

        private void P_Exited(object sender, EventArgs e)
        {

            Console.WriteLine("Exit");
            exited = true;
        }

        public System.Diagnostics.Process p;
        public System.IO.StreamWriter sw;
        public DownloadFile downloadfile;
        public string savedfilename = "";
        public string savedlocation = "";

        public void LiveRecord(string filename, DownloadFile file, string extension = "")
        {
            this.Text = file.Title;
            label1.Text = "ダウンロードを開始します。このメッセージが変わらない場合はエラーだと思われます。";

            downloadfile = file;
            savedfilename = filename;

            new Thread(() =>
            {
                LiveRecord0(filename, file);

            }).Start();

            this.TopMost = true;
            this.BringToFront();
        }

        public void LiveRecord0(string filename, DownloadFile file, string extension = "")
        {
            if(extension == "")
            {
                extension = file.Extension;
            }

            string filename2 = DateTime.Now.ToShortDateString() + "-" + DateTime.Now.ToShortTimeString() + "_" + DateTime.Now.Second + "." + extension;
            filename2 = Util.MakeValidFileName(filename2);
            savedlocation = filename + filename2;

            string url = file.Uri;
            string argument = "ffmpeg -c copy \"" + filename + filename2 + "\" -i " + url;
            //Processオブジェクトを作成
            p = new System.Diagnostics.Process();
            exited = false;
            p.Exited += P_Exited;

            //出力とエラーをストリームに書き込むようにする
            p.StartInfo.UseShellExecute = false;

            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            //OutputDataReceivedとErrorDataReceivedイベントハンドラを追加
            p.OutputDataReceived += p_OutputDataReceived;
            p.ErrorDataReceived += p_ErrorDataReceived;

            p.StartInfo.FileName =
                System.Environment.GetEnvironmentVariable("ComSpec");
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.CreateNoWindow = true;
            //p.StartInfo.Arguments = argument;
            //p.StandardInput.Encoding = Encoding.Unicode;
            //起動
            p.Start();

            //非同期で出力とエラーの読み取りを開始
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();



            sw = p.StandardInput;
            if (sw.BaseStream.CanWrite)
            {
                //「dir c:\ /w」を実行する
                sw.WriteLine(argument);

            }


            return;
        }


        public void StopRecording()
        {
            try
            {

                sw.WriteLine("q");
            }
            catch
            {

            }

        }


        //OutputDataReceivedイベントハンドラ
        //行が出力されるたびに呼び出される
         void p_OutputDataReceived(object sender,
            System.Diagnostics.DataReceivedEventArgs e)
        {
            try
            {
                if (e.Data.Trim() != "")
                    this.InvokeOnUiThreadIfRequired(() => label1.Text = e.Data);

                if (!e.Data.Contains(" ") && e.Data.Last() == '>')
                {
                    this.Close();
                }

                //エラー出力された文字列を表示する
                Console.WriteLine(e.Data);
            }
            catch
            {

            }
        }

        bool showerror = true;
        public void ErrorMessage(string text, DownloadStatus status = DownloadStatus.Error)
        {
            downloadfile.DownloadStatus = status;
            this.InvokeOnUiThreadIfRequired(() => this.Close());

            if (showerror == true)
            {
                showerror = false;
                MessageBox.Show(new Form { TopMost = true }, text + "(" + downloadfile.Title + ")");
            }
        }
        //ErrorDataReceivedイベントハンドラ
         void p_ErrorDataReceived(object sender,
            System.Diagnostics.DataReceivedEventArgs e)
        {
            try
            {
                if (e.Data.Trim() != "")
                    this.InvokeOnUiThreadIfRequired(() => label1.Text = e.Data);

                string message = e.Data.ToLower();
                if (message.Contains("invalid argument"))
                {
                    ErrorMessage("コマンドエラー。アドレスが間違っているか、保存拡張子が不正な可能性があります。");

                }
                if (message.Contains("conversion failed"))
                {
                    Util.DeleteFile(savedlocation);

                    if (Util.GetExtension(savedlocation) != "aac")
                    {
                        sw.Close();
                        p.WaitForExit(3 * 1000);
                        p.Close();

                        new Thread(() =>
                        {
                            LiveRecord0(savedfilename, downloadfile, "aac");

                        }).Start();
                    }
                    else
                    {
                        ErrorMessage("保存に失敗しました。動画形式が対応していないか、拡張子が間違っていると思われます。");
                    }
                    


                }
                if (message.Contains("http error"))
                {
                    //ErrorMessage("サーバーがエラーを返しました。動画が存在しないか、アクセスを拒否された可能性があります。");

                    sw.Close();
                    p.WaitForExit(3 * 1000);
                    p.Close();

                    this.InvokeOnUiThreadIfRequired(() =>
                    {
                        timer1.Start();
                    }
                    
                    );

                }
                else if(message.Contains("muxing overhead"))
                {
                    //ErrorMessage("動画が終了しました。", DownloadStatus.Downloaded);

                    sw.Close();
                    p.WaitForExit(60 * 1000);
                    p.Close();

                    this.InvokeOnUiThreadIfRequired(() =>
                    {
                        timer1.Start();
                    }
                    );

                }
                //エラー出力された文字列を表示する
                Console.WriteLine("ERR>{0}", e.Data);
            }
            catch
            {

            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            showerror = false;
            this.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();

            new Thread(() =>
            {
                LiveRecord0(savedfilename, downloadfile);

            }).Start();
        }
    }

    public static class ControlExtensions
    {
        /// <summary>
        /// Executes the Action asynchronously on the UI thread, does not block execution on the calling thread.
        /// </summary>
        /// <param name="control">the control for which the update is required</param>
        /// <param name="action">action to be performed on the control</param>
        public static void InvokeOnUiThreadIfRequired(this Control control, Action action)
        {
            if (control.InvokeRequired)
            {
                control.BeginInvoke(action);
            }
            else
            {
                action.Invoke();
            }
        }
    }
}

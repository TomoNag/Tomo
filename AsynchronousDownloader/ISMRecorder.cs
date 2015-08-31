﻿using AsynchronousDownloader.Model;
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
using System.Windows;
using System.Windows.Forms;

namespace AsynchronousDownloader
{
 

    public partial class ISMRecorder : Form
    {
        static string directory = "DownloadCache";

        public static void Clear()
        {
            Util.DeleteAllFiles(directory);
        }

        public ISMRecorder()
        {
            InitializeComponent();

            this.FormClosing += Form1_FormClosing;
            timer1.Interval = 60 * 1000;
        }

        bool exited = false;
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            label1.Text = "動画ヘッダを処理しています。しばらくお待ちください。";
            if (downloadfile.DownloadStatus == DownloadStatus.SilverlightDownloading)
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
                    //System.Windows.MessageBox.Show("プロセスの終了に失敗しました。");
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
        public string templocation = "";



        public void Record(string filename, DownloadFile file, string extension = "")
        {
            this.Text = file.Title;
            label1.Text = "ダウンロードを開始します。このメッセージが変わらない場合はエラーだと思われます。";

            downloadfile = file;
            savedfilename = filename;

            new Thread(() =>
            {
                Record0(filename, file);

            }).Start();

            this.TopMost = true;
            this.BringToFront();
        }

        public void Record0(string filename, DownloadFile file, string extension = "")
        {
            if (extension == "")
            {
                extension = file.Extension;
            }

            string filename2 = Util.RemoveExtension(filename) + "." + extension;
            //filename2 = Util.MakeValidFileName(filename2);
            savedlocation = filename2;


            System.IO.Directory.CreateDirectory(directory);

            templocation = directory + "\\" + Guid.NewGuid() + ".mp4";

            string url = file.Uri;
            string argument = "ismdownloader \"" + url + "\" \"" + templocation + "\"";

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

               // sw.WriteLine("q");
            }
            catch
            {

            }

        }

        public void MessageHandler(string message)
        {
            if (message.Contains("invalid argument"))
            {
                ErrorMessage("コマンドエラー。アドレスが間違っているか、保存拡張子が不正な可能性があります。");

            }
            else if (message.Contains("mux completed"))
            {
                if (System.IO.File.Exists(savedlocation))
                {
                    if (System.Windows.MessageBox.Show("既に存在するファイルを削除してもよろしいですか?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    {

                    }
                    else
                    {
                        System.IO.File.Delete(savedlocation);
                    }
                }



                try
                {
                    System.IO.File.Move(templocation, savedlocation);
                }
                catch
                {

                }

                this.InvokeOnUiThreadIfRequired(() => this.Close());
               

            }

            //エラー出力された文字列を表示する
            Console.WriteLine("ERR>{0}", message);
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

                string message = e.Data.ToLower();
                MessageHandler(message);

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
                System.Windows.Forms.MessageBox.Show(new Form { TopMost = true }, text + "(" + downloadfile.Title + ")");
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
                MessageHandler(message);

            }
            catch
            {

            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            showerror = false;

            downloadfile.DownloadStatus = DownloadStatus.Canceled;
            this.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();

        }
    }

 
}

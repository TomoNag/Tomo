using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Threading;

namespace AsynchronousDownloader
{
    public class LiveRecorder
    {
        public System.Diagnostics.Process p;

        public void LiveRecord(string filename, string url)
        {

            string argument = "ffmpeg -c copy \"" + filename + ".avi" + "\" -i " + url;
            //Processオブジェクトを作成
            p = new System.Diagnostics.Process();
            
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
            p.StartInfo.CreateNoWindow = false;
            //p.StartInfo.Arguments = argument;
            //p.StandardInput.Encoding = Encoding.Unicode;
            //起動
            p.Start();

            //非同期で出力とエラーの読み取りを開始
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();



            System.IO.StreamWriter sw = p.StandardInput;
            if (sw.BaseStream.CanWrite)
            {
                //「dir c:\ /w」を実行する
                sw.WriteLine(argument);
               
            }
            sw.Close();
            
      
            return;
        }


        public void StopRecording()
        {
            System.IO.StreamWriter sw = p.StandardInput;
            if (sw.BaseStream.CanWrite)
            {
                sw.WriteLine("q");
            }
            sw.Close();

            p.WaitForExit();
            p.Close();
        }
        //OutputDataReceivedイベントハンドラ
        //行が出力されるたびに呼び出される
        static void p_OutputDataReceived(object sender,
            System.Diagnostics.DataReceivedEventArgs e)
        {
            //出力された文字列を表示する
            Console.WriteLine(e.Data);
        }

        //ErrorDataReceivedイベントハンドラ
        static void p_ErrorDataReceived(object sender,
            System.Diagnostics.DataReceivedEventArgs e)
        {
            //エラー出力された文字列を表示する
            Console.WriteLine("ERR>{0}", e.Data);
        }

    }
}

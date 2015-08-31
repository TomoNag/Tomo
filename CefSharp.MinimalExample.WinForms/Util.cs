using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;


//using mshtml;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Web;

namespace MyUtil
{
    public class Util
    {

        public static String[] movies = { "mp4", "webm", "ogv", "mov", "wmv", "flv", "mkv", "avi", "mpeg", "ogm", "rm", "divx" };
        public static String[] pictures = { "png", "jpg" , "jpeg" };
        public static String[] lives = { "m3u8"};
        public static String[] livedomains = { "twitcasting.tv", "ustream.tv" };

        public static bool IsLiveURL(string url)
        {
            foreach (var item in lives)
            {
                if (url.Contains("."+item))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsLiveDomain(string domain)
        {
            return false;

            foreach (var item in livedomains)
            {
                if (domain == item)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsLive(string ext)
        {
            foreach (var item in lives)
            {
                if (ext == item)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsMovie(string ext)
        {
            foreach (var item in movies)
            {
                if (ext == item)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsPicture(string ext)
        {
            foreach (var item in pictures)
            {
                if (ext == item)
                {
                    return true;
                }
            }

            return false;
        }

        public static string MakeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
        }

        public static string GetExtension(string url)
        {

            if(url.Contains("?"))
            {
                int num = url.IndexOf("?");

                url = url.Substring(0, num);

              
            }

            url = url.Split('/').Last();

            if (url.Contains("."))
            {
                int num = url.LastIndexOf(".");

                url = url.Substring(num + 1);
            }
            else
            {
                return "";
            }

            url = url.ToLower();

            return url;

        }

        public static string RemoveExtension(string url)
        {

            //if (url.Contains("?"))
            //{
            //    int num = url.IndexOf("?");

            //    url = url.Substring(0, num);


            //}

            //url = url.Split('/').Last();

            if (url.Contains("."))
            {
                int num = url.LastIndexOf(".");

                url = url.Substring(0, num);
            }
           
            return url;

        }



        public static void DeleteFile(string filepath)
        {
            try
            {
                File.Delete(filepath);
            }
            catch
            {

            }
        }

        public static void DeleteAllFiles(string directorypath)
        {
            var list = GetFiles(directorypath);

            foreach (var item in list)
            {
                try
                {
                    File.Delete(item);
                }
                catch
                {

                }
            }
          
        }

        public static List<String> GetFiles(string path)
        {
           
            if (System.IO.Directory.Exists(path))
            {
                string[] files = System.IO.Directory.GetFiles(path, "*", System.IO.SearchOption.AllDirectories);
                return files.ToList();
            }

            return new List<string>();
        }
        //Shift-JIS に決め打ち
        public static string GetHtml(string url, Encoding encode = null)
        {
            if(encode == null)
            {
                encode = Encoding.GetEncoding("shift_jis");
            }

            string data = "";
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            StreamReader readStream = null;

            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                response = (HttpWebResponse)request.GetResponse();
               

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream receiveStream = response.GetResponseStream();
                 

                    //if (response.CharacterSet == null)
                    //{
                    //    readStream = new StreamReader(receiveStream);
                    //}
                    //else
                    //{
                    //    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                    //}

                    readStream = new StreamReader(receiveStream, encode);

                    data = readStream.ReadToEnd();


                }

                response.Close();
                readStream.Close();
            }
            catch
            {

            }
           
            

            return data;
        }
        //public static Dictionary<string, string> BigCamera(string html)
        //{

        //}
        public static string HtmlDecode(string html)
        {
            return System.Net.WebUtility.HtmlDecode(html);


        }
        //public static string DeleteTag(string html)
        //{
        //    string anchor = @"<.*?>";
        //    return Replace(html, anchor, "");

        //}

        public static int ToInt(string orig)
        {
            string str = Replace(orig, "[^0-9]*", "");
            if(str == "")
            {
                return 0;
            }
            return int.Parse(str);
        }

        public static double ToDouble(string orig)
        {
            string str = Replace(orig, @"[^0-9\.]*", "");
            if (str == "")
            {
                return 0;
            }
            return double.Parse(str);
        }

        public static string Analyze(string html, string anchor)
        {
            if (html == null)
                return "";

            Regex re = new Regex(anchor, RegexOptions.IgnoreCase | RegexOptions.Singleline);

            for (Match m = re.Match(html); m.Success; m = m.NextMatch())
            {

                string value = m.Groups["value"].Value;

                return value;
            }

            return "";
        }

        public static List<string> Analyze2(string html, string anchor)
        {
            List<string> list = new List<string>();
            Regex re = new Regex(anchor, RegexOptions.IgnoreCase | RegexOptions.Singleline);

            for (Match m = re.Match(html); m.Success; m = m.NextMatch())
            {

                string value = m.Groups["value"].Value;
                list.Add(value);
            }
            return list;
        }

        static public string Replace(string text, string from, string to)
        {
            return System.Text.RegularExpressions.Regex.Replace(text, from, to, System.Text.RegularExpressions.RegexOptions.Multiline);
        }

        static public string DeleteTag(string text)
        {
            return Replace(text, "<(\"[^\"]*\"|'[^']*'|[^'\">])*>", "");
        }

        public static string DeletePercentage(string text)
        {
            text = Replace(text, "[0-9]*％", "");
            return Replace(text, "[0-9]*\\%", "");

        }

        static public System.Net.WebClient wc = new System.Net.WebClient();
        static public void Access(string url)
        {
            //WebRequestの作成
            System.Net.HttpWebRequest webreq =
                (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);

            //サーバーからの応答を受信するためのWebResponseを取得
            System.Net.HttpWebResponse webres =
                (System.Net.HttpWebResponse)webreq.GetResponse();
        }

   
        static public bool DownFile(string url, string filename, bool overwrite = true)
        {
            if (File.Exists(filename) && overwrite == false)
            {
                return false;
            }
            try
            {
                wc.DownloadFile(url, filename);
            }
            catch
            {
                //return false;
            }

            // wc.Dispose();

            return true;
        }

        public static void SaveDict(Dictionary<string, string> dict, string file)
        {
            StreamWriter writer =
                           new StreamWriter(file, false, Encoding.Unicode);
            foreach (var item in dict)
            {
                writer.WriteLine(item.Key + "\t" + item.Value);
            }
            writer.Close();
        }

        public static void SaveList(List<string> list, string file)
        {
            StreamWriter writer =
                           new StreamWriter(file, false);
            foreach (var item in list)
            {
                writer.WriteLine(item);
            }
            writer.Close();
        }

        public static void SaveListUnique(List<string> list, string file)
        {
            var listunique = list.Distinct().ToList();
            listunique.Sort();


            StreamWriter writer =
                           new StreamWriter(file, false, Encoding.Unicode);
            foreach (var item in listunique)
            {
                writer.WriteLine(item);
            }
            writer.Close();
        }

        public static void SaveText(string text, string file)
        {
            StreamWriter writer =
                           new StreamWriter(file, false);
          
               writer.Write(text);
            
            writer.Close();
        }

        public static void SaveTextUTF8(string text, string file)
        {
            StreamWriter writer =
                           new StreamWriter(file, false, Encoding.UTF8);

            writer.Write(text);

            writer.Close();
        }

        public static void SaveLine(string text, string file)
        {
            StreamWriter writer =
                           new StreamWriter(file,true);

            writer.WriteLine(text);

            writer.Close();
        }
        public static string Translate(Dictionary<string, string> dict, string text)
        {

            foreach (var item in dict)
            {
                if (item.Value == "XXX" || item.Key == "")
                {
                    continue;
                }

                text = text.Replace(item.Key, item.Value);
            }

            return text;
        }

        public static string TranslateExact(Dictionary<string, string> dict, string text)
        {

            foreach (var item in dict)
            {
                if (text == item.Key)
                {
                    return item.Value;
                }

            }

            return text;
        }

        public static Dictionary<string, string> LoadDict(string file)
        {

            Dictionary<string, string> dict = new Dictionary<string, string>();



            System.IO.StreamReader sr = new System.IO.StreamReader(file, Encoding.Unicode);

            while (sr.Peek() > -1)
            {
                string text = sr.ReadLine();
                if(text.Length == 0)
                {
                    continue;
                }

               
                int num = text.LastIndexOf("\t");
                if (num == text.Length - 1)
                {
                    text = text.Substring(0, num);
                    num = text.LastIndexOf("\t");
                }

                if (num == -1)
                    continue;

                string value = text.Substring(num + 1);
                string key = text.Substring(0, num);
                dict.Add(key, value);

            }
            //閉じる
            sr.Close();

            return dict;
        }

        public static Dictionary<string, string> LoadDict2(string file, Dictionary<string, bool> isgood)
        {

            Dictionary<string, string> dict = new Dictionary<string, string>();



            System.IO.StreamReader sr = new System.IO.StreamReader(file, Encoding.Unicode);

            while (sr.Peek() > -1)
            {
                string text = sr.ReadLine();
                if (text.Length == 0)
                {
                    continue;
                }

                bool good = false;
                if (text.Length >= 2)
                {
                    int num2 = text.Length;
                    if (text[num2 - 1] == '○' && text[num2 - 2] == '\t')
                    {
                        text = text.Substring(0, num2 - 2);
                        good = true;
                    }
                }


                int num = text.LastIndexOf("\t");
                if (num == text.Length - 1)
                {
                    text = text.Substring(0, num);
                    num = text.LastIndexOf("\t");
                }

                if (num == -1)
                    continue;

                string value = text.Substring(num + 1);
                string key = text.Substring(0, num);
                dict.Add(key, value);
                isgood.Add(key, good);

            }
            //閉じる
            sr.Close();

            return dict;
        }

        public static Dictionary<string, string> LoadDictOld(string file)
        {

            Dictionary<string, string> dict = new Dictionary<string, string>();

            try
            {

                System.IO.StreamReader sr = new System.IO.StreamReader(file, Encoding.Unicode);

                string temp = FileToText(file);

                string anchor = "(?<key>.*)\t(?<value>.*?)$"; //最長一致、もしくは$で最後を指定しないとvalueが空文字になる。
                Regex re = new Regex(anchor, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                //内容を一行ずつ読み込む
                while (sr.Peek() > -1)
                {
                    string text = sr.ReadLine();
                   
                    for (Match m = re.Match(text); m.Success; m = m.NextMatch())
                    {

                        string value = m.Groups["value"].Value;
                        string key = m.Groups["key"].Value;
                        dict.Add(key, value);
                        break;
                    }

                }
                //閉じる
                sr.Close();
            }
            catch
            {
            }
            return dict;
        }

        public static List<string> FileToList(string file)
        {
            List<string> list = new List<string>();
            System.IO.StreamReader sr = new System.IO.StreamReader(file);

            //内容を一行ずつ読み込む
            while (sr.Peek() > -1)
            {
                list.Add(sr.ReadLine());
            }
            //閉じる
            sr.Close();

            return list;
        }


        public static string FileToText(string file)
        {
            string text = "";

            try
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(file);
                text = sr.ReadToEnd();
                //閉じる
                sr.Close();
            }
            catch
            {
            }

            return text;
        }

        public static string FileToTextUTF8(string file)
        {
            string text = "";

            try
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(file, Encoding.UTF8);
                text = sr.ReadToEnd();
                //閉じる
                sr.Close();
            }
            catch
            {
            }

            return text;
        }


        public static string FileToTextSjis(string file)
        {
            string text = "";

            try
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(file, System.Text.Encoding.GetEncoding("shift_jis"));
                text = sr.ReadToEnd();
                //閉じる
                sr.Close();
            }
            catch
            {
            }

            return text;
        }
        public static string ListToString(List<string> list, string separator = "\n")
        {
            string text = "";
            foreach (var item in list)
            {
                if (text != "")
                {
                    text += separator;
                }
                text += item;
            }

            return text;
        }
    }
}

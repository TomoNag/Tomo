using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MyUtil;
using IPCTestServer;

namespace CefSharp.MinimalExample.WinForms
{
    public class ULIZA
    {
        public static List<DownloadData> Uliza(string json, string domain)
        {
            var list = new List<DownloadData>();
            ULIZAINFO info = JsonConvert.DeserializeObject<ULIZAINFO>(json);

            string url = info.NET_CONN;
            string title = info.TITLE;
            string dir = info.STORAGESUBDIR;

            foreach (var item in info.PLAYLIST)
            {

                string filename = item.NAME;
                string ext = Util.GetExtension(filename);

                string T = url;
                string Y = String.Format("{0}:{1}/{2}", ext, dir, filename);


                DownloadData data = new DownloadData
                {
                    URL = url,                  
                    Title = title,
                    Folder = domain,
                    Protocol = "RTMP",
                    Extension = "",

                    RTMP_T = T,
                    RTMP_Y = Y,
                };

                list.Add(data);
            }
            //rtmpdump -r "rtmp://flv.nhk.or.jp/ondemand/flv/mp4:nhk-mov/7b1527cb9377660aa81d869c284e73dd-1_multi_464000_v1.mp4"  -s "https://aka-secure-img.uliza.jp/Player/2417/player.swf?d=20150416" -t "rtmp://flv.nhk.or.jp/ondemand/flv/" -y "mp4:nhk-mov/7b1527cb9377660aa81d869c284e73dd-1_multi_464000_v1.mp4" -o filename.mp4

            return list;
        }


        public class PLAYLIST
        {
            public string NAME { get; set; }
            public string START { get; set; }
            public string LEN { get; set; }
            public string RESET { get; set; }
            public string LOG_PARAMS { get; set; }
        }

        public class ULIZAINFO
        {
            public string TITLE { get; set; }
            public string IMAGE_BEFORE_STREAM { get; set; }
            public string IMAGE_THUMNAIL { get; set; }
            public string IMAGE_BANNER { get; set; }
            public string FACEBOOK_TXT { get; set; }
            public string TWITTER_TXT { get; set; }
            public string LIVEDOOR_TXT { get; set; }
            public string DELICIOUSE_TXT { get; set; }
            public string GOOGLE_TXT { get; set; }
            public string YAHOO_TXT { get; set; }
            public string DESCRIPTION { get; set; }
            public string NETWORK_ID { get; set; }
            public string NET_CONN { get; set; }
            public string USEBEACON_FLAG { get; set; }
            public string BEACONRECEIVE_URL { get; set; }
            public string BEACONPING_INTERVAL_SEC { get; set; }
            public string BEACON_TYPE { get; set; }
            public string LINK_URL { get; set; }
            public string LINK_TEXT { get; set; }
            public List<PLAYLIST> PLAYLIST { get; set; }
            public string SITE_ID { get; set; }
            public string SITE_URL { get; set; }
            public string GENRE_ID { get; set; }
            public string GENRE_CODE { get; set; }
            public string PROGRAM_ID { get; set; }
            public string PROGRAM_CODE { get; set; }
            public string EPISODE_ID { get; set; }
            public string VIDEO_ID { get; set; }
            public string MEMBER_SITE_ID { get; set; }
            public string VIDEO_CLICK_URL { get; set; }
            public string CDNID { get; set; }
            public string STORAGESUBDIR { get; set; }
            public string KEY { get; set; }
            public string STIME { get; set; }
            public string USERID { get; set; }
            public string BANDWIDTH { get; set; }
            public string SHOW_LITTLE_FLAG { get; set; }
            public string SHOW_LITTLE_START_TIME { get; set; }
            public string SHOW_LITTLE_END_TIME { get; set; }
            public List<object> DYNAMIC_STREAMING { get; set; }
            public List<object> LIVE_STREAMING { get; set; }
            public List<object> CHAPTER { get; set; }
        }
    }
}

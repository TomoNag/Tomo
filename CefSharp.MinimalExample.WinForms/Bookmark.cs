using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MyUtil;
using System.IO;
using System.ComponentModel;

namespace CefSharp.MinimalExample.WinForms
{
    public class Bookmark
    {
        public static BindingList<string> bookmarks;

        static string dirname = "Setting";
        static string filename = "Boookmark.txt";
        static char splitter = '　';
        public static string GetUrl(string item)
        {
            if(item.Contains(splitter))
            {
                return item.Split(splitter)[1];
            }

            return item;
        }

        public static void LoadFile()
        {
            try
            {
                bookmarks = new BindingList<string>(Util.FileToList(dirname + "\\" + filename));
            }
            catch
            {
                bookmarks = new BindingList<string>();
            }
        }

        public static void SaveFile()
        {

            Directory.CreateDirectory(dirname);

            Util.SaveList(bookmarks.ToList(), dirname + "\\" + filename);
        }


        public static void Add(string title, string url)
        {
            title = title.Replace(splitter, ' ');
            bookmarks.Add(title + splitter + url);
        }

    }
}

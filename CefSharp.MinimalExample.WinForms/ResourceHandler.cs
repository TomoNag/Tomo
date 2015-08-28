//using System;
//using System.Windows.Forms;
//using CefSharp.MinimalExample.WinForms.Controls;
//using CefSharp.WinForms;

//using System.Windows.Forms.Integration; //Not so Given.
//using IPCTestServer;
//using System.Threading;
//using HtmlAgilityPack;
//using System.Windows.Threading;
//using MyUtil;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Text;
//using System.IO;

//namespace CefSharp.MinimalExample.WinForms
//{

//    class MyResourceHandlerFactory : IResourceHandlerFactory
//    {

//        public void RegisterHandler(string url, ResourceHandler handler) { }
//        public void UnregisterHandler(string url) { }

//        public bool HasHandlers
//        {
//            get { return true; }
//        }

//        public ResourceHandler GetResourceHandler(IWebBrowser browserControl, IRequest request)
//        {
//            if (request.Url.StartsWith("http"))
//                return new MyResourceHandler();
//            return null;
//        }
//    }



//    class MyResourceHandler : ResourceHandler
//    {
       

//        private HttpWebRequest webRequest;
//        private HttpWebResponse webResponse;
//        private Stream requestStream;
//        private byte[] requestBytes;
//        ICallback callback;

//        public Stream GetResponse(IResponse response, out long responseLength, out string redirectUrl)
//        {
//            responseLength = webResponse.ContentLength;
//            redirectUrl = null;
//            string type = webResponse.ContentType;
//            // strip off the encoding, if present
//            if (type.IndexOf("; ") > 0)
//                type = type.Substring(0, type.IndexOf("; "));
//            response.MimeType = type;
//            // only a direct assignment works here, don't try to use other methods of the name/value collection;
//            response.ResponseHeaders = webResponse.Headers;
//            response.StatusCode = (int)webResponse.StatusCode;
//            response.StatusText = webResponse.StatusDescription;
//            // TODO return a wrapper around this stream to capture the response inline.
//            return webResponse.GetResponseStream();
//        }

//        private void SendRequestBody(IAsyncResult result)
//        {
//            requestStream = webRequest.EndGetRequestStream(result);
//            requestStream.BeginWrite(requestBytes, 0, requestBytes.Length, RequestSent, null);
//        }

//        private void RequestSent(IAsyncResult ar)
//        {
//            requestStream.EndWrite(ar);
//            requestStream.Close();
//            webRequest.BeginGetResponse(new AsyncCallback(Response), null);
//        }

//        private void Response(IAsyncResult ar)
//        {
//            webResponse = (HttpWebResponse)webRequest.EndGetResponse(ar);
//            callback.Continue();
//        }

//        public bool ProcessRequestAsync(IRequest request, ICallback callback)
//        {
//            this.callback = callback;
//            webRequest = (HttpWebRequest)WebRequest.Create(request.Url);
//            webRequest.Method = request.Method;
//            foreach (string key in request.Headers)
//            {
//                string value = request.Headers[key];
//                // HttpWebRequest doesn't like it if you try to set these values through the Headers collection
//                if ("Accept".Equals(key))
//                    webRequest.Accept = value;
//                else if ("User-Agent".Equals(key))
//                    webRequest.UserAgent = value;
//                else if ("Referer".Equals(key))
//                    webRequest.Referer = value;
//                else if ("Content-Type".Equals(key))
//                    webRequest.ContentType = value;
//                else if ("Content-Length".Equals(key))
//                    webRequest.ContentLength = Convert.ToInt32(value);
//                else
//                    webRequest.Headers.Add(key, value);
//            }
//            //if (!String.IsNullOrWhiteSpace(request.Body))
//            //{
//            //    this.requestBytes = Encoding.UTF8.GetBytes(request.Body);
//            //    webRequest.BeginGetRequestStream(new AsyncCallback(SendRequestBody), null);
//            //}
//                        //if (!String.IsNullOrWhiteSpace(request.Body))
//            //{
//            //    this.requestBytes = Encoding.UTF8.GetBytes(request.Body);
//            //    webRequest.BeginGetRequestStream(new AsyncCallback(SendRequestBody), null);
//            //}
//            //else
//            {
//                webRequest.BeginGetResponse(new AsyncCallback(Response), null);
//            }
//            return true;
//        }
//    }
//}

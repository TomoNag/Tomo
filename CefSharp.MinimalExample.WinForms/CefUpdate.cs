////
//// 概要:
////     Called when the browser needs credentials from the user.
////
//// パラメーター:
////   browserControl:
////     The ChromiumWebBrowser control
////
////   browser:
////     the browser object
////
////   frame:
////     The frame object that needs credentials (This will contain the URL that is being
////     requested.)
////
////   isProxy:
////     indicates whether the host is a proxy server
////
////   host:
////     hostname
////
////   port:
////     port number
////
////   realm:
////     realm
////
////   scheme:
////     scheme
////
////   callback:
////     Callback interface used for asynchronous continuation of authentication requests.
////
//// 戻り値:
////     Return true to continue the request and call CefAuthCallback::Continue() when
////     the authentication information is available. Return false to cancel the request.
//public bool GetAuthCredentials(IWebBrowser browserControl, IBrowser browser, IFrame frame, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback) { return true; }
////
//// 概要:
////     Called before browser navigation. If the navigation is allowed CefSharp.IWebBrowser.FrameLoadStart
////     and CefSharp.IWebBrowser.FrameLoadEnd will be called. If the navigation is canceled
////     CefSharp.IWebBrowser.LoadError will be called with an ErrorCode value of CefSharp.CefErrorCode.Aborted.
////
//// パラメーター:
////   browserControl:
////     the ChromiumWebBrowser control
////
////   browser:
////     the browser object
////
////   frame:
////     The frame the request is coming from
////
////   request:
////     the request object - cannot be modified in this callback
////
////   isRedirect:
////     has the request been redirected
////
//// 戻り値:
////     Return true to cancel the navigation or false to allow the navigation to proceed.
//public bool OnBeforeBrowse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, bool isRedirect) { return false; }
////
//// 概要:
////     Called on the browser process IO thread before a plugin is loaded.
////
//// パラメーター:
////   browserControl:
////     The ChromiumWebBrowser control
////
////   browser:
////     the browser object
////
////   url:
////     URL
////
////   policyUrl:
////     policy URL
////
////   info:
////     plugin information
////
//// 戻り値:
////     Return true to block loading of the plugin.
//public bool OnBeforePluginLoad(IWebBrowser browserControl, IBrowser browser, string url, string policyUrl, WebPluginInfo info) { return false; }
////
//// 概要:
////     Called before a resource request is loaded. For async processing return CefSharp.CefReturnValue.ContinueAsync
////     and execute CefSharp.IRequestCallback.Continue(System.Boolean) or CefSharp.IRequestCallback.Cancel
////
//// パラメーター:
////   browserControl:
////     The ChromiumWebBrowser control
////
////   browser:
////     the browser object
////
////   frame:
////     The frame object
////
////   request:
////     the request object - can be modified in this callback.
////
////   callback:
////     Callback interface used for asynchronous continuation of url requests.
////
//// 戻り値:
////     To cancel loading of the resource return CefSharp.CefReturnValue.Cancel or CefSharp.CefReturnValue.Continue
////     to allow the resource to load normally. For async return CefSharp.CefReturnValue.ContinueAsync
//public CefReturnValue OnBeforeResourceLoad(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback)
//{
//    string url = request.Url;

//    string ext = Util.GetExtension(url);
//    bool found = false;
//    string protocol = "HTTP";

//    if (Util.IsLive(ext))
//    {
//        protocol = "LIVE";
//        found = true;
//    }

//    if (Util.IsMovie(ext))
//    {
//        found = true;

//        if (Util.IsLiveDomain(domain))
//        {
//            found = false;
//        }
//    }

//    if (found == true)
//    {


//        DownloadData data = new DownloadData
//        {
//            URL = url,
//            Title = this.Text,
//            Folder = domain,
//            Protocol = protocol,

//        };


//        new Thread(() =>
//        {

//            client.Add(data);

//        }).Start();
//    }
//    return CefReturnValue.Continue;
//}
////
//// 概要:
////     Called to handle requests for URLs with an invalid SSL certificate. Return true
////     and call CefSharp.IRequestCallback.Continue(System.Boolean) either in this method
////     or at a later time to continue or cancel the request. If CefSettings.IgnoreCertificateErrors
////     is set all invalid certificates will be accepted without calling this method.
////
//// パラメーター:
////   browserControl:
////     the ChromiumWebBrowser control
////
////   browser:
////     the browser object
////
////   errorCode:
////     the error code for this invalid certificate
////
////   requestUrl:
////     the url of the request for the invalid certificate
////
////   sslInfo:
////     ssl certificate information
////
////   callback:
////     Callback interface used for asynchronous continuation of url requests. If empty
////     the error cannot be recovered from and the request will be canceled automatically.
////
//// 戻り値:
////     Return false to cancel the request immediately. Return true and use CefSharp.IRequestCallback
////     to execute in an async fashion.
//public bool OnCertificateError(IWebBrowser browserControl, IBrowser browser, CefErrorCode errorCode, string requestUrl, ISslInfo sslInfo, IRequestCallback callback) { return true; }
////
//// 概要:
////     Called on the UI thread before OnBeforeBrowse in certain limited cases where
////     navigating a new or different browser might be desirable. This includes user-initiated
////     navigation that might open in a special way (e.g. links clicked via middle-click
////     or ctrl + left-click) and certain types of cross-origin navigation initiated
////     from the renderer process (e.g. navigating the top-level frame to/from a file
////     URL).
////
//// パラメーター:
////   browserControl:
////     the ChromiumWebBrowser control
////
////   browser:
////     the browser object
////
////   frame:
////     The frame object
////
////   targetUrl:
////     target url
////
////   targetDisposition:
////     The value indicates where the user intended to navigate the browser based on
////     standard Chromium behaviors (e.g. current tab, new tab, etc).
////
////   userGesture:
////     The value will be true if the browser navigated via explicit user gesture (e.g.
////     clicking a link) or false if it navigated automatically (e.g. via the DomContentLoaded
////     event).
////
//// 戻り値:
////     Return true to cancel the navigation or false to allow the navigation to proceed
////     in the source browser's top-level frame.
//public bool OnOpenUrlFromTab(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture) { return false; }
////
//// 概要:
////     Called when a plugin has crashed
////
//// パラメーター:
////   browserControl:
////     the ChromiumWebBrowser control
////
////   browser:
////     the browser object
////
////   pluginPath:
////     path of the plugin that crashed
//public void OnPluginCrashed(IWebBrowser browserControl, IBrowser browser, string pluginPath) { }
////
//// 概要:
////     Called on the UI thread to handle requests for URLs with an unknown protocol
////     component. SECURITY WARNING: YOU SHOULD USE THIS METHOD TO ENFORCE RESTRICTIONS
////     BASED ON SCHEME, HOST OR OTHER URL ANALYSIS BEFORE ALLOWING OS EXECUTION.
////
//// パラメーター:
////   browserControl:
////     The ChromiumWebBrowser control
////
////   browser:
////     the browser object
////
////   url:
////     the request url
////
//// 戻り値:
////     return to true to attempt execution via the registered OS protocol handler, if
////     any. Otherwise return false.
//public bool OnProtocolExecution(IWebBrowser browserControl, IBrowser browser, string url) { return true; }
////
//// 概要:
////     Called when JavaScript requests a specific storage quota size via the webkitStorageInfo.requestQuota
////     function. For async processing return true and execute CefSharp.IRequestCallback.Continue(System.Boolean)
////     at a later time to grant or deny the request or CefSharp.IRequestCallback.Cancel
////     to cancel.
////
//// パラメーター:
////   browserControl:
////     The ChromiumWebBrowser control
////
////   browser:
////     the browser object
////
////   originUrl:
////     the origin of the page making the request
////
////   newSize:
////     is the requested quota size in bytes
////
////   callback:
////     Callback interface used for asynchronous continuation of url requests.
////
//// 戻り値:
////     Return false to cancel the request immediately. Return true to continue the request
////     and call CefSharp.IRequestCallback.Continue(System.Boolean) either in this method
////     or at a later time to grant or deny the request.
//public bool OnQuotaRequest(IWebBrowser browserControl, IBrowser browser, string originUrl, long newSize, IRequestCallback callback) { return true; }
////
//// 概要:
////     Called when the render process terminates unexpectedly.
////
//// パラメーター:
////   browserControl:
////     The ChromiumWebBrowser control
////
////   browser:
////     the browser object
////
////   status:
////     indicates how the process terminated.
//public void OnRenderProcessTerminated(IWebBrowser browserControl, IBrowser browser, CefTerminationStatus status) { }
////
//// 概要:
////     Called on the browser process UI thread when the render view associated with
////     |browser| is ready to receive/handle IPC messages in the render process.
////
//// パラメーター:
////   browserControl:
////     The ChromiumWebBrowser control
////
////   browser:
////     the browser object
//public void OnRenderViewReady(IWebBrowser browserControl, IBrowser browser) { }
////
//// 概要:
////     Called on the IO thread when a resource load is redirected. The CefSharp.IRequest.Url
////     parameter will contain the old URL and other request-related information.
////
//// パラメーター:
////   browserControl:
////     The ChromiumWebBrowser control
////
////   browser:
////     the browser object
////
////   frame:
////     The frame that is being redirected.
////
////   request:
////     the request object - cannot be modified in this callback
////
////   newUrl:
////     the new URL and can be changed if desired
//public void OnResourceRedirect(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, ref string newUrl) { }
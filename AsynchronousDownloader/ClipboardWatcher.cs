using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

/// <summary>
/// クリップボードを監視するクラス。
/// 使用後は必ずDispose()メソッドを呼び出して下さい。
/// </summary>
public class ClipBoardWatcher : IDisposable
{
    ClipBoardWatcherForm form;

    /// <summary>
    /// クリップボードに内容に変更があると発生します。
    /// </summary>
    public event EventHandler DrawClipBoard;

    /// <summary>
    /// ClipBoardWatcherクラスを初期化して
    /// クリップボードビューアチェインに登録します。
    /// 使用後は必ずDispose()メソッドを呼び出して下さい。
    /// </summary>
    public ClipBoardWatcher()
    {
        form = new ClipBoardWatcherForm();
        form.StartWatch(raiseDrawClipBoard);
    }

    private void raiseDrawClipBoard()
    {
        if (DrawClipBoard != null)
        {
            DrawClipBoard(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// ClipBoardWatcherクラスを
    /// クリップボードビューアチェインから削除します。
    /// </summary>
    public void Dispose()
    {
        form.Dispose();
    }

    private class ClipBoardWatcherForm : Form
    {
        [DllImport("user32.dll")]
        private static extern IntPtr SetClipboardViewer(IntPtr hwnd);
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        private static extern bool ChangeClipboardChain(IntPtr hwnd, IntPtr hWndNext);

        const int WM_DRAWCLIPBOARD = 0x0308;
        const int WM_CHANGECBCHAIN = 0x030D;

        IntPtr nextHandle;
        System.Threading.ThreadStart proc;

        public void StartWatch(System.Threading.ThreadStart proc)
        {
            this.proc = proc;
            nextHandle = SetClipboardViewer(this.Handle);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_DRAWCLIPBOARD)
            {
                SendMessage(nextHandle, m.Msg, m.WParam, m.LParam);
                proc();
            }
            else if (m.Msg == WM_CHANGECBCHAIN)
            {
                if (m.WParam == nextHandle)
                {
                    nextHandle = m.LParam;
                }
                else
                {
                    SendMessage(nextHandle, m.Msg, m.WParam, m.LParam);
                }
            }
            base.WndProc(ref m);
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                ChangeClipboardChain(this.Handle, nextHandle);
                base.Dispose(disposing);
            }
            catch
            {
            }
        }
    }
}
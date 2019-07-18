using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MyERPSecondDevTools.Common
{
    public class MessageBoxTimeOut
    {
        private string _caption;

        public void Show(string text, string caption)
        {
            Show(3000, text, caption);
        }
        public void Show(int timeout, string text, string caption)
        {
            Show(timeout, text, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public void Show(int timeout, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            this._caption = caption;
            StartTimer(timeout);
            MessageBox.Show(text, caption, buttons, icon);
        }
        private void StartTimer(int interval)
        {
            Timer timer = new Timer();
            timer.Interval = interval;
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Enabled = true;
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            KillMessageBox();
            //停止计时器
            ((Timer)sender).Enabled = false;
        }
        [DllImport("User32.dll", EntryPoint = "FindWindow", CharSet = CharSet.Auto)]
        private extern static IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        public const int WM_CLOSE = 0x10;
        private void KillMessageBox()
        {
            //查找MessageBox的弹出窗口,注意对应标题
            IntPtr ptr = FindWindow(null, this._caption);
            if (ptr != IntPtr.Zero)
            {
                //查找到窗口则关闭
                PostMessage(ptr, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            }
        }
    }
}

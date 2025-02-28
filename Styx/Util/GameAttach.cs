using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Styx.Util
{
    public class GameAttach
    {
        public static bool GameWindowAttached;
        public static int GwlStyle = -16;
        public static int WsVisible = 0x10000000;
        public static IntPtr GameWindowHandle;
        public static IntPtr OriginalWindowStyle;
        public static Rect OriginalWindowPos;

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string className, string windowTitle);

        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        public static extern bool MoveWindow(IntPtr handle, int x, int y, int w, int h, bool repaint);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hwnd, out Rect lpRect);

        internal static void Resize(ref Panel gamePanel)
        {
            if (GameWindowAttached) MoveWindow(GameWindowHandle, 0, 0, gamePanel.Width, gamePanel.Height, true);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
    }
}
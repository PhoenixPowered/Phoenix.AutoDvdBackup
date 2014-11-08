using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Phoenix.AutoDvdBackup
{
    public class DriveTools
    {
        [DllImport("winmm.dll", EntryPoint = "mciSendStringA", CharSet = CharSet.Ansi)]
        protected static extern int mciSendString(string lpstrCommand, StringBuilder lpstrReturnString, int uReturnLength, IntPtr hwndCallback);

        public static void OpenDrive()
        {
            mciSendString("set cdaudio door open", null, 0, IntPtr.Zero);
        }

        public static void CloseDrive()
        {
            mciSendString("set cdaudio door closed", null, 0, IntPtr.Zero);
        }
    }
}

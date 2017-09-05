using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace Usb.NativeMethods
{
    class Kernel32
    {
        public const Int32 FILE_ATTRIBUTE_NORMAL = 0x80;
        public const Int32 FILE_FLAG_OVERLAPPED = 0x40000000;
        public const Int32 FILE_SHARE_READ = 1;
        public const Int32 FILE_SHARE_WRITE = 2;
        public const UInt32 GENERIC_READ = 0x80000000;
        public const UInt32 GENERIC_WRITE = 0x40000000;
        public const Int32 OPEN_EXISTING = 3;

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern SafeFileHandle CreateFile(
            String lpFileName, UInt32 dwDesiredAccess, Int32 dwShareMode,
            IntPtr lpSecurityAttributes, Int32 dwCreationDisposition,
            Int32 dwFlagsAndAttributes, Int32 hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CancelIo(SafeFileHandle hFile);
    }
}

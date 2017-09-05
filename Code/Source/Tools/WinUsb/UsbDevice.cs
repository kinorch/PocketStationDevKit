using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Usb.NativeMethods;
using System.Text.RegularExpressions;
using System.Globalization;
using Microsoft.Win32.SafeHandles;

namespace Usb
{
    public class UsbDevice
    {
        public string DevicePath
        {
            get;
            protected set;
        }
        public UInt16 Vid
        {
            get;
            protected set;
        }
        public UInt16 Pid
        {
            get;
            protected set;
        }
        public Guid Guid
        {
            get;
            protected set;
        }

        public UsbDevice()
        {

        }

        public void Initialize(Guid guid, string devicePath)
        {
            this.Guid = guid;
            this.DevicePath = devicePath;
            this.Vid = ExtractVidFromDevicePath(devicePath);
            this.Pid = ExtractPidFromDevicePath(devicePath);
        }

        public SafeFileHandle Open()
        {
            return Open(this.DevicePath);
        }

        private static SafeFileHandle Open(string devicePath)
        {
            return Kernel32.CreateFile(devicePath,
                        (Kernel32.GENERIC_WRITE | Kernel32.GENERIC_READ),
                        Kernel32.FILE_SHARE_READ | Kernel32.FILE_SHARE_WRITE,
                        IntPtr.Zero,
                        Kernel32.OPEN_EXISTING,
                        Kernel32.FILE_ATTRIBUTE_NORMAL | Kernel32.FILE_FLAG_OVERLAPPED,
                        0);
        }

        private static UInt16 ExtractVidFromDevicePath(string DevicePath)
        {
            var regex = new Regex(@"vid_(?<vid>[0-9|a-f]{4}?)",RegexOptions.IgnoreCase);
            var mc = regex.Match(DevicePath);
            var vidString = mc.Groups["vid"].Value;
            short vid = 0;
            if (Int16.TryParse(vidString, System.Globalization.NumberStyles.AllowHexSpecifier, NumberFormatInfo.CurrentInfo, out vid))
            {
                return (UInt16)vid;
            }
            else
            {
                return 0;
            }
        }
        
        private static UInt16 ExtractPidFromDevicePath(string DevicePath)
        {
            var regex = new Regex(@"pid_(?<pid>[0-9|a-f]{4}?)",RegexOptions.IgnoreCase);
            var mc = regex.Match(DevicePath);
            var vidString = mc.Groups["pid"].Value;
            short pid = 0;
            if (Int16.TryParse(vidString, System.Globalization.NumberStyles.AllowHexSpecifier, NumberFormatInfo.CurrentInfo, out pid))
            {
                return (UInt16)pid;
            }
            else
            {
                return 0;
            }
        }
    }

    public static class UsbEnumerator<T> where T : UsbDevice, new()
    {
        private static IEnumerable<T> Enumerate(Guid guid)
        {
            var devices = new List<T>();
            var devicePathList = EnumerateDevicePath(guid);

            foreach (var devicePath in devicePathList)
            {
                var device = new T();
                device.Initialize(guid, devicePath);
                devices.Add(device);
            }

            return devices;
        }

        private static IEnumerable<string> EnumerateDevicePath(Guid guid)
        {
            var devicePathList = new List<string>();
            var hDevInfo = SetupApi.SetupDiGetClassDevs(ref guid, IntPtr.Zero, IntPtr.Zero, SetupApi.DIGCF_PRESENT | SetupApi.DIGCF_DEVICEINTERFACE);

            var spid = new SetupApi.SP_DEVICE_INTERFACE_DATA();
            spid.cbSize = Marshal.SizeOf(spid);
            int memberindex = 0;
            while (SetupApi.SetupDiEnumDeviceInterfaces(hDevInfo, IntPtr.Zero, ref guid, memberindex, ref spid))
            {
                int bufferSize = 0;
                SetupApi.SetupDiGetDeviceInterfaceDetail(hDevInfo, ref spid, IntPtr.Zero, 0, ref bufferSize, IntPtr.Zero);

                var buffer = Marshal.AllocHGlobal(bufferSize);
                Marshal.WriteInt32(buffer, (IntPtr.Size == 4) ? (4 + Marshal.SystemDefaultCharSize) : 8);

                var da = new SetupApi.SP_DEVINFO_DATA();
                da.cbSize = Marshal.SizeOf(da);

                SetupApi.SetupDiGetDeviceInterfaceDetail(hDevInfo, ref spid, buffer, bufferSize, ref bufferSize, ref da);
                IntPtr pDevicePathName = new IntPtr(buffer.ToInt64() + 4);
                string pathName = Marshal.PtrToStringUni(pDevicePathName);

                Marshal.FreeHGlobal(buffer);
                buffer = IntPtr.Zero;
                memberindex++;

                Console.WriteLine(pathName);
                devicePathList.Add(pathName);
            }

            return devicePathList;
        }
    }
}

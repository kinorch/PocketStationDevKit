using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Runtime.ConstrainedExecution;

namespace Usb.NativeMethods
{
    public class WinUsb
    {
        public enum USBD_PIPE_TYPE : int
        {
            Control = 0,
            Isochronous,
            Bulk,
            Interrupt,
        }

        [StructLayout(LayoutKind.Sequential)]
        struct USB_INTERFACE_DESCRIPTOR
        {
            public byte bLength;
            public byte bDescriptorType;
            public byte bInterfaceNumber;
            public byte bAlternateSetting;
            public byte bNumEndpoints;
            public byte bInterfaceClass;
            public byte bInterfaceSubClass;
            public byte bInterfaceProtocol;
            public byte iInterface;
        };

        [StructLayout(LayoutKind.Sequential)]
        struct WINUSB_PIPE_INFORMATION
        {
            public USBD_PIPE_TYPE PipeType;
            public byte PipeId;
            public ushort MaximumPacketSize;
            public byte Interval;
        }

        public class WinUsbHandle : SafeHandleMinusOneIsInvalid
        {
            public WinUsbHandle() : base(true){}

            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            protected bool ReleaseHandle()
            {
                return Free(this);
            }
        }

        public static bool Initialize(SafeFileHandle DeviceHandle, WinUsbHandle InterfaceHandle)
        {
            return WinUsb_Initialize(DeviceHandle, InterfaceHandle);
        }

        public static bool Free(WinUsbHandle InterfaceHandle)
        {
            return WinUsb_Free(InterfaceHandle);
        }

        private static bool QueryInterfaceSettings(
            WinUsbHandle InterfaceHandle, Byte AlternateInterfaceNumber, out USB_INTERFACE_DESCRIPTOR UsbAltInterfaceDescriptor)
        {
            return WinUsb_QueryInterfaceSettings(InterfaceHandle, AlternateInterfaceNumber, out UsbAltInterfaceDescriptor);
        }

        public static bool QueryPipe(
            WinUsbHandle InterfaceHandle, Byte AlternateInterfaceNumber, Byte PipeIndex, out WINUSB_PIPE_INFORMATION PipeInformation)
        {
            return WinUsb_QueryPipe(InterfaceHandle, AlternateInterfaceNumber, PipeIndex, out PipeInformation);
        }

        public static bool WritePipe(
            WinUsbHandle InterfaceHandle, byte PipeID, byte[] pBuffer, uint BufferLength, out uint LengthTransferred)
        {
            return WinUsb_WritePipe(InterfaceHandle, PipeID, pBuffer, BufferLength, out LengthTransferred, IntPtr.Zero);
        }
        
        public static bool ReadPipe(
            WinUsbHandle InterfaceHandle, byte PipeID, byte[] pBuffer, uint BufferLength, out uint LengthTransferred)
        {
            return WinUsb_ReadPipe(InterfaceHandle, PipeID, pBuffer, BufferLength, out LengthTransferred, IntPtr.Zero);
        }



        [DllImport("winusb.Dll", SetLastError = true)]
        private static extern bool WinUsb_Initialize(SafeFileHandle DeviceHandle, WinUsbHandle InterfaceHandle);

        [DllImport("winusb.Dll", SetLastError = true)]
        private static extern bool WinUsb_Free(WinUsbHandle InterfaceHandle);

        [DllImport("winusb.dll", SetLastError = true)]
        private static extern bool WinUsb_QueryPipe(
            WinUsbHandle InterfaceHandle, Byte AlternateInterfaceNumber, Byte PipeIndex, out WINUSB_PIPE_INFORMATION PipeInformation);

        [DllImport("winusb.dll", SetLastError = true)]
        private static extern bool WinUsb_WritePipe(
            WinUsbHandle InterfaceHandle, byte PipeID, byte[] pBuffer, uint BufferLength, out uint LengthTransferred, IntPtr Overlapped);
        
        [DllImport("winusb.dll", SetLastError = true)]
        private static extern bool WinUsb_ReadPipe(
            WinUsbHandle InterfaceHandle, byte PipeID, byte[] pBuffer, uint BufferLength, out uint LengthTransferred, IntPtr Overlapped);

        [DllImport("winusb.dll", SetLastError = true)]
        private static extern bool WinUsb_QueryInterfaceSettings(
            WinUsbHandle InterfaceHandle, Byte AlternateInterfaceNumber, out USB_INTERFACE_DESCRIPTOR UsbAltInterfaceDescriptor);

    }
}

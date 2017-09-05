using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryCardAdaptorTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var deviceList = Usb.UsbDevice.EnumerateDevicePath(new Guid("33E9B9F8-C444-472C-8867-76A2B23E5076"));
        }
    }
}

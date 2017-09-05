using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Usb
{
    public class MemoryCardReader : UsbDevice
    {
        public MemoryCardReader() { }
        
        public void Initialize(Guid guid, string devicePath)
        {
            base.Initialize(guid, devicePath);
        }


    }
}

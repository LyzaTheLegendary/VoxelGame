using System.Runtime.InteropServices;


namespace NativeCalls
{

    [StructLayout(LayoutKind.Sequential)]
    
    public struct MonitorStruct
    {
        private ref struct DEVMODE
        {
            public const int DM_DISPLAYFREQUENCY = 0x00400000;

            private const int CCHDEVICENAME = 32;
            private const int CCHFORMNAME = 32;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
            public string dmDeviceName;
            public ushort dmSpecVersion;
            public ushort dmDriverVersion;
            public ushort dmSize;
            public ushort dmDriverExtra;
            public uint dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public uint dmDisplayOrientation;
            public uint dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHFORMNAME)]
            public string dmFormName;
            public ushort dmLogPixels;
            public uint dmBitsPerPel;
            public uint dmPelsWidth;
            public uint dmPelsHeight;
            public uint dmDisplayFlags;
            public uint dmNup;
            public uint dmDisplayFrequency;
        }
        [DllImport("user32.dll")]
        private static extern bool EnumDisplaySettings(string? deviceName, int modeNum, ref DEVMODE devMode);

        public string Name { get; private set; }
        public int Frequency { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }
        public int Size { get; private set; }
        public MonitorStruct()
        {
            DEVMODE dm = new DEVMODE();
            dm.dmSize = (ushort)Marshal.SizeOf(typeof(DEVMODE));

            if (!EnumDisplaySettings(null, -1, ref dm))
                throw new Exception("Failed to retrieve monitor data");


            Frequency = (int)dm.dmDisplayFrequency;
            Height = (int)dm.dmPelsHeight;
            Width = (int)dm.dmPelsWidth;
            Size = dm.dmSize;
            Name = dm.dmDeviceName;
        }
    }


}



namespace Machine.CPU.Registers;

public static class ControlRegister 
{
    // Flag register (8 bits)
    public static byte Flags { get; private set; }

    public const byte INTERRUPT_ENABLE = 0x01;
    public const byte KERNEL_MODE = 0x02;
    public const byte HALT_ON_ERROR = 0x04;
    public const byte DEBUG_MODE = 0x08;
    //public const byte FLAG = 0x10;
    //public const byte FLAG = 0x20;
    public const byte VCP_CONTROL= 0x40;
    //public const byte FLAG = 0x80;

    public static void ClearFlags() => Flags = 0;

    private static void SetFlag(byte flagMask, bool value)
    {
        if (value)
            Flags |= flagMask;
        else
            Flags &= (byte)~flagMask;
    }
}


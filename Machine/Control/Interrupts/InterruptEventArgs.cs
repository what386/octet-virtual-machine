using System;

namespace Machine.Control.Interrupts;

public class InterruptEventArgs : EventArgs
{
    public byte portAddress { get; private set; }
    public byte interruptVector { get; private set; }
    
    public InterruptEventArgs(byte portAddress, byte interruptVector)
    {
        this.portAddress = portAddress;
        this.interruptVector = interruptVector;
    }
}

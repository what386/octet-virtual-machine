namespace Machine.Control.Interrupts;

public class InterruptVectorTable
{
    private readonly ushort[] interruptVectorTable;
    
    // Interrupt status and control
    public bool interruptsEnabled = false;
    public bool inInterruptHandler {get; private set;} = false;

    public InterruptVectorTable(int vectorTableSize = 16)
    {
        interruptVectorTable = new ushort[vectorTableSize];
    }

    public void SetInterruptVector(byte interruptNumber, ushort handlerAddress)
    {
        interruptVectorTable[interruptNumber] = handlerAddress;
    }

    public ushort GetInterruptVector(byte interruptNumber)
    {
        return interruptVectorTable[interruptNumber];
    }

    public void RequestInterrupt(byte interruptNumber)
    {
    }
       
    public void EnterInterruptHandler(byte interruptNumber)
    {
        inInterruptHandler = true;
        
        // do a thing
    }
    
    public void ExitInterruptHandler()
    {
        inInterruptHandler = false;
    }
}

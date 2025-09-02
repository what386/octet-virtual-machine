namespace Machine.Processor;

public class CentralProcessingUnit
{
    private int registerFileSize = 8;
    public byte flags;

    RegisterFile registers = new RegisterFile(8);

    
}

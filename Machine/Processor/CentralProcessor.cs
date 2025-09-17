using Machine.Processor.Registers;
using Machine.Processor.Arithmetic;

namespace Machine.Processor;

public class CentralProcessor
{
    private readonly StatusRegister flagsRegister = new StatusRegister();
    private readonly ControlRegister controlRegister = new ControlRegister();
    private readonly RegisterFile registerFile;
    private readonly ArithmeticManager arithmetic;

    public CentralProcessor()
    {
        registerFile = new RegisterFile(8);
        arithmetic = new ArithmeticManager(flagsRegister);
    }

    byte result = 0;

    public void Execute(Opcode opcode, int sourceA, int sourceB)
    {
        byte a = registerFile.Read(sourceA);
        byte b = registerFile.Read(sourceB);
        result = arithmetic.PerformOperation(opcode, a, b);
    }

    public void Writeback(int dest)
    {
        registerFile.Write(dest, result);
    }
}

namespace Machine.Control.InstructionDecoder;

public abstract class DecodedInstruction
{
    public readonly ushort rawInstruction;

    public ushort longImmediate;
    public byte immediate;

    public byte destRegister;
    public byte sourceA;
    public byte sourceB;

    public readonly Opcode opcode;
    public readonly string type;

    public readonly int executeCycles = 1;
    public int cyclesRemaining;
}

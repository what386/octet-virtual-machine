namespace Machine.Pipeline;

public class Instruction
{
    public ushort Address { get; set; }        // Where instruction was fetched from
    public byte Opcode { get; set; }           // The instruction opcode
    public byte Operand1 { get; set; }         // First operand/register
    public byte Operand2 { get; set; }         // Second operand/register  
    public byte Immediate { get; set; }        // Immediate value
    public InstructionType Type { get; set; }  // ALU, Branch, Load/Store, etc.
    public bool IsNop { get; set; }            // Pipeline bubble/stall
    
    // Execution results (filled during execute stage)
    public byte Result { get; set; }
    public byte DestinationRegister { get; set; }
    public bool WritesToRegister { get; set; }
    public bool UpdatesFlags { get; set; }

    public bool UsesRegister(byte register)
    {
        if (IsNop) return false;

        // For ALU, Load, etc. → they usually use Operand1 as a source
        if (Operand1 == register) return true;

        // Some instructions may use Operand2 as well
        if (Operand2 == register) return true;

        return false;
    }

}

public enum InstructionType
{
    ALU,        // Arithmetic/Logic operations
    Load,       // Load from memory
    Store,      // Store to memory  
    Branch,     // Conditional branches
    Jump,       // Unconditional jumps
    Nop         // No operation / pipeline bubble
}

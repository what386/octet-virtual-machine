namespace Machine.Control.InstructionDecoder;

public class DecodedInstruction
{
    // Raw instruction
    public ushort rawInstruction { get; set; }
    
    // Basic fields
    public byte opcode { get; set; }
    public InstructionType instructionType { get; set; }
    
    // Register fields (3-bit each)
    public byte destinationRegister { get; set; }
    public byte sourceA { get; set; }
    public byte sourceB { get; set; }
    
    // Immediate and address fields
    public byte immediate { get; set; }
    public ushort address { get; set; }
    public sbyte offset { get; set; } // For signed offsets (±32 range)
    
    // Operation modifiers
    public byte type { get; set; } // Raw type field for complex decoding
    public BranchCondition condition { get; set; }
    public ArithmeticOperation operation { get; set; }
    
    // Type-specific fields
    public BranchType branchType { get; set; }
    public StackType stackType { get; set; }
    public PushType pushType { get; set; }
    public SpecialRegister specialRegister { get; set; }
    
    // Arithmetic operation types
    public AddType addType { get; set; }
    public SubType subType { get; set; }
    public BitwiseType bitwiseType { get; set; }
    public InverseBitwiseType inverseBitwiseType { get; set; }
    public BarrelShiftType barrelShiftType { get; set; }
    public MultiplyDivideType multiplyDivideType { get; set; }
    public BitCountType bitCountType { get; set; }
    
    // Special instruction fields
    public byte setting { get; set; } // For SYS instruction
    public ushort coProcessorInstruction { get; set; } // For CPC instruction
    
    // Control flags
    public bool writesToRegister { get; set; }
    public bool updatesFlags { get; set; }
    public bool exitFlag { get; set; } // For HLT instruction
    public bool breakFlag { get; set; } // For RET instruction
    public bool haltsExecution { get; set; } // Instructions that halt pipeline
    
    // Pipeline information
    public int executeCycles { get; set; } = 1; // Default to 1 cycle
    public int cyclesRemaining { get; set; }
    
    public bool UsesRegister(byte regIndex)
    {
        if (instructionType == InstructionType.Nop) return false;
        if (regIndex == 0) return false; // R0 is always zero, no real dependency
        
        if (sourceA == regIndex) return true;
        if (sourceB == regIndex) return true;
        
        return false;
    }
    
    public bool ReadsFromRegister(byte regIndex)
    {
        return UsesRegister(regIndex);
    }
    
    public bool WritesToRegister(byte regIndex)
    {
        return writesToRegister && destinationRegister == regIndex && regIndex != 0;
    }
    
    // Helper method to get human-readable instruction string (for debugging)
    public override string ToString()
    {
        return $"{instructionType} (0x{RawInstruction:X4})";
    }
}

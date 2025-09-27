using Machine.Control.InstructionDecoder.Decoders;

namespace Machine.Control.InstructionDecoder;

public class DecodingManager 
{
    public static DecodedInstruction Decode(ushort instruction)
    {
        // Extract opcode from bits 15-12
        byte opcode = (byte)((instruction >> 12) & 0x0F);

        try
        {
            return opcode switch
            {
                // Control Flow Instructions (0-7)
                >= 0 and <= 7 => ControlFlowDecoder.Decode(instruction, opcode),
                
                // Memory and I/O Instructions (8-15)
                >= 8 and <= 15 => MemoryIODecoder.Decode(instruction, opcode),
                
                // Data Movement Instructions (16-17)
                >= 16 and <= 17 => DataMovementDecoder.Decode(instruction, opcode),
                
                // Immediate Operations (18-21)
                >= 18 and <= 21 => ImmediateDecoder.Decode(instruction, opcode),
                
                // Register Arithmetic Operations (22-27)
                >= 22 and <= 27 => ArithmeticDecoder.Decode(instruction, opcode),
                
                // Complex Operations (28-31)
                >= 28 and <= 31 => ComplexDecoder.Decode(instruction, opcode),
                
                _ => CreateInvalidInstruction(instruction, opcode)
            };
        }
        catch (System.ArgumentException)
        {
            // Handle invalid opcodes gracefully
            return CreateInvalidInstruction(instruction, opcode);
        }
    }

    private static DecodedInstruction CreateInvalidInstruction(ushort instruction, byte opcode)
    {
        return new DecodedInstruction
        {
            rawInstruction = instruction,
            opcode = opcode,
            instructionType = InstructionType.Nop, // Treat as NOP
            executeCycles = 1
        };
    }

    public static bool IsControlFlow(ushort instruction)
    {
        byte opcode = (byte)((instruction >> 12) & 0x0F);
        return opcode switch
        {
            4 => true,  // JMP
            5 => true,  // BRA
            6 => true,  // CAL
            7 => true,  // RET
            _ => false
        };
    }

    public static bool WritesToRegister(ushort instruction)
    {
        byte opcode = (byte)((instruction >> 12) & 0x0F);
        return opcode switch
        {
            0 or 1 or 4 or 6 or 7 or 9 or 11 or 30 => false, // NOP, HLT, JMP, CAL, RET, OUT, SST, OPI
            2 => true,  // SYS (may write to special registers)
            3 or 8 or 10 or 14 or 16 or 17 => true, // CLI, INP, SLD, MLD, LDI, MOV
            12 => (instruction >> 7 & 0x03) <= 2, // POP: only POP and POPF write
            13 => false, // PSH doesn't write to registers
            15 => false, // MST doesn't write to registers
            >= 18 and <= 21 => opcode != 20 && opcode != 21, // ADI, ANI write; CPI, TSI don't
            >= 22 and <= 29 => true, // All arithmetic operations write
            31 => true, // CPC may write
            _ => false
        };
    }

    public static byte GetDestinationRegister(ushort instruction)
    {
        if (!WritesToRegister(instruction))
            return 0xFF; // No destination register
            
        byte opcode = (byte)((instruction >> 12) & 0x0F);
        return opcode switch
        {
            3 or 8 or 10 or 12 or 14 or 16 or 17 => (byte)((instruction >> 9) & 0x07), // Bits 11-9
            18 or 22 or 23 or 24 or 25 or 26 or 27 or 28 or 29 => (byte)((instruction >> 9) & 0x07), // Bits 11-9
            19 => (byte)((instruction >> 9) & 0x07), // ANI: src/dest in bits 11-9
            _ => 0xFF
        };
    }
}

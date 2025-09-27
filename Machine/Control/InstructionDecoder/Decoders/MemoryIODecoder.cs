namespace Machine.Control.InstructionDecoder.Decoders;

public class MemoryIODecoder
{
    public static DecodedInstruction Decode(ushort instruction, byte opcode)
    {
        var decoded = new DecodedInstruction
        {
            RawInstruction = instruction,
            Opcode = opcode
        };

        switch (opcode)
        {
            case 8: DecodeInput(decoded); break;
            case 9: DecodeOutput(decoded); break;
            case 10: DecodeSpecialLoad(decoded); break;
            case 11: DecodeSpecialStore(decoded); break;
            case 12: DecodePopStack(decoded); break;
            case 13: DecodePushStack(decoded); break;
            case 14: DecodeMemoryLoad(decoded); break;
            case 15: DecodeMemoryStore(decoded); break;
            default:
                throw new System.ArgumentException($"Invalid memory/IO opcode: {opcode}");
        }

        return decoded;
    }

    private static void DecodeInput(DecodedInstruction decoded)
    {
        // INP: 01000 DDD AAAA AAAA
        // DDD = Destination register (3 bits), AAAA AAAA = Address (8 bits)
        decoded.InstructionType = InstructionType.Input;
        decoded.DestinationRegister = (byte)((decoded.RawInstruction >> 9) & 0x07); // Bits 11-9
        decoded.Address = (ushort)(decoded.RawInstruction & 0x1FF); // Bits 8-0
        decoded.WritesToRegister = true;
        decoded.HaltsExecution = true; // Halts until input fetched
        decoded.ExecuteCycles = 1; // Variable based on IO device
    }

    private static void DecodeOutput(DecodedInstruction decoded)
    {
        // OUT: 01001 SSS AAAA AAAA
        // SSS = Source register (3 bits), AAAA AAAA = Address (8 bits)
        decoded.InstructionType = InstructionType.Output;
        decoded.SourceA = (byte)((decoded.RawInstruction >> 9) & 0x07); // Bits 11-9
        decoded.Address = (ushort)(decoded.RawInstruction & 0x1FF); // Bits 8-0
        decoded.ExecuteCycles = 1;
    }

    private static void DecodeSpecialLoad(DecodedInstruction decoded)
    {
        // SLD: 01010 DDD ---- -RRR
        // DDD = Destination register (3 bits), RRR = Special register (3 bits)
        decoded.InstructionType = InstructionType.SpecialLoad;
        decoded.DestinationRegister = (byte)((decoded.RawInstruction >> 9) & 0x07); // Bits 11-9
        decoded.SpecialRegister = (SpecialRegister)(decoded.RawInstruction & 0x07); // Bits 2-0
        decoded.WritesToRegister = true;
        decoded.ExecuteCycles = 1; // May be longer for page retrieval
    }

    private static void DecodeSpecialStore(DecodedInstruction decoded)
    {
        // SST: 01011 RRR SSS- ----
        // RRR = Special register (3 bits), SSS = Source register (3 bits)
        decoded.InstructionType = InstructionType.SpecialStore;
        decoded.SpecialRegister = (SpecialRegister)((decoded.RawInstruction >> 9) & 0x07); // Bits 11-9
        decoded.SourceA = (byte)((decoded.RawInstruction >> 6) & 0x07); // Bits 8-6
        decoded.ExecuteCycles = 1;
    }

    private static void DecodePopStack(DecodedInstruction decoded)
    {
        // POP: 01100 DDD TT OOOOOOO
        // DDD = Destination register (3 bits), TT = Type (2 bits), OOOOOOO = Offset (7 bits signed)
        decoded.InstructionType = InstructionType.PopStack;
        decoded.DestinationRegister = (byte)((decoded.RawInstruction >> 9) & 0x07); // Bits 11-9
        decoded.StackType = (StackType)((decoded.RawInstruction >> 7) & 0x03); // Bits 8-7
        
        // Handle signed 7-bit offset (-64 to +63)
        byte offsetRaw = (byte)(decoded.RawInstruction & 0x7F); // Bits 6-0
        decoded.Offset = (offsetRaw & 0x40) != 0 ? (sbyte)(offsetRaw | 0x80) : (sbyte)offsetRaw;
        
        // Only POP and POPF write to registers
        decoded.WritesToRegister = (decoded.StackType == StackType.Pop || decoded.StackType == StackType.PopFlags);
        decoded.HaltsExecution = true; // May halt on cache miss
        decoded.ExecuteCycles = 1;
    }

    private static void DecodePushStack(DecodedInstruction decoded)
    {
        // PSH: 01101 SSS TT OOOOOOO
        // SSS = Source register (3 bits), TT = Type (2 bits), OOOOOOO = Offset (7 bits signed)
        decoded.InstructionType = InstructionType.PushStack;
        decoded.SourceA = (byte)((decoded.RawInstruction >> 9) & 0x07); // Bits 11-9
        decoded.PushType = (PushType)((decoded.RawInstruction >> 7) & 0x03); // Bits 8-7
        
        // Handle signed 7-bit offset (-64 to +63)
        byte offsetRaw = (byte)(decoded.RawInstruction & 0x7F); // Bits 6-0
        decoded.Offset = (offsetRaw & 0x40) != 0 ? (sbyte)(offsetRaw | 0x80) : (sbyte)offsetRaw;
        
        decoded.HaltsExecution = true; // May halt on cache miss
        decoded.ExecuteCycles = 1;
    }

    private static void DecodeMemoryLoad(DecodedInstruction decoded)
    {
        // MLD: 01110 DDD AAAA AAAA
        // DDD = Destination register (3 bits), AAAA AAAA = Address (8 bits)
        // Note: Address = pointer + offset based on comments
        decoded.InstructionType = InstructionType.MemoryLoad;
        decoded.DestinationRegister = (byte)((decoded.RawInstruction >> 9) & 0x07); // Bits 11-9
        decoded.Address = (ushort)(decoded.RawInstruction & 0x1FF); // Bits 8-0
        decoded.WritesToRegister = true;
        decoded.HaltsExecution = true; // May halt on cache miss
        decoded.ExecuteCycles = 1;
    }

    private static void DecodeMemoryStore(DecodedInstruction decoded)
    {
        // MST: 01111 SSS AAAA AAAA
        // SSS = Source register (3 bits), AAAA AAAA = Address (8 bits)
        // Note: Address = pointer + offset based on comments
        decoded.InstructionType = InstructionType.MemoryStore;
        decoded.SourceA = (byte)((decoded.RawInstruction >> 9) & 0x07); // Bits 11-9
        decoded.Address = (ushort)(decoded.RawInstruction & 0x1FF); // Bits 8-0
        decoded.HaltsExecution = true; // May halt on cache miss
        decoded.ExecuteCycles = 1;
    }
}

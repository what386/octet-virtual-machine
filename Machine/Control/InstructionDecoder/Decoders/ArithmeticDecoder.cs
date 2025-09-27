namespace Machine.Control.InstructionDecoder.Decoders;

public static class ArithmeticDecoder
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
            case 22: DecodeAdd(decoded); break;
            case 23: DecodeSubtract(decoded); break;
            case 24: DecodeBitwise(decoded); break;
            case 25: DecodeInverseBitwise(decoded); break;
            case 26: DecodeBarrelShift(decoded); break;
            case 27: DecodeBarrelShiftImmediate(decoded); break;
            default:
                throw new System.ArgumentException($"Invalid arithmetic opcode: {opcode}");
        }

        return decoded;
    }

    private static void DecodeAdd(DecodedInstruction decoded)
    {
        // ADD: 10110 DDD SSS TTT BBB
        // DDD = Destination (3 bits), SSS = Source A (3 bits), TTT = Type (3 bits), BBB = Source B (3 bits)
        decoded.InstructionType = InstructionType.Arithmetic;
        decoded.Operation = ArithmeticOperation.Add;
        decoded.DestinationRegister = (byte)((decoded.RawInstruction >> 9) & 0x07); // Bits 11-9
        decoded.SourceA = (byte)((decoded.RawInstruction >> 6) & 0x07); // Bits 8-6
        decoded.Type = (byte)((decoded.RawInstruction >> 3) & 0x07); // Bits 5-3
        decoded.SourceB = (byte)(decoded.RawInstruction & 0x07); // Bits 2-0
        
        // Convert type to specific add operation
        decoded.AddType = (AddType)(decoded.Type & 0x03); // Use lower 2 bits for now
        
        decoded.WritesToRegister = true;
        decoded.UpdatesFlags = true;
        decoded.ExecuteCycles = 1;
    }

    private static void DecodeSubtract(DecodedInstruction decoded)
    {
        // SUB: 10111 DDD SSS TTT BBB
        // DDD = Destination (3 bits), SSS = Source A (3 bits), TTT = Type (3 bits), BBB = Source B (3 bits)
        decoded.InstructionType = InstructionType.Arithmetic;
        decoded.Operation = ArithmeticOperation.Subtract;
        decoded.DestinationRegister = (byte)((decoded.RawInstruction >> 9) & 0x07); // Bits 11-9
        decoded.SourceA = (byte)((decoded.RawInstruction >> 6) & 0x07); // Bits 8-6
        decoded.Type = (byte)((decoded.RawInstruction >> 3) & 0x07); // Bits 5-3
        decoded.SourceB = (byte)(decoded.RawInstruction & 0x07); // Bits 2-0
        
        // Convert type to specific subtract operation
        decoded.SubType = (SubType)(decoded.Type & 0x03); // Use lower 2 bits for now
        
        decoded.WritesToRegister = true;
        decoded.UpdatesFlags = true;
        decoded.ExecuteCycles = 1;
    }

    private static void DecodeBitwise(DecodedInstruction decoded)
    {
        // BIT: 11000 DDD SSS TTT BBB
        // DDD = Destination (3 bits), SSS = Source A (3 bits), TTT = Type (3 bits), BBB = Source B (3 bits)
        // Type: 00=OR, 01=AND, 10=XOR, 11=IMP
        decoded.InstructionType = InstructionType.Bitwise;
        decoded.DestinationRegister = (byte)((decoded.RawInstruction >> 9) & 0x07); // Bits 11-9
        decoded.SourceA = (byte)((decoded.RawInstruction >> 6) & 0x07); // Bits 8-6
        decoded.Type = (byte)((decoded.RawInstruction >> 3) & 0x07); // Bits 5-3
        decoded.SourceB = (byte)(decoded.RawInstruction & 0x07); // Bits 2-0
        
        // Convert type to specific bitwise operation
        decoded.BitwiseType = (BitwiseType)(decoded.Type & 0x03);
        decoded.Operation = decoded.BitwiseType switch
        {
            BitwiseType.Or => ArithmeticOperation.Or,
            BitwiseType.And => ArithmeticOperation.And,
            BitwiseType.Xor => ArithmeticOperation.Xor,
            BitwiseType.Implies => ArithmeticOperation.Implies,
            _ => ArithmeticOperation.Or
        };
        
        decoded.WritesToRegister = true;
        decoded.UpdatesFlags = true;
        decoded.ExecuteCycles = 1;
    }

    private static void DecodeInverseBitwise(DecodedInstruction decoded)
    {
        // BNT: 11001 DDD SSS TTT BBB
        // DDD = Destination (3 bits), SSS = Source A (3 bits), TTT = Type (3 bits), BBB = Source B (3 bits)
        // Type: 00=NOR, 01=NAND, 10=XNOR, 11=NIMP
        decoded.InstructionType = InstructionType.InverseBitwise;
        decoded.DestinationRegister = (byte)((decoded.RawInstruction >> 9) & 0x07); // Bits 11-9
        decoded.SourceA = (byte)((decoded.RawInstruction >> 6) & 0x07); // Bits 8-6
        decoded.Type = (byte)((decoded.RawInstruction >> 3) & 0x07); // Bits 5-3
        decoded.SourceB = (byte)(decoded.RawInstruction & 0x07); // Bits 2-0
        
        // Convert type to specific inverse bitwise operation
        decoded.InverseBitwiseType = (InverseBitwiseType)(decoded.Type & 0x03);
        
        decoded.WritesToRegister = true;
        decoded.UpdatesFlags = true;
        decoded.ExecuteCycles = 1;
    }

    private static void DecodeBarrelShift(DecodedInstruction decoded)
    {
        // BSH: 11010 DDD SSS TTT BBB
        // DDD = Destination (3 bits), SSS = Source A (3 bits), TTT = Type (3 bits), BBB = Source B (3 bits)
        // Type: 00=BSL, 01=BSR, 10=ROR, 11=BSXR
        decoded.InstructionType = InstructionType.BarrelShift;
        decoded.DestinationRegister = (byte)((decoded.RawInstruction >> 9) & 0x07); // Bits 11-9
        decoded.SourceA = (byte)((decoded.RawInstruction >> 6) & 0x07); // Bits 8-6
        decoded.Type = (byte)((decoded.RawInstruction >> 3) & 0x07); // Bits 5-3
        decoded.SourceB = (byte)(decoded.RawInstruction & 0x07); // Bits 2-0 (shift amount)
        
        // Convert type to specific barrel shift operation
        decoded.BarrelShiftType = (BarrelShiftType)(decoded.Type & 0x03);
        
        decoded.WritesToRegister = true;
        decoded.UpdatesFlags = true;
        decoded.ExecuteCycles = 1; // Barrel shifter is single cycle
    }

    private static void DecodeBarrelShiftImmediate(DecodedInstruction decoded)
    {
        // BSI: 11011 DDD SSS TTT III
        // DDD = Destination (3 bits), SSS = Source A (3 bits), TTT = Type (3 bits), III = Immediate (3 bits)
        // Type: 00=LSHI, 01=RSHI, 10=RORI, 11=BSXRI
        decoded.InstructionType = InstructionType.BarrelShiftImmediate;
        decoded.DestinationRegister = (byte)((decoded.RawInstruction >> 9) & 0x07); // Bits 11-9
        decoded.SourceA = (byte)((decoded.RawInstruction >> 6) & 0x07); // Bits 8-6
        decoded.Type = (byte)((decoded.RawInstruction >> 3) & 0x07); // Bits 5-3
        decoded.Immediate = (byte)(decoded.RawInstruction & 0x07); // Bits 2-0 (shift amount 0-7)
        
        // Convert type to specific barrel shift operation
        decoded.BarrelShiftType = (BarrelShiftType)(decoded.Type & 0x03);
        
        decoded.WritesToRegister = true;
        decoded.UpdatesFlags = true;
        decoded.ExecuteCycles = 1; // Barrel shifter is single cycle
    }
}

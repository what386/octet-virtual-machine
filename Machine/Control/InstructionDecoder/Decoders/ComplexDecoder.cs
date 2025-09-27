namespace Machine.Control.InstructionDecoder.Decoders;

public class ComplexDecoder
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
            case 28: DecodeMultiplyDivide(decoded); break;
            case 29: DecodeBitCount(decoded); break;
            case 30: DecodeOutputImmediate(decoded); break;
            case 31: DecodeCoProcessor(decoded); break;
            default:
                throw new System.ArgumentException($"Invalid complex opcode: {opcode}");
        }

        return decoded;
    }

    private static void DecodeMultiplyDivide(DecodedInstruction decoded)
    {
        // MUL: 11100 DDD SSS TTT BBB
        // DDD = Destination (3 bits), SSS = Source A (3 bits), TTT = Type (3 bits), BBB = Source B (3 bits)
        // Type: 00=MUL (low), 01=MULU (high), 10=DIV, 11=MOD
        decoded.InstructionType = InstructionType.MultiplyDivide;
        decoded.DestinationRegister = (byte)((decoded.RawInstruction >> 9) & 0x07); // Bits 11-9
        decoded.SourceA = (byte)((decoded.RawInstruction >> 6) & 0x07); // Bits 8-6
        decoded.Type = (byte)((decoded.RawInstruction >> 3) & 0x07); // Bits 5-3
        decoded.SourceB = (byte)(decoded.RawInstruction & 0x07); // Bits 2-0
        
        // Convert type to specific multiply/divide operation
        decoded.MultiplyDivideType = (MultiplyDivideType)(decoded.Type & 0x03);
        
        decoded.WritesToRegister = true;
        decoded.UpdatesFlags = true;
        decoded.HaltsExecution = true; // Complex math operations halt until finished
        
        // Variable execution cycles based on operation
        decoded.ExecuteCycles = decoded.MultiplyDivideType switch
        {
            MultiplyDivideType.MultiplyLow => 4,
            MultiplyDivideType.MultiplyHigh => 4,
            MultiplyDivideType.Divide => 8,
            MultiplyDivideType.Modulo => 8,
            _ => 4
        };
    }

    private static void DecodeBitCount(DecodedInstruction decoded)
    {
        // BTC: 11101 DDD SSS TTT ---
        // DDD = Destination (3 bits), SSS = Source A (3 bits), TTT = Type (3 bits)
        // Type: 00=SQRT, 01=CLZ, 10=CTZ, 11=CTO (POPCNT)
        decoded.InstructionType = InstructionType.BitCount;
        decoded.DestinationRegister = (byte)((decoded.RawInstruction >> 9) & 0x07); // Bits 11-9
        decoded.SourceA = (byte)((decoded.RawInstruction >> 6) & 0x07); // Bits 8-6
        decoded.Type = (byte)((decoded.RawInstruction >> 3) & 0x07); // Bits 5-3
        
        // Convert type to specific bit count operation
        decoded.BitCountType = (BitCountType)(decoded.Type & 0x03);
        
        decoded.WritesToRegister = true;
        decoded.UpdatesFlags = true;
        decoded.HaltsExecution = true; // Complex bit operations halt until finished
        
        // Variable execution cycles based on operation complexity
        decoded.ExecuteCycles = decoded.BitCountType switch
        {
            BitCountType.SquareRoot => 8,
            BitCountType.CountLeadingZeros => 2,
            BitCountType.CountTrailingZeros => 2,
            BitCountType.CountOnes => 3,
            _ => 2
        };
    }

    private static void DecodeOutputImmediate(DecodedInstruction decoded)
    {
        // OPI: 11110 AAAA IIII IIII
        // AAAA = Address (4 bits), IIII IIII = Immediate (8 bits)
        decoded.InstructionType = InstructionType.OutputImmediate;
        decoded.Address = (ushort)((decoded.RawInstruction >> 8) & 0x0F); // Bits 11-8
        decoded.Immediate = (byte)(decoded.RawInstruction & 0xFF); // Bits 7-0
        decoded.ExecuteCycles = 1;
    }

    private static void DecodeCoProcessor(DecodedInstruction decoded)
    {
        // CPC: 11111 III IIII IIII
        // III IIII IIII = Co-processor instruction (12 bits)
        decoded.InstructionType = InstructionType.CoProcessor;
        decoded.CoProcessorInstruction = (ushort)(decoded.RawInstruction & 0x0FFF); // Bits 11-0
        decoded.HaltsExecution = true; // Halts until co-processor responds
        decoded.WritesToRegister = true; // May write back to register
        decoded.ExecuteCycles = 1; // Variable based on co-processor operation
    }
}

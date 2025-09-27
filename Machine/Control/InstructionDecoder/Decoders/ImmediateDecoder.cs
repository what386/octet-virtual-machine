namespace Machine.Control.InstructionDecoder.Decoders;

public class ImmediateDecoder
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
            case 18: DecodeAddImmediate(decoded); break;
            case 19: DecodeAndImmediate(decoded); break;
            case 20: DecodeCompareImmediate(decoded); break;
            case 21: DecodeTestImmediate(decoded); break;
            default:
                throw new System.ArgumentException($"Invalid immediate opcode: {opcode}");
        }

        return decoded;
    }

    private static void DecodeAddImmediate(DecodedInstruction decoded)
    {
        // ADI: 10010 DDD SSS IIIIII
        // DDD = Destination register (3 bits), SSS = Source register (3 bits), IIIIII = Immediate (6 bits)
        decoded.InstructionType = InstructionType.ArithmeticImmediate;
        decoded.Operation = ArithmeticOperation.Add;
        decoded.DestinationRegister = (byte)((decoded.RawInstruction >> 9) & 0x07); // Bits 11-9
        decoded.SourceA = (byte)((decoded.RawInstruction >> 6) & 0x07); // Bits 8-6
        decoded.Immediate = (byte)(decoded.RawInstruction & 0x3F); // Bits 5-0
        decoded.WritesToRegister = true;
        decoded.UpdatesFlags = true;
        decoded.ExecuteCycles = 1;
    }

    private static void DecodeAndImmediate(DecodedInstruction decoded)
    {
        // ANI: 10011 RRR IIII IIII
        // RRR = Source/Destination register (3 bits), IIII IIII = Immediate (8 bits)
        decoded.InstructionType = InstructionType.ArithmeticImmediate;
        decoded.Operation = ArithmeticOperation.And;
        decoded.DestinationRegister = (byte)((decoded.RawInstruction >> 9) & 0x07); // Bits 11-9
        decoded.SourceA = decoded.DestinationRegister; // Same register for src/dest
        decoded.Immediate = (byte)(decoded.RawInstruction & 0xFF); // Bits 7-0
        decoded.WritesToRegister = true;
        decoded.UpdatesFlags = true;
        decoded.ExecuteCycles = 1;
    }

    private static void DecodeCompareImmediate(DecodedInstruction decoded)
    {
        // CPI: 10100 SSS IIII IIII
        // SSS = Source register (3 bits), IIII IIII = Immediate (8 bits)
        // Compare performs subtraction but doesn't write back result
        decoded.InstructionType = InstructionType.CompareImmediate;
        decoded.Operation = ArithmeticOperation.Subtract;
        decoded.SourceA = (byte)((decoded.RawInstruction >> 9) & 0x07); // Bits 11-9
        decoded.Immediate = (byte)(decoded.RawInstruction & 0xFF); // Bits 7-0
        decoded.WritesToRegister = false; // Compare doesn't write back
        decoded.UpdatesFlags = true;
        decoded.ExecuteCycles = 1;
    }

    private static void DecodeTestImmediate(DecodedInstruction decoded)
    {
        // TSI: 10101 SSS IIII IIII
        // SSS = Source register (3 bits), IIII IIII = Immediate (8 bits)
        // Test performs AND but doesn't write back result
        decoded.InstructionType = InstructionType.TestImmediate;
        decoded.Operation = ArithmeticOperation.And;
        decoded.SourceA = (byte)((decoded.RawInstruction >> 9) & 0x07); // Bits 11-9
        decoded.Immediate = (byte)(decoded.RawInstruction & 0xFF); // Bits 7-0
        decoded.WritesToRegister = false; // Test doesn't write back
        decoded.UpdatesFlags = true;
        decoded.ExecuteCycles = 1;
    }

namespace Machine.Control.InstructionDecoder.Decoders;

public class DataMovementDecoder
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
            case 16: DecodeLoadImmediate(decoded); break;
            case 17: DecodeMove(decoded); break;
            default:
                throw new System.ArgumentException($"Invalid data movement opcode: {opcode}");
        }

        return decoded;
    }

    private static void DecodeLoadImmediate(DecodedInstruction decoded)
    {
        // LDI: 10000 DDD IIII IIII
        // DDD = Destination register (3 bits), IIII IIII = Immediate (8 bits)
        decoded.InstructionType = InstructionType.LoadImmediate;
        decoded.DestinationRegister = (byte)((decoded.RawInstruction >> 9) & 0x07); // Bits 11-9
        decoded.Immediate = (byte)(decoded.RawInstruction & 0xFF); // Bits 7-0
        decoded.WritesToRegister = true;
        decoded.ExecuteCycles = 1;
    }

    private static void DecodeMove(DecodedInstruction decoded)
    {
        // MOV: 10001 DDD SSS- ----
        // DDD = Destination register (3 bits), SSS = Source register (3 bits)
        decoded.InstructionType = InstructionType.Move;
        decoded.DestinationRegister = (byte)((decoded.RawInstruction >> 9) & 0x07); // Bits 11-9
        decoded.SourceA = (byte)((decoded.RawInstruction >> 6) & 0x07); // Bits 8-6
        decoded.WritesToRegister = true;
        decoded.UpdatesFlags = true; // MOV updates flags according to ISA
        decoded.Operation = ArithmeticOperation.Or; // MOV is essentially OR with R0
        decoded.ExecuteCycles = 1;
    }
}

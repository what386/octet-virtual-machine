namespace Machine.Control.InstructionDecoder.Decoders;

public class ControlFlowDecoder
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
            case 0: DecodeNop(decoded); break;
            case 1: DecodeHalt(decoded); break;
            case 2: DecodeSystem(decoded); break;
            case 3: DecodeConditionalLoadImmediate(decoded); break;
            case 4: DecodeJump(decoded); break;
            case 5: DecodeBranch(decoded); break;
            case 6: DecodeCall(decoded); break;
            case 7: DecodeReturn(decoded); break;
            default:
                throw new System.ArgumentException($"Invalid control flow opcode: {opcode}");
        }

        return decoded;
    }

    private static void DecodeNop(DecodedInstruction decoded)
    {
        // NOP: All bits should be 0 for true NOP
        decoded.InstructionType = InstructionType.Nop;
        decoded.ExecuteCycles = 1;
    }

    private static void DecodeHalt(DecodedInstruction decoded)
    {
        // HLT: 00001 E-- ---- ----
        // Bit E (8) determines if this is EXIT (stop clock) or just halt
        decoded.InstructionType = InstructionType.Halt;
        decoded.ExitFlag = ((decoded.RawInstruction >> 8) & 0x01) == 1;
        decoded.HaltsExecution = true;
        decoded.ExecuteCycles = 1;
    }

    private static void DecodeSystem(DecodedInstruction decoded)
    {
        // SYS: 00010 SSS IIII IIII
        // SSS = Setting (3 bits), IIII IIII = Immediate (8 bits)
        decoded.InstructionType = InstructionType.System;
        decoded.Setting = (byte)((decoded.RawInstruction >> 8) & 0x07); // Bits 10-8
        decoded.Immediate = (byte)(decoded.RawInstruction & 0xFF); // Bits 7-0
        decoded.ExecuteCycles = 1;
    }

    private static void DecodeConditionalLoadImmediate(DecodedInstruction decoded)
    {
        // CLI: 00011 DDD CCC IIIIII
        // DDD = Destination (3 bits), CCC = Condition (3 bits), IIIIII = Immediate (6 bits)
        decoded.InstructionType = InstructionType.ConditionalLoadImm;
        decoded.DestinationRegister = (byte)((decoded.RawInstruction >> 9) & 0x07); // Bits 11-9
        decoded.Condition = (BranchCondition)((decoded.RawInstruction >> 6) & 0x07); // Bits 8-6
        decoded.Immediate = (byte)(decoded.RawInstruction & 0x3F); // Bits 5-0
        decoded.WritesToRegister = true;
        decoded.ExecuteCycles = 1;
    }

    private static void DecodeJump(DecodedInstruction decoded)
    {
        // JMP: 00100 AAA AAAA AAAA
        // AAAA AAAA AAAA = Address (12 bits)
        decoded.InstructionType = InstructionType.Jump;
        decoded.Address = (ushort)(decoded.RawInstruction & 0x0FFF); // Bits 11-0
        decoded.HaltsExecution = true; // Causes pipeline flush
        decoded.ExecuteCycles = 1;
    }

    private static void DecodeBranch(DecodedInstruction decoded)
    {
        // BRA: 00101 CCC TT AAAAAAA
        // CCC = Condition (3 bits), TT = Type (2 bits), AAAAAAA = Address in page (7 bits)
        decoded.InstructionType = InstructionType.Branch;
        decoded.Condition = (BranchCondition)((decoded.RawInstruction >> 9) & 0x07); // Bits 11-9
        decoded.BranchType = (BranchType)((decoded.RawInstruction >> 7) & 0x03); // Bits 8-7
        decoded.Address = (ushort)(decoded.RawInstruction & 0x7F); // Bits 6-0
        
        // Branch may or may not halt based on prediction
        decoded.HaltsExecution = false; // Let branch predictor decide
        decoded.ExecuteCycles = 1;
    }

    private static void DecodeCall(DecodedInstruction decoded)
    {
        // CAL: 00110 AAA AAAA AAAA
        // AAAA AAAA AAAA = Address (12 bits)
        decoded.InstructionType = InstructionType.Call;
        decoded.Address = (ushort)(decoded.RawInstruction & 0x0FFF); // Bits 11-0
        decoded.HaltsExecution = true; // Causes pipeline flush and stack push
        decoded.ExecuteCycles = 1;
    }

    private static void DecodeReturn(DecodedInstruction decoded)
    {
        // RET: 00111 -- B- ---- ----
        // Bit B (8) determines if this breaks to oldest stack entry and flushes call stack
        decoded.InstructionType = InstructionType.Return;
        decoded.BreakFlag = ((decoded.RawInstruction >> 8) & 0x01) == 1;
        decoded.HaltsExecution = true; // Causes pipeline flush and stack pop
        decoded.ExecuteCycles = 1;
    }
}

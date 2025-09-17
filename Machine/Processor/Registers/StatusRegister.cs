namespace Machine.Processor.Registers;

public class StatusRegister
{
    // Flag register (8 bits)
    public byte Flags { get; private set; }

    // Bits 1 and 3 are reserved
    public const byte CARRY_FLAG = 0x01;      // Bit 0 - Carry flag
    public const byte RESERVED1 = 0x02;
    public const byte PARITY_FLAG = 0x04;     // Bit 2 - Parity flag (even parity)
    public const byte RESERVED3 = 0x08;
    public const byte AUX_CARRY_FLAG = 0x10;  // Bit 4 - Auxiliary carry (half-carry)
    public const byte OVERFLOW_FLAG = 0x20;   // Bit 5 - Overflow flag
    public const byte ZERO_FLAG = 0x40;       // Bit 6 - Zero flag
    public const byte SIGN_FLAG = 0x80;       // Bit 7 - Sign flag

    // Flag access properties
    public bool CarryFlag => (Flags & CARRY_FLAG) != 0;
    public bool ParityFlag => (Flags & PARITY_FLAG) != 0;
    public bool AuxCarryFlag => (Flags & AUX_CARRY_FLAG) != 0;
    public bool OverflowFlag => (Flags & OVERFLOW_FLAG) != 0;
    public bool ZeroFlag => (Flags & ZERO_FLAG) != 0;
    public bool SignFlag => (Flags & SIGN_FLAG) != 0;

    public void ClearFlags() => Flags = 0;

    private void SetFlag(byte flagMask, bool value)
    {
        if (value)
            Flags |= flagMask;
        else
            Flags &= (byte)~flagMask;
    }

    // Calculate parity (even parity = true)
    public bool CalculateParity(byte value)
    {
        int count = 0;
        for (int i = 0; i < 8; i++)
        {
            if ((value & (1 << i)) != 0)
                count++;
        }
        return (count % 2) == 0;
    }

    // Update flags based on result
    public void UpdateFlags(byte result, bool carry = false, bool auxCarry = false, bool overflow = false)
    {
        SetFlag(ZERO_FLAG, result == 0);
        SetFlag(SIGN_FLAG, (result & 0x80) != 0);  // Check bit 7 for sign
        SetFlag(PARITY_FLAG, CalculateParity(result));
        SetFlag(CARRY_FLAG, carry);
        SetFlag(AUX_CARRY_FLAG, auxCarry);
        SetFlag(OVERFLOW_FLAG, overflow);
    }
}


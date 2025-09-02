namespace Machine.Processor
{
    public static class ArithmeticLogicUnit
    {
        // Flag register (8 bits)
        public static byte Flags { get; private set; }

        // Bits 1 and 3 are reserved
        public const byte CARRY_FLAG = 0x01;      // Bit 0 - Carry flag
        public const byte PARITY_FLAG = 0x04;     // Bit 2 - Parity flag (even parity)
        public const byte AUX_CARRY_FLAG = 0x10;  // Bit 4 - Auxiliary carry (half-carry)
        public const byte OVERFLOW_FLAG = 0x20;   // Bit 5 - Overflow flag
        public const byte ZERO_FLAG = 0x40;       // Bit 6 - Zero flag
        public const byte SIGN_FLAG = 0x80;       // Bit 7 - Sign flag

        // Flag access properties
        public static bool CarryFlag => (Flags & CARRY_FLAG) != 0;
        public static bool ParityFlag => (Flags & PARITY_FLAG) != 0;
        public static bool AuxCarryFlag => (Flags & AUX_CARRY_FLAG) != 0;
        public static bool OverflowFlag => (Flags & OVERFLOW_FLAG) != 0;
        public static bool ZeroFlag => (Flags & ZERO_FLAG) != 0;
        public static bool SignFlag => (Flags & SIGN_FLAG) != 0;

        public static void ClearFlags() => Flags = 0;

        private static void SetFlag(byte flagMask, bool value)
        {
            if (value)
                Flags |= flagMask;
            else
                Flags &= (byte)~flagMask;
        }

        // Calculate parity (even parity = true)
        private static bool CalculateParity(byte value)
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
        private static void UpdateFlags(byte result, bool carry = false, bool auxCarry = false, bool overflow = false)
        {
            SetFlag(ZERO_FLAG, result == 0);
            SetFlag(SIGN_FLAG, (result & 0x80) != 0);  // Check bit 7 for sign
            SetFlag(PARITY_FLAG, CalculateParity(result));
            SetFlag(CARRY_FLAG, carry);
            SetFlag(AUX_CARRY_FLAG, auxCarry);
            SetFlag(OVERFLOW_FLAG, overflow);
        }

        // External flag update method for external units (BSU and CMU)
        public static void UpdateFlagsExternal(byte result, bool carry = false, bool auxCarry = false, bool overflow = false)
        {
            UpdateFlags(result, carry, auxCarry, overflow);
        }

        // ---Arithmetic---

        public static byte Add(byte a, byte b)
        {
            int result = a + b;
            byte byteResult = (byte)result;
            
            bool carry = result > 255;
            bool auxCarry = ((a & 0x0F) + (b & 0x0F)) > 0x0F;
            
            // Check for signed overflow (two positive numbers yielding negative, or two negative yielding positive)
            bool overflow = ((a & 0x80) == (b & 0x80)) && ((a & 0x80) != (byteResult & 0x80));
            
            UpdateFlags(byteResult, carry, auxCarry, overflow);
            return byteResult;
        }

        public static byte Sub(byte a, byte b)
        {
            int result = a - b;
            byte byteResult = (byte)result;
            
            bool carry = result < 0;  // Borrow occurred
            bool auxCarry = (a & 0x0F) < (b & 0x0F);
            
            // Check for signed overflow
            bool overflow = ((a & 0x80) != (b & 0x80)) && ((a & 0x80) != (byteResult & 0x80));
            
            UpdateFlags(byteResult, carry, auxCarry, overflow);
            return byteResult;
        }

        public static byte Increment(byte a)
        {
            int result = a + 1;
            byte byteResult = (byte)result;
            
            bool auxCarry = (a & 0x0F) == 0x0F;
            bool overflow = a == 0x7F;  // Incrementing max positive signed value
           
            // Increment does not affect the carry flag
            UpdateFlags(byteResult, CarryFlag, auxCarry, overflow);
            return byteResult;
        }

        public static byte Decrememt(byte a)
        {
            int result = a - 1;
            byte byteResult = (byte)result;
            
            bool auxCarry = (a & 0x0F) == 0x00;
            bool overflow = a == 0x80;  // Decrementing max negative signed value
            
            // Decrement does not affect the carry flag
            UpdateFlags(byteResult, CarryFlag, auxCarry, overflow);
            return byteResult;
        }

        // Compare operation (subtract without storing result)
        public static void Compare(byte a, byte b) => Sub(a, b);       

        // Test operation (AND without storing result)
        public static void Test(byte a, byte b) => And(a, b);

        // ---Bitwise---

        public static byte And(byte a, byte b)
        {
            byte result = (byte)(a & b);
            UpdateFlags(result, false, false, false);  // AND clears carry, aux carry, and overflow
            return result;
        }

        public static byte Or(byte a, byte b)
        {
            byte result = (byte)(a | b);
            UpdateFlags(result, false, false, false);  // OR clears carry, aux carry, and overflow
            return result;
        }

        public static byte Xor(byte a, byte b)
        {
            byte result = (byte)(a ^ b);
            UpdateFlags(result, false, false, false);  // XOR clears carry, aux carry, and overflow
            return result;
        }

        public static byte Not(byte a)
        {
            byte result = (byte)~a;
            UpdateFlags(result, false, false, false);  // NOT clears carry, aux carry, and overflow
            return result;
        }

        public static byte Implies(byte a, byte b)
        {
            byte result = (byte)(~a | b);
            UpdateFlags(result, false, false, false);  // Logical operation clears carry, aux carry, and overflow
            return result;
        }
    }
}

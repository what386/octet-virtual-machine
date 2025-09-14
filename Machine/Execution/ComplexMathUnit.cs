namespace Machine.Execution;

public static class ComplexMathUnit
{
    // Reference to ALU for flag operations
    private static void UpdateFlags(byte result, bool carry = false, bool overflow = false)
    {
        // Complex math operations typically don't affect aux carry
        ArithmeticLogicUnit.UpdateFlagsExternal(result, carry, false, overflow);
    }

    // Multiplication operations

    public static byte MultiplyLow(byte a, byte b)
    {
        int result = a * b;
        byte lowResult = (byte)(result & 0xFF);
        
        // Carry is set if high byte is non-zero
        bool carry = (result & 0xFF00) != 0;
        
        UpdateFlags(lowResult, carry, false);
        return lowResult;
    }

    public static byte MultiplyHigh(byte a, byte b)
    {
        int result = a * b;
        byte highResult = (byte)((result >> 8) & 0xFF);
        
        // Carry is typically cleared for high byte operations
        UpdateFlags(highResult, false, false);
        return highResult;
    }

    public static (byte low, byte high) MultiplyFull(byte a, byte b)
    {
        int result = a * b;
        byte low = (byte)(result & 0xFF);
        byte high = (byte)((result >> 8) & 0xFF);
        
        // Update flags based on the full 16-bit result
        bool carry = high != 0;
        UpdateFlags(low, carry, false);
        
        return (low, high);
    }

    // Division operations

    public static byte Divide(byte dividend, byte divisor)
    {
        if (divisor == 0)
        {
            // Division by zero - set overflow flag and return max value
            UpdateFlags(0xFF, true, true);
            return 0xFF;
        }

        byte result = (byte)(dividend / divisor);
        UpdateFlags(result, false, false);
        return result;
    }

    public static byte Modulo(byte dividend, byte divisor)
    {
        if (divisor == 0)
        {
            // Modulo by zero - set overflow flag and return dividend
            UpdateFlags(dividend, true, true);
            return dividend;
        }

        byte result = (byte)(dividend % divisor);
        UpdateFlags(result, false, false);
        return result;
    }

    public static (byte quotient, byte remainder) DivideWithRemainder(byte dividend, byte divisor)
    {
        if (divisor == 0)
        {
            // Division by zero - set overflow flag
            UpdateFlags(0xFF, true, true);
            return (0xFF, dividend);
        }

        byte quotient = (byte)(dividend / divisor);
        byte remainder = (byte)(dividend % divisor);
        
        UpdateFlags(quotient, false, false);
        return (quotient, remainder);
    }

    // Advanced mathematical operations

    public static byte SquareRoot(byte value)
    {
        if (value == 0)
        {
            UpdateFlags(0, false, false);
            return 0;
        }

        // Integer square root using binary search
        byte result = 0;
        byte bit = 0x80;  // Start with the highest bit

        while (bit > result)
        {
            byte temp = (byte)(result | bit);
            if (temp * temp <= value)
            {
                result = temp;
            }
            bit >>= 1;
        }

        // Set carry if the square root is not exact
        bool carry = (result * result) != value;
        
        UpdateFlags(result, carry, false);
        return result;
    }

    // Bit counting operations

    public static byte CountLeadingZeros(byte value)
    {
        if (value == 0)
        {
            UpdateFlags(8, false, false);  // All 8 bits are zero
            return 8;
        }

        byte count = 0;
        for (int i = 7; i >= 0; i--)
        {
            if ((value & (1 << i)) != 0)
                break;
            count++;
        }

        UpdateFlags(count, false, false);
        return count;
    }

    public static byte CountTrailingZeros(byte value)
    {
        if (value == 0)
        {
            UpdateFlags(8, false, false);  // All 8 bits are zero
            return 8;
        }

        byte count = 0;
        for (int i = 0; i < 8; i++)
        {
            if ((value & (1 << i)) != 0)
                break;
            count++;
        }

        UpdateFlags(count, false, false);
        return count;
    }

    public static byte CountOnes(byte value)
    {
        byte count = 0;
        
        // Brian Kernighan's algorithm for counting set bits
        byte temp = value;
        while (temp != 0)
        {
            count++;
            temp &= (byte)(temp - 1);  // Clear the lowest set bit
        }

        UpdateFlags(count, false, false);
        return count;
    }

    // Alternative POPCNT implementation using lookup table (faster for repeated calls)
    private static readonly byte[] PopCountTable = new byte[256];
    
    static ComplexMathUnit()
    {
        // Initialize population count lookup table
        for (int i = 0; i < 256; i++)
        {
            int count = 0;
            int value = i;
            while (value != 0)
            {
                count++;
                value &= value - 1;
            }
            PopCountTable[i] = (byte)count;
        }
    }

    public static byte CountOnesFast(byte value)
    {
        byte count = PopCountTable[value];
        UpdateFlags(count, false, false);
        return count;
    }

    // Additional utility operations

    public static byte FindFirstSet(byte value)
    {
        if (value == 0)
        {
            UpdateFlags(0, true, false);  // Set carry to indicate no bits set
            return 0;
        }

        byte position = 1;  // 1-indexed position
        for (int i = 0; i < 8; i++)
        {
            if ((value & (1 << i)) != 0)
            {
                UpdateFlags(position, false, false);
                return position;
            }
            position++;
        }

        // Should never reach here, but just in case
        UpdateFlags(0, true, false);
        return 0;
    }

    public static byte FindLastSet(byte value)
    {
        if (value == 0)
        {
            UpdateFlags(0, true, false);  // Set carry to indicate no bits set
            return 0;
        }

        byte position = 8;  // 1-indexed position from the left
        for (int i = 7; i >= 0; i--)
        {
            if ((value & (1 << i)) != 0)
            {
                UpdateFlags(position, false, false);
                return position;
            }
            position--;
        }

        // Should never reach here, but just in case
        UpdateFlags(0, true, false);
        return 0;
    }

    public static byte ReverseBits(byte value)
    {
        byte result = 0;
        for (int i = 0; i < 8; i++)
        {
            if ((value & (1 << i)) != 0)
            {
                result |= (byte)(1 << (7 - i));
            }
        }

        UpdateFlags(result, false, false);
        return result;
    }

    public static bool IsPowerOfTwo(byte value)
    {
        bool isPower = value != 0 && (value & (value - 1)) == 0;
        
        // Update zero flag based on the boolean result
        UpdateFlags((byte)(isPower ? 1 : 0), false, false);
        return isPower;
    }

    // Gray code operations
    public static byte BinaryToGray(byte binary)
    {
        byte gray = (byte)(binary ^ (binary >> 1));
        UpdateFlags(gray, false, false);
        return gray;
    }

    public static byte GrayToBinary(byte gray)
    {
        byte binary = gray;
        while (gray != 0)
        {
            gray >>= 1;
            binary ^= gray;
        }
        
        UpdateFlags(binary, false, false);
        return binary;
    }

    // Absolute value and sign operations
    public static byte AbsoluteValue(byte value)
    {
        // For signed interpretation of byte
        if ((value & 0x80) != 0)  // Negative in two's complement
        {
            // Two's complement negation: invert bits and add 1
            byte result = (byte)((~value) + 1);
            
            // Set overflow if we can't represent the absolute value (i.e., -128)
            bool overflow = value == 0x80;  // -128 in two's complement
            
            UpdateFlags(result, false, overflow);
            return result;
        }
        else
        {
            UpdateFlags(value, false, false);
            return value;
        }
    }

    public static byte Negate(byte value)
    {
        // Two's complement negation
        byte result = (byte)((~value) + 1);
        
        // Set overflow if negating -128 (0x80)
        bool overflow = value == 0x80;
        bool carry = value != 0;  // Carry set unless negating zero
        
        UpdateFlags(result, carry, overflow);
        return result;
    }
}


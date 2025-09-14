namespace Machine.Execution;

public static class ComplexMathUnit
{
    private static void UpdateFlags(byte result, bool carry = false, bool overflow = false)
    {
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

    // Additional operations

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

        // Should never reach here
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

        // Should never reach here
        UpdateFlags(0, true, false);
        return 0;
    }
}


using Coprocessor.Structures;

namespace Coprocessor.Execution;

public static class ComplexMathUnit
{
    public static byte MultiplyLow(byte a, byte b)
    {
        int result = a * b;
        byte lowResult = (byte)(result & 0xFF);
        
        return lowResult;
    }

    public static byte MultiplyHigh(byte a, byte b)
    {
        int result = a * b;
        byte highResult = (byte)((result >> 8) & 0xFF);
        
        return highResult;
    }

    public static (byte low, byte high) MultiplyFull(byte a, byte b)
    {
        int result = a * b;
        byte low = (byte)(result & 0xFF);
        byte high = (byte)((result >> 8) & 0xFF);
        
        return (low, high);
    }

    public static byte Divide(byte dividend, byte divisor)
    {
        if (divisor == 0)
        {
            // Division by zero - return max value
            return 0xFF;
        }

        byte result = (byte)(dividend / divisor);

        return result;
    }

    public static byte Modulo(byte dividend, byte divisor)
    {
        if (divisor == 0)
        {
            // Modulo by zero - return dividend
            return dividend;
        }

        byte result = (byte)(dividend % divisor);

        return result;
    }

    public static byte SquareRoot(byte value)
    {
        if (value == 0)
        {
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
        
        return result;
    }
}


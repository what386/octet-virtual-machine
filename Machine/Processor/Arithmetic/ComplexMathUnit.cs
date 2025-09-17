
using Machine.Processor.Registers;

namespace Machine.Processor.Arithmetic;

public class ComplexMathUnit
{
    private StatusRegister flagsRegister;

    public ComplexMathUnit(StatusRegister flagRegister)
    {
        this.flagsRegister = flagRegister;
    }

    private void UpdateFlags(byte result)
    {
        // carry will never occur (255 x 255  !> 65536)
        // both aux carry and overflow are undefined
        flagsRegister.UpdateFlags(result, false, false, false);
    }

    // Multiplication operations

    public byte MultiplyLow(byte a, byte b)
    {
        int result = a * b;
        byte lowResult = (byte)(result & 0xFF);

        UpdateFlags(lowResult);
        return lowResult;
    }

    public byte MultiplyHigh(byte a, byte b)
    {
        int result = a * b;
        byte highResult = (byte)((result >> 8) & 0xFF);

        bool auxCarry = ((a & 0x0F) * (b & 0x0F)) > 0x0F;

        UpdateFlags(highResult);
        return highResult;
    }

    public (byte low, byte high) MultiplyFull(byte a, byte b)
    {
        int result = a * b;
        byte low = (byte)(result & 0xFF);
        byte high = (byte)((result >> 8) & 0xFF);

        bool auxCarry = ((a & 0x0F) * (b & 0x0F)) > 0x0F;

        UpdateFlags(low);
        return (low, high);
    }

    // Division operations

    public byte Divide(byte dividend, byte divisor)
    {
        if (divisor == 0)
            throw new System.DivideByZeroException();

        byte result = (byte)(dividend / divisor);
        UpdateFlags(result);
        return result;
    }

    public byte Modulo(byte dividend, byte divisor)
    {
        if (divisor == 0)
            throw new System.DivideByZeroException();

        byte result = (byte)(dividend % divisor);
        UpdateFlags(result);
        return result;
    }

    public (byte quotient, byte remainder) DivideWithRemainder(byte dividend, byte divisor)
    {
        if (divisor == 0)
            throw new System.DivideByZeroException();

        byte quotient = (byte)(dividend / divisor);
        byte remainder = (byte)(dividend % divisor);

        UpdateFlags(quotient);
        return (quotient, remainder);
    }

    // Advanced mathematical operations

    public byte SquareRoot(byte value)
    {
        if (value == 0)
        {
            UpdateFlags(0);
            return 0;
        }

        byte result = 0;
        byte bit = 0x80;

        while (bit > result)
        {
            byte temp = (byte)(result | bit);
            if (temp * temp <= value)
                result = temp;
            bit >>= 1;
        }

        UpdateFlags(result);
        return result;
    }

    // Bit counting

    public byte CountLeadingZeros(byte value)
    {
        if (value == 0)
        {
            UpdateFlags(8);
            return 8;
        }

        byte count = 0;
        for (int i = 7; i >= 0; i--)
        {
            if ((value & (1 << i)) != 0)
                break;
            count++;
        }

        UpdateFlags(count);
        return count;
    }

    public byte CountTrailingZeros(byte value)
    {
        if (value == 0)
        {
            UpdateFlags(8);
            return 8;
        }

        byte count = 0;
        for (int i = 0; i < 8; i++)
        {
            if ((value & (1 << i)) != 0)
                break;
            count++;
        }

        UpdateFlags(count);
        return count;
    }

    public byte CountOnes(byte value)
    {
        byte count = 0;
        byte temp = value;

        while (temp != 0)
        {
            count++;
            temp &= (byte)(temp - 1);
        }

        UpdateFlags(count);
        return count;
    }
}

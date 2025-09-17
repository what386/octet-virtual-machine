using Machine.CPU.Core;

namespace Machine.CPU.Arithmetic;

public static class ArithmeticLogicUnit
{
    // ---Arithmetic---
    public static byte Add(byte a, byte b)
    {
        int result = a + b;
        byte byteResult = (byte)result;
        
        bool carry = result > 255;
        bool auxCarry = ((a & 0x0F) + (b & 0x0F)) > 0x0F;
        
        // Check for signed overflow (two positive numbers yielding negative, or two negative yielding positive)
        bool overflow = ((a & 0x80) == (b & 0x80)) && ((a & 0x80) != (byteResult & 0x80));
        
        StatusFlags.UpdateFlags(byteResult, carry, auxCarry, overflow);
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
        
        StatusFlags.UpdateFlags(byteResult, carry, auxCarry, overflow);
        return byteResult;
    }

    public static byte Increment(byte a)
    {
        int result = a + 1;
        byte byteResult = (byte)result;
        
        bool auxCarry = (a & 0x0F) == 0x0F;
        bool overflow = a == 0x7F;  // Incrementing max positive signed value
       
        // Increment does not affect the carry flag
        StatusFlags.UpdateFlags(byteResult, StatusFlags.CarryFlag, auxCarry, overflow);
        return byteResult;
    }

    public static byte Decrement(byte a)
    {
        int result = a - 1;
        byte byteResult = (byte)result;
        
        bool auxCarry = (a & 0x0F) == 0x00;
        bool overflow = a == 0x80;  // Decrementing max negative signed value
        
        // Decrement does not affect the carry flag
        StatusFlags.UpdateFlags(byteResult, StatusFlags.CarryFlag, auxCarry, overflow);
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
        StatusFlags.UpdateFlags(result, false, false, false);  // AND clears carry, aux carry, and overflow
        return result;
    }

    public static byte Or(byte a, byte b)
    {
        byte result = (byte)(a | b);
        StatusFlags.UpdateFlags(result, false, false, false);  // OR clears carry, aux carry, and overflow
        return result;
    }

    public static byte Xor(byte a, byte b)
    {
        byte result = (byte)(a ^ b);
        StatusFlags.UpdateFlags(result, false, false, false);  // XOR clears carry, aux carry, and overflow
        return result;
    }

    public static byte Not(byte a)
    {
        byte result = (byte)~a;
        StatusFlags.UpdateFlags(result, false, false, false);  // NOT clears carry, aux carry, and overflow
        return result;
    }

    public static byte Implies(byte a, byte b)
    {
        byte result = (byte)(~a | b);
        StatusFlags.UpdateFlags(result, false, false, false);  // Logical operation clears carry, aux carry, and overflow
        return result;
    }
}


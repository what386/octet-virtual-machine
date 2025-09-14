using System;

namespace Machine.Execution;

public static class BitShiftUnit
{
    private static void UpdateFlags(byte result, bool carry)
    {
        ArithmeticLogicUnit.UpdateFlagsExternal(result, carry, false, false);
    }

    // Left shift operations

    public static byte ShiftLeft(byte value, int positions)
    {
        if (positions < 0 || positions > 7)
            throw new ArgumentOutOfRangeException(nameof(positions), "Shift positions must be between 0 and 7");

        if (positions == 0)
        {
            // No shift, but still update flags
            UpdateFlags(value, ArithmeticLogicUnit.CarryFlag);
            return value;
        }

        bool carry = false;
        byte result = value;

        for (int i = 0; i < positions; i++)
        {
            carry = (result & 0x80) != 0;  // Capture bit 7 before shifting
            result = (byte)(result << 1);
        }

        UpdateFlags(result, carry);
        return result;
    }

    public static byte ShiftLeftLogical(byte value, int positions)
    {
        // Same as ShiftLeft for byte operations
        return ShiftLeft(value, positions);
    }

    // Right shift operations

    public static byte ShiftRightLogical(byte value, int positions)
    {
        if (positions < 0 || positions > 7)
            throw new ArgumentOutOfRangeException(nameof(positions), "Shift positions must be between 0 and 7");

        if (positions == 0)
        {
            // No shift, but still update flags
            UpdateFlags(value, ArithmeticLogicUnit.CarryFlag);
            return value;
        }

        bool carry = false;
        byte result = value;

        for (int i = 0; i < positions; i++)
        {
            carry = (result & 0x01) != 0;  // Capture bit 0 before shifting
            result = (byte)(result >> 1);
        }

        UpdateFlags(result, carry);
        return result;
    }

    public static byte ShiftRightArithmetic(byte value, int positions)
    {
        if (positions < 0 || positions > 7)
            throw new ArgumentOutOfRangeException(nameof(positions), "Shift positions must be between 0 and 7");

        if (positions == 0)
        {
            // No shift, but still update flags
            UpdateFlags(value, ArithmeticLogicUnit.CarryFlag);
            return value;
        }

        bool carry = false;
        byte result = value;
        bool signBit = (value & 0x80) != 0;  // Preserve sign bit

        for (int i = 0; i < positions; i++)
        {
            carry = (result & 0x01) != 0;  // Capture bit 0 before shifting
            result = (byte)(result >> 1);
            if (signBit)
                result |= 0x80;  // Fill with sign bit
        }

        UpdateFlags(result, carry);
        return result;
    }

    // Rotate operations

    public static byte RotateLeft(byte value, int positions)
    {
        if (positions < 0 || positions > 7)
            throw new ArgumentOutOfRangeException(nameof(positions), "Rotate positions must be between 0 and 7");

        if (positions == 0)
        {
            // No rotation, but still update flags
            UpdateFlags(value, ArithmeticLogicUnit.CarryFlag);
            return value;
        }

        // Normalize positions to 0-7 range (8 positions = no change)
        positions = positions % 8;
        
        byte result = (byte)((value << positions) | (value >> (8 - positions)));
        
        // Carry flag gets the last bit that was rotated out (bit 7 of original after positions-1 rotations)
        bool carry = positions > 0 ? ((value >> (8 - positions)) & 0x01) != 0 : false;
        
        UpdateFlags(result, carry);
        return result;
    }

    public static byte RotateRight(byte value, int positions)
    {
        if (positions < 0 || positions > 7)
            throw new ArgumentOutOfRangeException(nameof(positions), "Rotate positions must be between 0 and 7");

        if (positions == 0)
        {
            // No rotation, but still update flags
            UpdateFlags(value, ArithmeticLogicUnit.CarryFlag);
            return value;
        }

        // Normalize positions to 0-7 range (8 positions = no change)
        positions = positions % 8;
        
        byte result = (byte)((value >> positions) | (value << (8 - positions)));
        
        // Carry flag gets the last bit that was rotated out (bit 0 of original after positions-1 rotations)
        bool carry = positions > 0 ? ((value >> (positions - 1)) & 0x01) != 0 : false;
        
        UpdateFlags(result, carry);
        return result;
    }

    // Rotate through carry operations (9-bit rotations including carry flag)

    public static byte RotateLeftThroughCarry(byte value, int positions)
    {
        if (positions < 0 || positions > 7)
            throw new ArgumentOutOfRangeException(nameof(positions), "Rotate positions must be between 0 and 7");

        if (positions == 0)
        {
            // No rotation, but still update flags
            UpdateFlags(value, ArithmeticLogicUnit.CarryFlag);
            return value;
        }

        bool carry = ArithmeticLogicUnit.CarryFlag;
        byte result = value;

        for (int i = 0; i < positions; i++)
        {
            bool newCarry = (result & 0x80) != 0;
            result = (byte)((result << 1) | (carry ? 1 : 0));
            carry = newCarry;
        }

        UpdateFlags(result, carry);
        return result;
    }

    public static byte RotateRightThroughCarry(byte value, int positions)
    {
        if (positions < 0 || positions > 7)
            throw new ArgumentOutOfRangeException(nameof(positions), "Rotate positions must be between 0 and 7");

        if (positions == 0)
        {
            // No rotation, but still update flags
            UpdateFlags(value, ArithmeticLogicUnit.CarryFlag);
            return value;
        }

        bool carry = ArithmeticLogicUnit.CarryFlag;
        byte result = value;

        for (int i = 0; i < positions; i++)
        {
            bool newCarry = (result & 0x01) != 0;
            result = (byte)((result >> 1) | (carry ? 0x80 : 0));
            carry = newCarry;
        }

        UpdateFlags(result, carry);
        return result;
    }
}

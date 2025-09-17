namespace Machine.CPU.Arithmetic;

public class ArithmeticManager 
{
    public enum Opcodes
    {
        Add, Subtract, And, Or, Xor, Implies, Compare, Test,
        Multiply, MultiplyUpper, Divide, Modulo, SquareRoot, CountLeadingZeros, CountTrailingZeros, Popcnt,
        LogicalShiftLeft, LogicalShiftRight, RotateRight, ArithmeticShiftRight,
    }
    
    public byte PerformOperation(Opcodes opcode, byte a, byte b)
    {
        return opcode switch
        {
            // Arithmetic operations
            Opcodes.Add => ArithmeticLogicUnit.Add(a, b),
            Opcodes.Subtract => ArithmeticLogicUnit.Sub(a, b),
            
            // Bitwise operations
            Opcodes.And => ArithmeticLogicUnit.And(a, b),
            Opcodes.Or => ArithmeticLogicUnit.Or(a, b),
            Opcodes.Xor => ArithmeticLogicUnit.Xor(a, b),
            Opcodes.Implies => ArithmeticLogicUnit.Implies(a, b),

            // Complex math operations
            Opcodes.Multiply => ComplexMathUnit.MultiplyLow(a, b),
            Opcodes.MultiplyUpper => ComplexMathUnit.MultiplyHigh(a, b),
            Opcodes.Divide => ComplexMathUnit.Divide(a, b),
            Opcodes.Modulo => ComplexMathUnit.Modulo(a, b),
            Opcodes.SquareRoot => ComplexMathUnit.SquareRoot(a), // Note: only uses 'a', ignores 'b'
            Opcodes.CountLeadingZeros => ComplexMathUnit.CountLeadingZeros(a), // Note: only uses 'a', ignores 'b'
            Opcodes.CountTrailingZeros => ComplexMathUnit.CountTrailingZeros(a), // Note: only uses 'a', ignores 'b'
            Opcodes.Popcnt => ComplexMathUnit.CountOnes(a), // Note: only uses 'a', ignores 'b'
            
            // Bit shift operations (using 'b' as shift amount, but BitShiftUnit expects int)
            Opcodes.LogicalShiftLeft => BitShiftUnit.ShiftLeftLogical(a, b),
            Opcodes.LogicalShiftRight => BitShiftUnit.ShiftRightLogical(a, b),
            Opcodes.ArithmeticShiftRight => BitShiftUnit.ShiftRightArithmetic(a, b),
            Opcodes.RotateRight => BitShiftUnit.RotateRight(a, b),
            
            _ => throw new System.ArgumentException($"Unsupported ALU operation: {opcode}")
        };
    }

    // Convenience method for one-operand arithmetic (currently only the NOT operator)
    public byte PerformOperation(Opcodes opcode, byte a) => PerformOperation(opcode, a, 0);
}


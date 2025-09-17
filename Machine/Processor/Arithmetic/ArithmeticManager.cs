using Machine.Processor.Registers;

namespace Machine.Processor.Arithmetic;

public class ArithmeticManager 
{
    private StatusRegister flagsRegister;
    private ArithmeticLogicUnit ALU;
    private ComplexMathUnit CMU;
    private BitShiftUnit BSU;

    public ArithmeticManager(StatusRegister flagsRegister)
    {
        this.flagsRegister = flagsRegister;
        ALU = new ArithmeticLogicUnit(flagsRegister);
        CMU = new ComplexMathUnit(flagsRegister);
        BSU = new BitShiftUnit(flagsRegister);
    }

    public byte PerformOperation(Opcode opcode, byte a, byte b)
    {
        return opcode switch
        {
            // Arithmetic operations
            Opcode.Add => ALU.Add(a, b),
            Opcode.Subtract => ALU.Sub(a, b),
            
            // Bitwise operations
            Opcode.And => ALU.And(a, b),
            Opcode.Or => ALU.Or(a, b),
            Opcode.Xor => ALU.Xor(a, b),
            Opcode.Implies => ALU.Implies(a, b),

            // Complex math operations
            Opcode.Multiply => CMU.MultiplyLow(a, b),
            Opcode.MultiplyUpper => CMU.MultiplyHigh(a, b),
            Opcode.Divide => CMU.Divide(a, b),
            Opcode.Modulo => CMU.Modulo(a, b),
            Opcode.SquareRoot => CMU.SquareRoot(a), // Note: only uses 'a', ignores 'b'
            Opcode.CountLeadingZeros => CMU.CountLeadingZeros(a), // Note: only uses 'a', ignores 'b'
            Opcode.CountTrailingZeros => CMU.CountTrailingZeros(a), // Note: only uses 'a', ignores 'b'
            Opcode.Popcnt => CMU.CountOnes(a), // Note: only uses 'a', ignores 'b'
            
            // Bit shift operations (using 'b' as shift amount, but BSU expects int)
            Opcode.LogicalShiftLeft => BSU.ShiftLeftLogical(a, b),
            Opcode.LogicalShiftRight => BSU.ShiftRightLogical(a, b),
            Opcode.ArithmeticShiftRight => BSU.ShiftRightArithmetic(a, b),
            Opcode.RotateRight => BSU.RotateRight(a, b),
            
            _ => throw new System.ArgumentException($"Unsupported ALU operation: {opcode}")
        };
    }

    // Convenience method for one-operand arithmetic
    // currently only the NOT operator
    public byte PerformOperation(Opcode opcode, byte a) => PerformOperation(opcode, a, 0);
}


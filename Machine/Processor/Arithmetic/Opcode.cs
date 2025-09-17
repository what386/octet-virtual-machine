namespace Machine.Processor.Arithmetic;

public enum Opcode
{
    Add, Subtract, And, Or, Xor, Implies, Compare, Test,
    Multiply, MultiplyUpper, Divide, Modulo, SquareRoot, CountLeadingZeros, CountTrailingZeros, Popcnt,
    LogicalShiftLeft, LogicalShiftRight, RotateRight, ArithmeticShiftRight,
}

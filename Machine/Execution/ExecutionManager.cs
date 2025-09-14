namespace Machine.Execution;

public class ExecutionManager 
{
    const int registerFileSize = 8;
    public byte flags;

    RegisterFile registers = new RegisterFile(8);

    private enum ALUOps
    {
        Add, Subtract, And, Or, Xor, Implies,
        Multiply, MultiplyUpper, Divide, SquareRoot, CountLeadingZeros, CountTrailingZeros, Popcnt,
        LogicalShiftLeft, LogicalShiftRight, RotateRight, ArithmeticShiftRight,
    }

    
}

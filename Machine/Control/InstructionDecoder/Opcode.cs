namespace Machine.Control.InstructionDecoder;

public enum Opcode 
{
    NoOperation,
    Halt,
    System,
    ConditionalLoadImm,
    Jump,
    Branch,
    Call,
    Return,
    Input,
    Output,
    SpecialLoad,
    SpecialStore,
    PopStack,
    PushStack,
    MemoryLoad,
    MemoryStore,
    LoadImmediate,
    Move,
    AddImmediate,
    AndImmediate,
    CompareImmediate,
    TestImmediate,
    Add,
    Subtract,
    Bitwise,
    InvBitwise,
    BarrelShift,
    BarrelShiftImmediate,
    MultiplyDivide,
    BitCount,
    OutputImmediate,
    CoProcessorCommand
}


namespace Machine.Control.InstructionDecoder;

public enum InstructionType
{
    Nop,
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
    ArithmeticImmediate,
    CompareImmediate,
    TestImmediate,
    Arithmetic,
    Bitwise,
    InverseBitwise,
    BarrelShift,
    BarrelShiftImmediate,
    MultiplyDivide,
    BitCount,
    OutputImmediate,
    CoProcessor
}

public enum ArithmeticOperation
{
    Add,
    Subtract,
    And,
    Or,
    Xor,
    Implies
}

// Branch conditions (3-bit field)
public enum BranchCondition : byte
{
    // Regular conditions
    Equal = 0,           // Z
    NotEqual = 1,        // !Z
    Lower = 2,           // !C
    Higher = 3,          // C AND !Z
    LowerSame = 4,       // !C OR Z
    HigherSame = 5,      // C
    Even = 6,            // E
    Always = 7,          // Always

    // Alternate conditions (when SYS sets alternate mode)
    Overflow = 0,        // V
    NoOverflow = 1,      // !V
    Less = 2,            // N!=V
    Greater = 3,         // N=V AND !Z
    LessEqual = 4,       // N!=V OR Z
    GreaterEqual = 5,    // N=V
    Odd = 6,             // !E
    // Always = 7 (same as regular)
}

// Branch types (2-bit field)
public enum BranchType : byte
{
    AssumeNothing = 0,   // BRA
    AssumeNotTaken = 1,  // BRN
    AssumeTaken = 2,     // BRT
    BranchToPointer = 3  // BRP
}

// Stack operation types (2-bit field)
public enum StackType : byte
{
    Pop = 0,             // POP
    Peek = 1,            // PEEK
    PopFlags = 2,        // POPF
    DecrementSP = 3      // DSP
}

public enum PushType : byte
{
    Push = 0,            // PSH
    Poke = 1,            // POKE
    PushFlags = 2,       // PSHF
    IncrementSP = 3      // ISP
}

// Special registers (3-bit field)
public enum SpecialRegister : byte
{
    AddressPointer = 0,
    StackPointer = 1,
    LoopPointer = 2,
    FlagsRegister = 3,
    Reserved4 = 4,
    BranchOffset = 5,
    ProgramCounterLow = 6,
    ProgramCounterHigh = 7
}

// ADD operation types (3-bit field)
public enum AddType : byte
{
    Add = 0,             // ADD - standard add
    AddWithCarry = 1,    // ADDC - add with carry
    AddVector = 2,       // ADDV - add 4-bit nibbles (no carry bit 3→4)
    AddVectorCarry = 3   // ADDVC - add vector with carry
}

// SUB operation types (3-bit field)
public enum SubType : byte
{
    Subtract = 0,        // SUB - standard subtract
    SubWithBorrow = 1,   // SUBB - subtract with borrow
    SubVector = 2,       // SUBV - subtract 4-bit nibbles
    SubVectorBorrow = 3  // SUBVB - subtract vector with borrow
}

// Bitwise operation types (2-bit field)
public enum BitwiseType : byte
{
    Or = 0,              // OR
    And = 1,             // AND
    Xor = 2,             // XOR
    Implies = 3          // IMP
}

// Inverse bitwise operation types (2-bit field)
public enum InverseBitwiseType : byte
{
    Nor = 0,             // NOR
    Nand = 1,            // NAND
    Xnor = 2,            // XNOR
    NotImplies = 3       // NIMP
}

// Barrel shift types (2-bit field)
public enum BarrelShiftType : byte
{
    LogicalShiftLeft = 0,    // BSL
    LogicalShiftRight = 1,   // BSR
    RotateRight = 2,         // ROR
    ArithmeticShiftRight = 3 // BSXR
}

// Multiply/Divide types (2-bit field)
public enum MultiplyDivideType : byte
{
    MultiplyLow = 0,     // MUL - save low bits
    MultiplyHigh = 1,    // MULU - save high bits
    Divide = 2,          // DIV
    Modulo = 3           // MOD
}

// Bit count types (2-bit field)
public enum BitCountType : byte
{
    SquareRoot = 0,          // SQRT
    CountLeadingZeros = 1,   // CLZ
    CountTrailingZeros = 2,  // CTZ
    CountOnes = 3            // CTO, POPCNT
}

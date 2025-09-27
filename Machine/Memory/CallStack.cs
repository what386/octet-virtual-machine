namespace Machine.Memory;

public class CallStack
{
    const int stackSize = 32;

    byte stackPointer = 0;

    ushort[] stack = new ushort[stackSize];

    public void Push(ushort data)
    {
        stack[stackPointer] = data;

        stackPointer++;
        
        if (stackPointer is >= stackSize)
            throw new System.IndexOutOfRangeException();
    }

    public ushort Pop()
    {
        if (stackPointer == 0)
            throw new System.IndexOutOfRangeException();

        stackPointer--;

        return stack[stackPointer];
    }
}

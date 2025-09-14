using System;

namespace Machine.Storage;

public class InstructionMemory

{
    private readonly int size;
    private readonly int blockSize;
    private ushort[] memory;

    public InstructionMemory(int size, int blockSize)
    {
        this.size = size;
        this.blockSize = blockSize;
        this.memory = new ushort[size];
    }

    public int GetSize() => memory.Length;

    public ushort[] ReadPage(int index)
    {
        int offset = index * blockSize;

        ushort[] block = new ushort[blockSize];
        Array.Copy(memory, offset, block, 0, blockSize);
        return block;
    }
        
    public void Flash(ushort[] data) => memory = data;

    public void Clear() => Array.Clear(memory, 0, size);
}


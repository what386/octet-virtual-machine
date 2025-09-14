using System;
using Coprocessor.Structures;

namespace Coprocessor.Storage;

public class DataMemory

{
    private int size;
    private int blockSize;
    private Vector[] memory;

    // RAM should only ever be accessed by the cache
    // so you can only directly interact with memory
    // through transfers of entire block-size chunks

    public DataMemory(int size, int blockSize)
    {
        this.size = size;
        this.blockSize = blockSize;
        this.memory = new Vector[size];
    }

    public Vector[] ReadPage(int index)
    {
        int offset = index * blockSize;

        Vector[] block = new Vector[blockSize];
        Array.Copy(memory, offset, block, 0, blockSize);
        return block;
    }
        
    public void WritePage(int index, Vector[] block)
    {
        int offset = index * blockSize;

        Array.Copy(block, 0, memory, offset, blockSize);
    }

    public void Clear() => Array.Clear(memory, 0, size);
}


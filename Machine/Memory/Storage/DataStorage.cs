 sing System;

namespace Machine.Memory.Storage;

public class DataStorage

{
    private int size;
    private int blockSize;
    private byte[] memory;

    // RAM should only ever be accessed by the cache
    // so you can only directly interact with memory
    // through transfers of entire block-size chunks

    public DataStorage(int size, int blockSize)
    {
        this.size = size;
        this.blockSize = blockSize;
        this.memory = new byte[size];
    }

    public byte[] ReadPage(int index)
    {
        int offset = index * blockSize;

        byte[] block = new byte[blockSize];
        Array.Copy(memory, offset, block, 0, blockSize);
        return block;
    }
        
    public void WritePage(int index, byte[] block)
    {
        int offset = index * blockSize;

        Array.Copy(block, 0, memory, offset, blockSize);
    }

    public void Clear() => Array.Clear(memory, 0, size);
}


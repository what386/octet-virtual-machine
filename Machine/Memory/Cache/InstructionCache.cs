using System;

namespace Machine.Memory.Cache;

public class InstructionCache
{
    private readonly int size;
    private ushort[] cache;

    public InstructionCache(int size)
    {
        this.size = size;
        this.cache = new ushort[size];
    }

    public ushort Read(int index) => cache[index];

    public ushort[] ReadBlock() => cache;
    public void WriteBlock(ushort[] block) => cache = block;

    public void Clear() => Array.Clear(cache, 0, size);
}

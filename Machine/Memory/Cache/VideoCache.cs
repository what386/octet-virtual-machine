using System;

namespace Machine.Memory.Cache;

public class VideoCache
{
    private readonly int size;
    private byte[] cache;

    public VideoCache(int size)
    {
        this.size = size;
        this.cache = new byte[size];
    }

    public byte Read(int index) => cache[index];
    public void Write(int index, byte data) => cache[index] = data;

    public byte[] ReadBlock() => cache;
    public void WriteBlock(byte[] block) => cache = block;

    public void Clear() => Array.Clear(cache, 0, size);
}

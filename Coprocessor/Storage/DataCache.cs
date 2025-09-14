using System;
using Coprocessor.Structures;

namespace Coprocessor.Storage;

public class DataCache
{
    private readonly int size;
    private Vector[] cache;

    public DataCache(int size)
    {
        this.size = size;
        this.cache = new Vector[size];
    }

    public Vector Read(int index) => cache[index];
    public void Write(int index, Vector data) => cache[index] = data;

    public Vector[] ReadBlock() => cache;
    public void WriteBlock(Vector[] block) => cache = block;

    public void Clear() => Array.Clear(cache, 0, size);
}

using System;

namespace Coprocessor.Structures;

public class Vector
{
    public const int length = 4;
    private byte[] value;
    
    public Vector()
    {
        this.value = new byte[length];
    }
       
    public byte this[int index]
    {
        get
        {
            if (index < 0 || index >= length)
                throw new IndexOutOfRangeException($"Index must be between 0 and {length - 1}");
            return value[index];
        }
        set
        {
            if (index < 0 || index >= length)
                throw new IndexOutOfRangeException($"Index must be between 0 and {length - 1}");
            this.value[index] = value;
        }
    }
}

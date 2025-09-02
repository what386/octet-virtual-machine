using System;

namespace Machine.Processor;

public class RegisterFile 
{
    private readonly int size;
    private readonly byte[] registers;

    public RegisterFile(int size)
    {
        if (size <= 0)
            throw new ArgumentOutOfRangeException(nameof(size), "Register file must have a positive size.");

        this.size = size;
        registers = new byte[size];
    }

    public byte Read(int index)
    {
        if (index == 0) 
            return 0; // Always 0

        return registers[index];
    }

    public void Write(int index, byte data)
    {
        if (index == 0) 
            return; // Ignore writes

        registers[index] = data;
    }

    public void Clear()
    {
        Array.Clear(registers, 0, size);
        // Ensure register 0 stays zero (redundant, but explicit for clarity)
        registers[0] = 0;
    }
}


using System;
using Coprocessor.Structures;

namespace Coprocessor.Execution;

public class RegisterFile 
{
    private readonly int size;
    private readonly Vector[] registers;

    public RegisterFile(int size)
    {
        if (size <= 0)
            throw new ArgumentOutOfRangeException(nameof(size), "Register file must have a positive size.");

        this.size = size;
        registers = new Vector[size];
    }

    public Vector Read(int index) => registers[index];

    public void Write(int index, Vector data) => registers[index] = data;

    public void Clear() => Array.Clear(registers, 0, size);
    
}


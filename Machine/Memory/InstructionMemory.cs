using System;
using Machine.Memory.Storage;
using Machine.Memory.Cache;

namespace Machine.Memory;

public class InstructionMemory 
{
    const int memSize = 2048;
    const int cacheSize = 32;
    const int numPages = memSize / cacheSize;

    public int activePage {get; private set;} = 0; 

    InstructionStorage memory = new InstructionStorage(memSize, cacheSize);
    InstructionCache cache = new InstructionCache(cacheSize);

    public void SwitchPage(int pageNum)
    {
        //TODO: Add consistent time penalty for a page swap
        cache.WriteBlock(memory.ReadPage(pageNum));

        activePage = pageNum;
    }

    public ushort ReadInstruction(int address)
    {
        if (address is < 0 or >= memSize)
            throw new ArgumentException();

        int selectedPage = address / cacheSize;

        if (selectedPage != activePage) SwitchPage(selectedPage);

        int blockOffset= address % cacheSize;

        return cache.Read(blockOffset);
    }

    public void FlashMemory(ushort[] data)
    {
        if (data.Length != memSize)
            throw new ArgumentException();

        memory.Flash(data);

        SwitchPage(0);
    }
}

using System;
using Machine.Memory.Storage;
using Machine.Memory.Cache;

namespace Machine.Memory;

public class DataMemory
{
    const int memSize = 256;
    const int cacheSize = 64;
    const int numPages = memSize / cacheSize;
    const int stackPage = numPages - 1;

    public int activePage {get; private set;} = 0; 

    public byte stackPointer {get; private set;} = 31;

    DataStorage memory = new DataStorage(memSize, cacheSize);
    DataCache cache = new DataCache(cacheSize);

    public void SwitchPage(int pageNum)
    {
        //TODO: Add consistent cycle penalty for a page swap
        memory.WritePage(activePage, cache.ReadBlock());
        cache.WriteBlock(memory.ReadPage(pageNum));

        activePage = pageNum;
    }

    public void Push(byte data, int offset)
    {
        if (activePage != stackPage) SwitchPage(stackPage);

        cache.Write(stackPointer + offset, data);

        stackPointer++;
        
        if (stackPointer is < 31 or > 63)
            throw new IndexOutOfRangeException();
    }

    public byte Pop(int offset)
    {
        if (activePage != stackPage) SwitchPage(stackPage);

        byte data = cache.Read(stackPointer + offset);

        stackPointer--;

        if (stackPointer is < 31 or > 63)
            throw new IndexOutOfRangeException();

        return data;
    }

    public byte Read(int address)
    {
        if (address is < 0 or > memSize - 1)
            throw new ArgumentException();

        int selectedPage = address / cacheSize;

        if (selectedPage != activePage) SwitchPage(selectedPage);

        int blockOffset= address % cacheSize;

        return cache.Read(blockOffset);
    }

    public void Write(int address, byte data)
    {
        if (address is < 0 or >= memSize)
            throw new ArgumentException();

        int selectedPage = address / cacheSize;

        if (selectedPage != activePage) SwitchPage(selectedPage);

        int blockOffset= address % cacheSize;

        cache.Write(blockOffset, data);
    }
}

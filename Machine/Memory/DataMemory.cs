using System;
using Machine.Memory.Storage;
using Machine.Memory.Cache;

namespace Machine.Memory;

public class DataMemory
{
    const int memSize = 256;
    const int cacheSize = 64;
    const int numPages = memSize / cacheSize;

    public int activePage {get; private set;} = 0; 

    DataStorage memory = new DataStorage(memSize, cacheSize);
    DataCache cache = new DataCache(cacheSize);

    public void SwitchPage(int pageNum)
    {
        //TODO: Add consistent time penalty for a page swap
        memory.WritePage(activePage, cache.ReadBlock());
        cache.WriteBlock(memory.ReadPage(pageNum));

        activePage = pageNum;
    }

    public byte ReadMemory(int address)
    {
        if (address is < 0 or > memSize - 1)
            throw new ArgumentException();

        int selectedPage = address / cacheSize;

        if (selectedPage != activePage) SwitchPage(selectedPage);

        int blockOffset= address % cacheSize;

        return cache.Read(blockOffset);
    }

    public void WriteMemory(int address, byte data)
    {
        if (address is < 0 or >= memSize)
            throw new ArgumentException();

        int selectedPage = address / cacheSize;

        if (selectedPage != activePage) SwitchPage(selectedPage);

        int blockOffset= address % cacheSize;

        cache.Write(blockOffset, data);
    }
}

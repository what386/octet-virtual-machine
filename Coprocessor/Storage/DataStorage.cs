using System;
using Coprocessor.Structures;

namespace Coprocessor.Storage;

public class DataStorage
{
    const int memSize = 64;
    const int cacheSize = 16;
    const int numPages = memSize / cacheSize;

    public int activePage {get; private set;} = 0; 

    DataMemory memory = new DataMemory(memSize, cacheSize);
    DataCache cache = new DataCache(cacheSize);

    public void SwitchPage(int pageNum)
    {
        //TODO: Add consistent time penalty for a page swap
        memory.WritePage(activePage, cache.ReadBlock());
        cache.WriteBlock(memory.ReadPage(pageNum));

        activePage = pageNum;
    }

    public Vector ReadMemory(int address)
    {
        if (address is < 0 or > memSize - 1)
            throw new ArgumentException();

        int selectedPage = address / cacheSize;

        if (selectedPage != activePage) SwitchPage(selectedPage);

        int blockOffset= address % cacheSize;

        return cache.Read(blockOffset);
    }

    public void WriteMemory(int address, Vector data)
    {
        if (address is < 0 or >= memSize)
            throw new ArgumentException();

        int selectedPage = address / cacheSize;

        if (selectedPage != activePage) SwitchPage(selectedPage);

        int blockOffset= address % cacheSize;

        cache.Write(blockOffset, data);
    }
}

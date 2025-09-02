using System.Collections.Generic;

namespace Machine.IO;

public abstract class IODevice
{
    public List<IOPort> connectedPorts;

    public void SendInterrupt(int interruptCode, int portID) => connectedPorts[portID].SendInterrupt(interruptCode);
    public void SendReady(bool ready, int portID) => connectedPorts[portID].deviceReady = ready;

    public abstract void Update(byte input, IOPort sender);
}

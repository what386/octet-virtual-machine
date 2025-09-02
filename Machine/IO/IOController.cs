namespace Machine.IO;

public class IOController
{
    const int numPorts = 8;

    IOPort[] ports = new IOPort[numPorts];

    public IOController()
    {
        for (int i = 0; i < numPorts; i++)
            ports[i] = new IOPort(this, i);
    }

    public void HandleInterrupt(IOPort sender, int interruptCode)
    {
        
    }

    public byte PortRead(int address)
    {
        if (address < 0 || address > numPorts - 1)
            throw new System.ArgumentException();

        return ports[address].GetFromPort();
    }

    public void PortWrite(int address, byte data)
    {
        if (address < 0 || address > numPorts -1)
            throw new System.ArgumentException();

        if(ports[address].portEnabled != true)
            throw new System.InvalidOperationException();

        ports[address].SendToPort(data);
    }

    public void ConnectPortToDevice(IOPort port, IODevice device) => port.ConnectDevice(device);
    public void DisconnectDevice(IODevice device) => device.connectedPort.DisconnectDevice(device);
}

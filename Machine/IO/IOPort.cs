namespace Machine.IO;

public class IOPort
{
    private IOController parentController; 
    private IODevice connectedDevice;

    private int portID;

    public bool deviceReady = false;
    public bool portEnabled = true;

    private byte inputBuffer = 0;
    private byte outputBuffer = 0;

    public IOPort(IOController controller, int id)
    {
        this.parentController = controller;
        this.portID = id;
    }

    public void ConnectDevice(IODevice device)
    {
        this.connectedDevice = device;
        device.connectedPorts.Add(this);
    }

    public void DisconnectDevice(IODevice device)
    {
        this.connectedDevice = null;
        device.connectedPorts.Remove(this);
    }

    public void UpdateDevice(byte input) => connectedDevice.Update(input, this);

    public void SendInterrupt(int interruptCode) 
    {
        if (portEnabled)
            parentController.HandleInterrupt(this, interruptCode);
    }

    public void SendToPort(byte data)
    {
        this.inputBuffer = data;
        UpdateDevice(this.inputBuffer);
    }

    public byte GetFromPort() => outputBuffer;
   
}

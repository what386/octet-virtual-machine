using System;
using Machine.Control.Interrupts;

namespace Machine.IO;

public class IOPort
{
    private IDevice? connectedDevice;
    public IDevice Device => connectedDevice;

    public bool isReady {get; private set; } = true;
    public bool isConnected => connectedDevice != null;
    
    public byte address { get; }
    private byte value = 0;
    
    public event EventHandler<InterruptEventArgs> InterruptRequested;
    
    public IOPort(byte address)
    {
        this.address = address;
    }
    
    public void ConnectDevice(IDevice device)
    {
        connectedDevice = device;
        connectedDevice.InterruptRequested += OnDeviceInterrupt;
    }
    
    public void DisconnectDevice()
    {
        if (connectedDevice != null)
        {
            connectedDevice.InterruptRequested -= OnDeviceInterrupt;
            connectedDevice = null;
        }
    }
    
    public byte Read() => this.value;
    public void Write(byte data) => this.value = data;

    private void OnDeviceInterrupt(object? sender, InterruptEventArgs e) => InterruptRequested?.Invoke(this, e);
    
    public void Reset()
    {
        this.value = 0;
        this.connectedDevice = null;
    }
}


using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Machine.Control.Interrupts;

namespace Machine.IO;

public class IOController
{
    public const int numPorts = 256;

    private readonly IOPort[] ports = new IOPort[numPorts];
    private readonly Queue<InterruptEventArgs> pendingInterrupts = new();
    private readonly object interruptLock = new();

    public event EventHandler<InterruptEventArgs> InterruptRequested;

    public IOController()
    {
        for (int i = 0; i < numPorts; i++)
        {
            ports[i] = new IOPort((byte)i);
            ports[i].InterruptRequested += OnPortInterrupt;
        }
    }

    public void ConnectDevice(IDevice device)
    {
        var port = ports[device.PortAddress];
        port.ConnectDevice(device);
    }

    public void DisconnectDevice(byte portAddress)
    {
        ports[portAddress].DisconnectDevice();
    }

    public bool IsPortReady(byte address) => ports[address].isReady;
    public bool IsPortConnected(byte address) => ports[address].isConnected;

    private void OnPortInterrupt(object sender, InterruptEventArgs e)
    {
        lock (interruptLock)
            pendingInterrupts.Enqueue(e);
        
        // Forward interrupt
        InterruptRequested?.Invoke(this, e);
    }

    public InterruptEventArgs GetNextInterrupt()
    {
        lock (interruptLock)
            return pendingInterrupts.Count > 0 ? pendingInterrupts.Dequeue() : null;
    }

    public int PendingInterruptCount
    {
        get
        {
            lock (interruptLock)
                return pendingInterrupts.Count;
        }
    }

    public async Task StartAllDevicesAsync()
    {
        var tasks = new List<Task>();
        
        foreach (var port in ports)
        {
            if (port.isConnected)
            {
                tasks.Add(port.Device!.StartAsync());
            }
        }
        
        if (tasks.Count > 0)
        {
            await Task.WhenAll(tasks);
        }
    }

    public async Task StopAllDevicesAsync()
    {
        var tasks = new List<Task>();
        
        foreach (var port in ports)
        {
            if (port.isConnected)
            {
                tasks.Add(port.Device!.StopAsync());
            }
        }
        
        if (tasks.Count > 0)
        {
            await Task.WhenAll(tasks);
        }
    }

    public void ResetAllPorts()
    {
        foreach (var port in ports)
        {
            port.Reset();
        }
        
        lock (interruptLock)
        {
            pendingInterrupts.Clear();
        }
    }
    
    public IEnumerable<byte> GetConnectedPorts()
    {
        for (byte i = 0; i < 255; i++)
        {
            if (ports[i].isConnected)
                yield return i;
        }
    }
}

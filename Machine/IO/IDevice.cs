using System;
using System.Threading.Tasks;
using Machine.Control.Interrupts;

namespace Machine.IO;

public interface IDevice
{
    byte PortAddress { get; }
    string DeviceName { get; }
    bool IsReady { get; }
    
    // Device operations
    Task<byte> ReadAsync();
    Task WriteAsync(byte data);
    
    // Interrupt support
    event EventHandler<InterruptEventArgs> InterruptRequested;
    
    // Device lifecycle
    Task StartAsync();
    Task StopAsync();
    void Reset();
}

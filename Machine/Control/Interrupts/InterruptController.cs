using System;
using System.Collections.Generic;

namespace Machine.Control.Interrupts;

public class InterruptController
{
    private readonly Queue<InterruptEventArgs> interruptQueue = new();
    private readonly object queueLock = new();

    private bool inInterruptHandler = false;
    private bool interruptsEnabled = true;

    public int PendingInterruptCount
    {
        get
        {
            lock (queueLock)
                return interruptQueue.Count;
        }
    }
    
    public event EventHandler<InterruptEventArgs> InterruptProcessed;
    
    public void RequestInterrupt(InterruptEventArgs interrupt)
    {
        lock (queueLock)
            interruptQueue.Enqueue(interrupt);
    }
    
    public InterruptEventArgs GetNextInterrupt()
    {
        if (!interruptsEnabled || inInterruptHandler)
            return null;
            
        lock (queueLock)
            return interruptQueue.Count > 0 ? interruptQueue.Dequeue() : null;
    }
    
    public void EnterInterruptHandler()
    {
        inInterruptHandler = true;
    }
    
    public void ExitInterruptHandler()
    {
        inInterruptHandler = false;
    }
    
    public void ClearPendingInterrupts()
    {
        lock (queueLock)
            interruptQueue.Clear();
    }
    
    public void ProcessInterrupt(InterruptEventArgs interrupt)
    {
        InterruptProcessed?.Invoke(this, interrupt);
    }
}

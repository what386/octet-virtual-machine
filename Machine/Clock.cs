using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Machine
{
    public class Clock
    {
        private long cycleCount = 0;
        private bool isRunning = false;
        private readonly int frequency; // Hz
        private CancellationTokenSource? cts;

        // If Frequency >= SpinThresholdHz => use full busy-spin (high precision)
        // If Frequency < SpinThresholdHz  => use hybrid (Task.Delay + spin) to save CPU
        private const int SpinThresholdHz = 200;
        private const int DelaySafetyMarginMs = 1;

        // Events for pipeline coordination
        public event Action OnRisingEdge;
        public event Action OnFallingEdge;
        public event Action<long> OnCycleComplete;

        public Clock(int frequency = 1000) // Default 1 kHz
        {
            if (frequency <= 0) throw new ArgumentException("Frequency must be > 0");
            this.frequency = frequency;
        }

        public long CycleCount => cycleCount;
        public bool IsRunning => isRunning;
        public int Frequency => frequency;

        public void Start(bool setHighPriority = false)
        {
            if (isRunning) return;

            isRunning = true;
            cts = new CancellationTokenSource();
            var token = cts.Token;

            // Choose mode depending on frequency and threshold
            if (frequency >= SpinThresholdHz)
            {
                // High precision busy-wait loop (recommended for 1-5 kHz)
                Task.Run(() =>
                {
                    if (setHighPriority)
                    {
                        try { Thread.CurrentThread.Priority = ThreadPriority.Highest; } catch { /* ignore */ }
                    }

                    double cycleSeconds = 1.0 / frequency;
                    long ticksPerCycle = (long)(cycleSeconds * Stopwatch.Frequency);

                    var sw = Stopwatch.StartNew();
                    long nextTick = sw.ElapsedTicks;

                    while (!token.IsCancellationRequested)
                    {
                        Step();

                        nextTick += ticksPerCycle;

                        // Busy-wait until it's time for the next cycle.
                        // Thread.SpinWait keeps the loop responsive; avoid an empty tight loop.
                        while (sw.ElapsedTicks < nextTick && !token.IsCancellationRequested)
                        {
                            Thread.SpinWait(10);
                        }
                    }
                }, token);
            }
            else
            {
                // Hybrid mode: use Task.Delay for the coarse portion, spin for the remainder.
                Task.Run(async () =>
                {
                    if (setHighPriority)
                    {
                        try { Thread.CurrentThread.Priority = ThreadPriority.Highest; } catch { /* ignore */ }
                    }

                    double cycleSeconds = 1.0 / frequency;
                    long ticksPerCycle = (long)(cycleSeconds * Stopwatch.Frequency);
                    var sw = Stopwatch.StartNew();
                    long nextTick = sw.ElapsedTicks;

                    while (!token.IsCancellationRequested)
                    {
                        Step();

                        nextTick += ticksPerCycle;

                        // Calculate remaining time in ticks
                        while (true)
                        {
                            long remainingTicks = nextTick - sw.ElapsedTicks;
                            if (remainingTicks <= 0) break;

                            double remainingMs = remainingTicks * 1000.0 / Stopwatch.Frequency;
                            if (remainingMs > DelaySafetyMarginMs)
                            {
                                // Sleep coarsely and re-evaluate
                                try
                                {
                                    // subtract margin so we wake up slightly before the deadline
                                    await Task.Delay(TimeSpan.FromMilliseconds(remainingMs - DelaySafetyMarginMs), token);
                                }
                                catch (TaskCanceledException) { break; }
                            }
                            else
                            {
                                // Final small spin for precision
                                while (sw.ElapsedTicks < nextTick && !token.IsCancellationRequested)
                                {
                                    Thread.SpinWait(10);
                                }
                                break;
                            }
                        }
                    }
                }, token);
            }
        }

        public void Stop()
        {
            if (!isRunning) return;
            isRunning = false;
            cts?.Cancel();
            cts?.Dispose();
            cts = null;
        }

        public void Reset()
        {
            Stop();
            cycleCount = 0;
        }

        // Manual single cycle advance (for testing/debugging)
        public void Step()
        {
            // Rising edge - pipeline stages prepare
            OnRisingEdge?.Invoke();

            // Falling edge - pipeline stages commit changes
            OnFallingEdge?.Invoke();

            cycleCount++;
            OnCycleComplete?.Invoke(cycleCount);
        }

        // Run for specified number of cycles (sprint, not real-time)
        public void RunCycles(int cycles)
        {
            for (int i = 0; i < cycles && isRunning; i++)
            {
                Step();
            }
        }
    }
}

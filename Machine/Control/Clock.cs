using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Machine.Control
{
    public class Clock
    {
        private long cycleCount = 0;
        private bool isRunning = false;
        private readonly int frequency; // Hz
        private CancellationTokenSource? cts;

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

        public void Start()
        {
            if (isRunning) return;

            isRunning = true;
            cts = new CancellationTokenSource();
            var token = cts.Token;
            Task.Run(async () =>
            {
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
                            await Task.Delay(TimeSpan.FromMilliseconds(remainingMs - 5), token);
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

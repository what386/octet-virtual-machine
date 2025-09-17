using System.Collections.Generic;

namespace Machine.System.Pipeline;

public class BranchPredictor
{
    // This exists to determine if a more complex
    // branch predictor is worth implementing in
    // hardware. If I end up not implementing it,
    // then it will remain for compatibility.
    private bool simpleMode;

    private readonly int maxEntries;
    private readonly Queue<ushort> accessOrder = new();  // Track access order for LRU

    public BranchPredictor(bool simpleMode, int maxEntries = 32)
    {
        this.simpleMode = simpleMode;

        // each entry is two bytes
        // so a 32 entry table is
        // actually 64 bytes to store
        this.maxEntries = maxEntries;
    }

    // 2 bit saturating counter:
    // 0 - StrongNotTaken
    // 1 - WeakNotTaken
    // 2 - WeakTaken
    // 3 - StrongTaken
    private readonly Dictionary<ushort, byte> branchCounters = new();
    
    public int TotalPredictions { get; private set; }
    public int CorrectPredictions { get; private set; }
    public int CacheEvictions { get; private set; }
    public double AccuracyPercentage => TotalPredictions == 0 ? 0.0 : (double)CorrectPredictions / TotalPredictions * 100.0;

    public bool PredictBranch(ushort pc, ushort targetPC)
    {
        if (simpleMode)
            return targetPC < pc;

        if (branchCounters.ContainsKey(pc))
        {
            // Move to end of access order (mark as recently used)
            MarkAsRecentlyUsed(pc);
            
            // Predict taken if WeakTaken (2) or StrongTaken (3) 
            byte counter = branchCounters[pc];
            return counter >= 2;
        }
        
        // First encounter - backward branches (loops) are usually taken
        bool staticPrediction = targetPC < pc;
        
        // Add to table if there's room, or evict LRU entry
        AddOrEvictEntry(pc, staticPrediction ? (byte)2 : (byte)1);
        
        return staticPrediction;
    }

    public void UpdatePredictor(ushort pc, ushort targetPC, bool wasTaken)
    {
        if (simpleMode)
            return;

        // Update statistics
        TotalPredictions++;
        bool prediction = PredictBranch(pc, targetPC);
        if (prediction == wasTaken)
        {
            CorrectPredictions++;
        }

        // Update 2-bit saturating counter (should exist after PredictBranch call)
        if (branchCounters.ContainsKey(pc))
        {
            byte counter = branchCounters[pc];
            
            if (wasTaken)
                if (counter < 3) counter++;  // Increment, saturate at StrongTaken
            else
                if (counter > 0) counter--;  // Decrement, saturate at StrongNotTaken
            
            branchCounters[pc] = counter;
            MarkAsRecentlyUsed(pc);
        }
    }

    public void ClearHistory()
    {
        branchCounters.Clear();
        accessOrder.Clear();
        TotalPredictions = 0;
        CorrectPredictions = 0;
        CacheEvictions = 0;
    }

    // Get prediction state for specific pc
    public string GetStateForPC(ushort pc)
    {
        if (branchCounters.ContainsKey(pc))
        {
            byte counter = branchCounters[pc];
            string[] states = { "StrongNotTaken", "WeakNotTaken", "WeakTaken", "StrongTaken" };
            return $"{counter} ({states[counter]})";
        }
        return "No history";
    }

    // Private helper methods for LRU cache management
    private void AddOrEvictEntry(ushort pc, byte initialCounter)
    {
        if (branchCounters.Count >= maxEntries)
        {
            // Table is full, evict least recently used entry
            EvictLRUEntry();
        }
        
        // Add new entry
        branchCounters[pc] = initialCounter;
        accessOrder.Enqueue(pc);
    }

    private void EvictLRUEntry()
    {
        if (accessOrder.Count > 0)
        {
            ushort lruPC = accessOrder.Dequeue();
            branchCounters.Remove(lruPC);
            CacheEvictions++;
        }
    }

    private void MarkAsRecentlyUsed(ushort pc)
    {
        // Remove from current position in queue and add to end
        var tempQueue = new Queue<ushort>();
        
        while (accessOrder.Count > 0)
        {
            ushort item = accessOrder.Dequeue();

            if (item != pc)
                tempQueue.Enqueue(item);
        }
        
        // Restore queue without the accessed item
        while (tempQueue.Count > 0)
            accessOrder.Enqueue(tempQueue.Dequeue());
        
        // Add accessed item to end (most recently used)
        accessOrder.Enqueue(pc);
    }
}

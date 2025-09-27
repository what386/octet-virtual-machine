using System;
using System.Collections.Generic;

namespace Machine.Control.Pipeline;

public class BranchPredictor
{
    public const int MaxEntries = 32;
    private readonly Dictionary<ushort, (PredictionState State, int Uses)> predictionTable = new(MaxEntries);

    public bool PredictBranch(ushort currentAddress, ushort targetAddress)
    {
        if (predictionTable.TryGetValue(targetAddress, out var entry))
        {
            predictionTable[targetAddress] = (entry.State, entry.Uses + 1);
            return entry.State >= PredictionState.WeakTaken;
        }

        PredictionState staticResult = targetAddress < currentAddress 
            ? PredictionState.WeakTaken 
            : PredictionState.WeakNotTaken;
            
        AddOrReplace(targetAddress, staticResult);
        return staticResult >= PredictionState.WeakTaken;
    }

    public void Update(ushort targetAddress, bool taken)
    {
        if (!predictionTable.TryGetValue(targetAddress, out var entry))
        {
            AddOrReplace(targetAddress, taken ? PredictionState.WeakTaken : PredictionState.WeakNotTaken);
            return;
        }

        int newUses = entry.Uses + 1;
        PredictionState newState = UpdateSaturatingCounter(entry.State, taken);
        predictionTable[targetAddress] = (newState, newUses);
    }

    private static PredictionState UpdateSaturatingCounter(PredictionState current, bool taken)
    {
        int state = (int)current;
        if (taken)
            return (PredictionState)Math.Min(state + 1, 3);
        else
            return (PredictionState)Math.Max(state - 1, 0);
    }

    private void AddOrReplace(ushort address, PredictionState initialState)
    {
        if (predictionTable.Count >= MaxEntries)
        {
            // Evict the least-used entry
            ushort lruKey = default;
            int minUses = int.MaxValue;
            foreach (var kvp in predictionTable)
            {
                if (kvp.Value.Uses < minUses)
                {
                    minUses = kvp.Value.Uses;
                    lruKey = kvp.Key;
                }
            }
            predictionTable.Remove(lruKey);
        }
        predictionTable[address] = (initialState, 1);
    }
}

public enum PredictionState
{
    StrongNotTaken = 0,  // 00
    WeakNotTaken = 1,    // 01
    WeakTaken = 2,       // 10
    StrongTaken = 3      // 11
}

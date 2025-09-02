using System.Collections.Generic;

namespace Machine.Pipeline
{
    public class BranchPredictor
    {
        // This exists to determine if a more complex
        // branch predictor is worth implementing in
        // hardware. If I end up not implementing it,
        // then it will remain for compatibility.

        private bool simpleMode;

        public BranchPredictor(bool simpleMode) => this.simpleMode = simpleMode;

        // 2 bit saturating counter:
        // 0 - StrongNotTaken
        // 1 - WeakNotTaken
        // 2 - WeakTaken
        // 3 - StrongTaken

        private readonly Dictionary<ushort, byte> branchCounters = new();
        
        public int TotalPredictions { get; private set; }
        public int CorrectPredictions { get; private set; }
        public double AccuracyPercentage => TotalPredictions == 0 ? 0.0 : (double)CorrectPredictions / TotalPredictions * 100.0;

        public bool PredictBranch(ushort pc, ushort targetPC)
        {
            if (simpleMode)
                return targetPC < pc;

            if (branchCounters.ContainsKey(pc))
            {
                // Predict taken if WeakTaken (2) or StrongTaken (3) 
                byte counter = branchCounters[pc];
                return counter >= 2;
            }
            
            // First encounter - backward branches (loops) are usually taken
 
            bool staticPrediction = targetPC < pc;
            // Initialize counter based on static prediction
            branchCounters[pc] = staticPrediction ? (byte)2 : (byte)1;  // WeakTaken or WeakNotTaken
            
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

            // Update 2-bit saturating counter
            if (!branchCounters.ContainsKey(pc))
            {
                // This shouldn't happen if PredictBranch was called first
                branchCounters[pc] = 1;  // WeakNotTaken
            }

            byte counter = branchCounters[pc];
            
            if (wasTaken)
                if (counter < 3) counter++;  // Increment, saturate at StrongTaken
            else
                if (counter > 0) counter--;  // Decrement, saturate at StrongNotTaken
            
            branchCounters[pc] = counter;
        }

        public void ClearHistory()
        {
            branchCounters.Clear();
            TotalPredictions = 0;
            CorrectPredictions = 0;
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

        // Get number of tracked branch instructions
        public int GetTrackedBranchCount() => branchCounters.Count;
    }
}

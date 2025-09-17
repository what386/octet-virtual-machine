namespace Machine.System.Pipeline;

public class PipelineProcessor
{
    private readonly BranchPredictor branchPredictor;
    private readonly HazardManager hazardManager;

    private readonly PipelineStage fetchStage = new();
    private readonly PipelineStage decodeStage = new(); 
    private readonly PipelineStage executeStage = new();
    private readonly PipelineStage writebackStage = new();

    private int cycleCount = 0;

    public PipelineProcessor()
    {
        hazardManager = new HazardManager(fetchStage, decodeStage, executeStage, writebackStage);
    }

    public void Cycle()
    {
        // 1. Detect and resolve hazards before stage updates
        hazardManager.ResolveHazards();

        // 2. Advance pipeline stages (back-to-front)
        ProcessWriteback();
        ProcessExecute(); 
        ProcessDecode();
        ProcessFetch();

        cycleCount++;
    }

    private void ProcessFetch()
    {
        if (!decodeStage.IsStalled)
        {
            decodeStage.CurrentInstruction = fetchStage.CurrentInstruction;
            fetchStage.CurrentInstruction = null;
        }
    }

    private void ProcessDecode()
    {
        if (decodeStage.IsStalled)
        {
            decodeStage.IsStalled = false; // release stall after one cycle
            return;
        }

        executeStage.CurrentInstruction = decodeStage.CurrentInstruction;
        decodeStage.CurrentInstruction = null;
    }

    private void ProcessExecute()
    {
        writebackStage.CurrentInstruction = executeStage.CurrentInstruction;
        executeStage.CurrentInstruction = null;
    }

    private void ProcessWriteback()
    {
        // TODO: Commit results to registers
        writebackStage.CurrentInstruction = null;
    }
}

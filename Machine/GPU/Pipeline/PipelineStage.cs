namespace Machine.Pipeline;

public class PipelineStage
{
    public Instruction? CurrentInstruction { get; set; }
    public bool IsStalled { get; set; }
    public int CyclesRemaining { get; set; }  // For multi-cycle operations
}

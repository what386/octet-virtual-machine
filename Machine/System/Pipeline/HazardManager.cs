namespace Machine.System.Pipeline;

public class HazardManager
{
    private readonly PipelineStage fetch;
    private readonly PipelineStage decode;
    private readonly PipelineStage execute;
    private readonly PipelineStage writeback;

    public HazardManager(
        PipelineStage fetchStage,
        PipelineStage decodeStage,
        PipelineStage executeStage,
        PipelineStage writebackStage)
    {
        fetch = fetchStage;
        decode = decodeStage;
        execute = executeStage;
        writeback = writebackStage;
    }

    public void ResolveHazards()
    {
        HandleDataHazards();
        HandleControlHazards();
        HandleStructuralHazards();
    }

    private void HandleDataHazards()
    {
        if (decode.CurrentInstruction is null || decode.CurrentInstruction.IsNop)
            return;

        if (execute.CurrentInstruction is { WritesToRegister: true } ex &&
            decode.CurrentInstruction.UsesRegister(ex.DestinationRegister))
        {
            StallDecode();
        }
        else if (writeback.CurrentInstruction is { WritesToRegister: true } wb &&
                 decode.CurrentInstruction.UsesRegister(wb.DestinationRegister))
        {
            StallDecode();
        }
    }

    private void HandleControlHazards()
    {
        if (decode.CurrentInstruction is null)
            return;

        // Branch or jump in decode → flush fetch
        if (decode.CurrentInstruction.Type == InstructionType.Branch ||
            decode.CurrentInstruction.Type == InstructionType.Jump)
        {
            FlushFetch();
        }
    }

    private void StallDecode()
    {
        decode.IsStalled = true;
    }

    private void FlushFetch()
    {
        if (fetch.CurrentInstruction is not null)
        {
            fetch.CurrentInstruction = new Instruction { IsNop = true, Type = InstructionType.Nop };
        }
    }
}

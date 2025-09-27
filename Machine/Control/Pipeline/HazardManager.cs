using Machine.Control.InstructionDecoder;

namespace Machine.Control.Pipeline;

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
    }

    private void HandleDataHazards()
    {
        if (decode.CurrentInstruction is null || decode.CurrentInstruction.instructionType == InstructionType.Nop)
            return;

        if (execute.CurrentInstruction is { writesToRegister: true } ex &&
            decode.CurrentInstruction.UsesRegister(ex.destinationRegister))
        {
            StallDecode();
        }
        else if (writeback.CurrentInstruction is { writesToRegister: true } wb &&
                 decode.CurrentInstruction.UsesRegister(wb.destinationRegister))
        {
            StallDecode();
        }
    }

    private void HandleControlHazards()
    {
        if (decode.CurrentInstruction is null)
            return;

        // Branch or jump in decode → flush fetch
        if (decode.CurrentInstruction.instructionType == InstructionType.Branch ||
            decode.CurrentInstruction.instructionType == InstructionType.Jump)
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
            fetch.CurrentInstruction = DecodingManager.Decode(0b0000000000000000);
        }
    }
}

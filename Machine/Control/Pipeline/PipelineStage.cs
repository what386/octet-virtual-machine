using Machine.Control.InstructionDecoder;

namespace Machine.Control.Pipeline;

public class PipelineStage
{
    public DecodedInstruction CurrentInstruction { get; set; }
    public bool IsStalled { get; set; }
}

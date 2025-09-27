namespace Machine.Control;

public class ProgramCounter
{
    private ushort pc = 0;
    private bool shouldBranch = false;
    private ushort branchTarget = 0;
    
    public ushort Current => pc;
    public bool HasPendingBranch => shouldBranch;
    public ushort BranchTarget => branchTarget;
    
    public void Increment()
    {
        pc++;
    }
    
    public void SetPC(ushort newPC)
    {
        pc = newPC;
        shouldBranch = false;
        branchTarget = 0;
    }
    
    public void QueueBranch(ushort target)
    {
        shouldBranch = true;
        branchTarget = target;
    }
    
    public void ApplyBranch()
    {
        if (shouldBranch)
        {
            pc = branchTarget;
            shouldBranch = false;
            branchTarget = 0;
        }
    }
    
    public void CancelBranch()
    {
        shouldBranch = false;
        branchTarget = 0;
    }
    
    public void Reset()
    {
        pc = 0;
        shouldBranch = false;
        branchTarget = 0;
    }
}

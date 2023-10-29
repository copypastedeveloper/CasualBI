namespace Casual.BI.LLM.Tools;

public abstract class UITool : ITool
{
    public abstract string Type { get; }
    public ExecutionEnvironment ExecutionEnvironment => ExecutionEnvironment.Client;
    public abstract string Description { get; }
}
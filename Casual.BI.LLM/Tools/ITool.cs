namespace Casual.BI.LLM.Tools;

public interface ITool
{
    string Type { get; }
    ExecutionEnvironment ExecutionEnvironment { get; }
    string Description { get; }
    
}
namespace Casual.BI.API.LLM.Tools;

public abstract class BrowserTool : ITool
{
    public abstract string Type { get; }
    public ExecutionEnvironment ExecutionEnvironment => ExecutionEnvironment.Browser;
    public abstract string Description { get; }
}
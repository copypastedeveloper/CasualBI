namespace Casual.BI.LLM.Tools;

public abstract class ServerTool : ITool
{
    public abstract string Type { get; }
    public ExecutionEnvironment ExecutionEnvironment => ExecutionEnvironment.Server;
    public abstract string Description { get; }

    public abstract Task<string> Execute(string currentStepTask, List<Message> history);
}
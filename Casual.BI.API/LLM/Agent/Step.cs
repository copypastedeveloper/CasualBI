using Casual.BI.API.LLM.Tools;

namespace Casual.BI.API.LLM.Agent;

public class Step
{
    public Step(string actionType,ITool tool, string task)
    {
        ActionType = actionType;
        Tool = tool;
        Task = task;
    }

    public string ActionType { get; init; }
    public ITool Tool { get; set; }
    public string Task { get; init; }
    public string? Result { get; set; }
}
using Casual.BI.LLM.Tools;

namespace Casual.BI.LLM.Prompts;

public abstract class AgentPrompt : Prompt<AgentPrompt>, IAgentPrompt
{
    const string StepsMessage = "Do not attempt to take action, answer the question or carry out steps, only list a succinct set of steps.  " +
                                 "Each step should be able to be accomplished using one of the  tools from above.  " +
                                 "Your response a json array of strings, where each element looks like " +
                                 "{{\"actionType\":\"[Tool To Use]\", \"task\": \"Task to complete\"}}.  Wrap it in a code block. \n " +
                                 "At the end, if any steps can be combined to achieve the same result, create a second " +
                                 "array with the combined steps with the same format.\n";

    const string ToolsMessage = "You have the following tools available to you: \n{0} \n";

    public List<ITool> AvailableTools { get; }

    protected AgentPrompt(List<ITool> availableTools)
    {
        AvailableTools = availableTools;
    }
    
    protected abstract List<Message> Instructions { get; }
    protected abstract string Persona { get; }
    
    public sealed override string ToString()
    {
        return string.Join(Environment.NewLine, AsChatMessages().Select(x => (string) x));
    }
    
    public sealed override List<Message> AsChatMessages()
    {
        var tools = AvailableTools.Aggregate(string.Empty, (current, tool) => current + $"- [{tool.Type}] - {tool.Description}");
        var toolsPart = string.Format(ToolsMessage, tools);

        var systemMessage = new Message(ChatRequest.Role.System, $"{Persona} \n {toolsPart}");
        var messages = new List<Message> {systemMessage};
        messages.AddRange(Instructions);
        messages.Add(new (ChatRequest.Role.User,StepsMessage));
        return messages;
    }
}
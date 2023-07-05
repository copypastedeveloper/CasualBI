using Casual.BI.API.LLM.Tools;
using Role = Casual.BI.API.LLM.ChatRequest.Role;

namespace Casual.BI.API.LLM.Prompts;

public class DataAnalysisAgentPrompt : AgentPrompt
{
    readonly string _schema;
    readonly string _question;

    protected override string Persona => "You are an AI that is responsible for answering questions about data.  " +
                                         "You are provided a database schema and a question, and using the tools at your " +
                                         "disposal you attempt to answer the question in a way that is easy to understand.";
    
    static readonly string PromptTemplate = "Here is a schema: \n {0} \n Based on the schema, what steps would " +
                                            "you take to answer the question: {1}";
    
    string UserMessage => string.Format(PromptTemplate, _schema, _question);
    
    public DataAnalysisAgentPrompt(string schema, string question) : 
        base(new () {new Analysis(),new Graph(),new GridView(),new QueryEngine()})
    {
        _schema = schema;
        _question = question;
    }

    protected override List<Message> Instructions => new(new List<Message>{new(Role.User, UserMessage)});
}

public abstract class AgentPrompt : Prompt<AgentPrompt>, IAgentPrompt
{
    static string StepsMessage = "Do not attempt to take action, answer the question or carry out steps, only list a succinct set of steps.  " +
                                 "Each step should be able to be accomplished using one of the  tools from above.  " +
                                 "Your response a json array of strings, where each element looks like " +
                                 "{{\"actionType\":\"[Tool To Use]\", \"task\": \"Task to complete\"}}.  Wrap it in a code block. \n " +
                                 "At the end, if any steps can be combined to achieve the same result, create a second " +
                                 "array with the combined steps with the same format.\n";

    static string ToolsMessage = "You have the following tools available to you: \n{0} \n";

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

        var systemMessage = new Message(Role.System, $"{Persona} \n {toolsPart}");
        var messages = new List<Message> {systemMessage};
        messages.AddRange(Instructions);
        messages.Add(new (Role.User,StepsMessage));
        return messages;
    }
}
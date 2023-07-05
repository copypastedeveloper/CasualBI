using Casual.BI.API.LLM.Agent;
using Casual.BI.API.LLM.Parsers;
using Casual.BI.API.LLM.Prompts;

namespace Casual.BI.API.LLM.Tools;

public sealed class QueryEngine : ServerTool
{
    public override string Type => "Query Engine";
    public override string Description => "A postgres database to query the schema that you were provided.";
    
    public override async Task<string> Execute(string currentStepTask, List<Message> history)
    {
        var prompt = new QueryBuildingPrompt(currentStepTask);

        var chat = ChatRequest.Create().FromMessages(history.Concat(prompt.AsChatMessages()).ToList());

        var response = await chat.Send(r => r.AsCodeBlocks().FirstOrDefault(r));

        return response;
    }
}
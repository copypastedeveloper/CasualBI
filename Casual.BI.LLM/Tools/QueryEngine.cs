using Casual.BI.LLM.Parsers;
using Casual.BI.LLM.Prompts;

namespace Casual.BI.LLM.Tools;

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
using Role = Casual.BI.API.LLM.ChatRequest.Role;

namespace Casual.BI.API.LLM.Prompts;

public class DataAnalysisAgentPrompt : Prompt<DataAnalysisAgentPrompt>, IAgentPrompt
{
    readonly string _schema;
    readonly string _question;

    static string SystemPrompt =
@"You are an AI that is responsible for answering questions about data.  You are provided a database schema and a question, and using the tools at your disposal you attempt to answer the question in a way that is easy to understand.  You have the following tools available to you:    
- [Query Engine] - A postgres database to query the schema that you were provided. Tables can be joined,grouped, sorted, any valid postgresql operation.
- [Grid View] - A view that renders the query results into a grid
- [Graph] - A graphing library capable of graphing the results using Vega-Lite
- [Analysis] - Create a text based, freeform paragraph with analysis 
The Grid View or Graph tools must be used before any analysis can be done.";
    
    static string PromptTemplate =
@"Here is a schema: 
{0}
Based on the schema, what steps would you take to answer the question: {1}

Do not attempt to answer the question or carry out the steps, only list a succinct set of steps.  Each step should be able to be accomplished using one of the  tools from above.  your response a json array of strings, where each element looks like {{""actionType"":""[Tool To Use]"", ""task"": ""Task to complete""}}.  Wrap it in a code block.

At the end, if any steps can be combined to achieve the same result, create a second array with the combined steps with the same format.
";
    string UserMessage => string.Format(PromptTemplate, _schema, _question);
    
    public DataAnalysisAgentPrompt(string schema, string question)
    {
        _schema = schema;
        _question = question;
    }

    public override string ToString()
    {
        return SystemPrompt + Environment.NewLine + UserMessage;
    }

    public override List<Message> AsChatMessages()
    {
        return new(new List<Message>{new (Role.System, SystemPrompt), new(Role.User, UserMessage)});
    }
}
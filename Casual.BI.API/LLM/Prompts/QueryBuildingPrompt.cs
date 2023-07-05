namespace Casual.BI.API.LLM.Prompts;
using Role = ChatRequest.Role;

public class QueryBuildingPrompt : Prompt<QueryBuildingPrompt>
{
    readonly string _task;
    const string SystemPrompt = "You are an expert in postgreSQL, and are assisting in writing queries based on a schema and data examples.  You respond in SQL wrapped in a code block.  Resulting field names should be english and can contain spaces.";
    const string PromptTemplate = "As a senior analyst your only output should be SQL code wrapped in a code block. Do not include any other text. Given the above schema, write a detailed and correct Postgres sql query that uses only that schema to answer the analytical question:{0}";

    public QueryBuildingPrompt(string task)
    {
        _task = task;
    }
    
    string UserMessage => string.Format(PromptTemplate, _task);

    
    public override string ToString()
    {
        return SystemPrompt + Environment.NewLine + UserMessage;
    }

    public override List<Message> AsChatMessages()
    {
        return new(new List<Message>{new (Role.System, SystemPrompt), new(Role.User, UserMessage)});
    }
}
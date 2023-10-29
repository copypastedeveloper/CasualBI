using Casual.BI.LLM.Tools;
using Role = Casual.BI.LLM.ChatRequest.Role;

namespace Casual.BI.LLM.Prompts;

public class DataAnalysisAgentPrompt : AgentPrompt
{
    readonly string _schema;
    readonly string _question;

    protected override string Persona => "You are an AI that is responsible for answering questions about data.  " +
                                         "You are provided a database schema and a question, and using the tools at your " +
                                         "disposal you attempt to answer the question in a way that is easy to understand.";
    
    const string PromptTemplate = "Here is a schema: \n {0} \n Based on the schema, what steps would " +
                                            "you take to answer the question: {1}";
    
    string UserMessage => string.Format(PromptTemplate, _schema, _question);
    
    public DataAnalysisAgentPrompt(string schema, string question, List<ITool> tools) : 
        base(tools)
    {
        _schema = schema;
        _question = question;
    }

    protected override List<Message> Instructions => new(new List<Message>{new(Role.User, UserMessage)});
}
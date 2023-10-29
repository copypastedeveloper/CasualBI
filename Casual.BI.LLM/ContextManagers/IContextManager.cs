namespace Casual.BI.LLM.ContextManagers;

public interface IContextManager : IEnumerable<List<Message>>
{
    void AddMessage(Message message);
    string Response { get; }
    List<Message> Messages { get; }
}
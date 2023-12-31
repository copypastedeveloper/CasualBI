using OpenAI.ObjectModels.RequestModels;

namespace Casual.BI.LLM;

public record Message(string Role, string Content)
{
    public static implicit operator Message(ChatMessage m) => new(m.Role, m.Content);
    public static implicit operator ChatMessage(Message m) => new(m.Role, m.Content);
    public static implicit operator string(Message m) => $"{m.Role}:{m.Content}";
}
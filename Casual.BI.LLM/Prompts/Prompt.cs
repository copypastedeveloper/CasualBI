namespace Casual.BI.LLM.Prompts;

public abstract class Prompt
{
    public abstract List<Message> AsChatMessages();
}

public abstract class Prompt<T> : Prompt
{
    public static implicit operator string(Prompt<T> p) => p.ToString() ?? string.Empty;
    public static implicit operator List<Message>(Prompt<T> p) => p.AsChatMessages();

    public override List<Message> AsChatMessages()
    {
        return new List<Message>
        {
            new(ChatRequest.Role.System, this)
        };
    }
}
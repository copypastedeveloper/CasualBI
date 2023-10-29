using OpenAI.ObjectModels.RequestModels;

namespace Casual.BI.LLM;

public record Message(string Role, string Content, string Purpose = "")
{
    public static implicit operator Message(ChatMessage m) => new(m.Role, m.Content,string.Empty);
    public static implicit operator ChatMessage(Message m) => new(m.Role, m.Content);
    public static implicit operator string(Message m) => $"{m.Role}:{m.Content}";

    /// <summary>
    /// Estimates token usage. will not be exact as model is not known.
    /// </summary>
    public int TokenCount { get; } = OpenAI.Tokenizer.GPT3.TokenizerGpt3.TokenCount(Content) + 4;
}
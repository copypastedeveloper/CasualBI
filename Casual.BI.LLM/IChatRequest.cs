using OpenAI.ObjectModels.ResponseModels;

namespace Casual.BI.LLM;

public interface IChatRequest
{
    List<Message> Messages { get; }
    IChatRequest WithPersona(string persona);
    IChatRequest WithMessage(string role, string message, string purpose = "context");
    IChatRequest FromMessages(List<Message> chatMessages);
    Task<string> Send(Func<string,string> extractor);
    Task<string> Send();
    IAsyncEnumerable<ChatCompletionCreateResponse> SendAsStream();
    Task<T> Send<T>(Func<string, T> parser);
}
using OpenAI.Interfaces;
using OpenAI.Managers;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels.ResponseModels;
using Config = Casual.BI.API.Startup.Startup;
namespace Casual.BI.API.LLM;

public class ChatRequest : IChatRequest
{
    readonly string _model;
    readonly float _temperature;
    readonly IOpenAIService _openAiService;
    public List<Message> Messages { get; set; } = new();

    public static class Role
    {
        public const string Assistant = "assistant";
        public const string User = "user";
        public const string System = "system";

        public static bool Validate(string role)
        {
            return role is Assistant or User or System;
        }
    }
    
    ChatRequest(string model, float temperature)
    {
        if (Startup.Startup.Container == null) throw new Exception("Container not initialized");
        
        _openAiService = Startup.Startup.Container.GetInstance<IOpenAIService>();
        _model = model;
        _temperature = temperature;
    }
    
    public static IChatRequest Create(string model = "gpt-3.5-turbo", float temperature = 0.7f)
    {
        return new ChatRequest(model,temperature);
    }

    IChatRequest IChatRequest.WithPersona(string persona)
    {
        Messages.Add(new(Role.System, persona));
        return this;
    }

    IChatRequest IChatRequest.FromMessages(List<Message> chatMessages)
    {
        Messages = new(chatMessages);
        return this;
    }

    IChatRequest IChatRequest.WithMessage(string role, string message)
    {
        if (!Role.Validate(role)) throw new ArgumentException($"{nameof(role)} must be valid", nameof(role));

        Messages.Add(new(role, message));
        return this;
    }

    async Task<string> Send()
    {
        var completionResult = await _openAiService.ChatCompletion.CreateCompletion(new()
        {
            Messages = Messages.Select(m => (ChatMessage) m).ToList(),
            Model = _model,
            Temperature = _temperature
        });

        var response = completionResult.Choices.First().Message.Content;
        return response;
    }

    async Task<string> IChatRequest.Send()
    {
        var response = await Send();
        Messages.Add(new (Role.Assistant,response));
        return response;
    }
    
    async Task<T> IChatRequest.Send<T>(Func<string,T> parser)
    {
        var response = await Send();
        Messages.Add(new (Role.Assistant,response));
        return parser(response);
    }

    async Task<string> IChatRequest.Send(Func<string,string> extractor)
    {
        var response = await Send();
        var extractedResponse = extractor.Invoke(response);
        Messages.Add(new (Role.Assistant,extractedResponse));
        return extractedResponse;
    }
    
    IAsyncEnumerable<ChatCompletionCreateResponse> IChatRequest.SendAsStream()
    {
        return _openAiService.ChatCompletion.CreateCompletionAsStream(new()
        {
            Messages = Messages.Select(m => (ChatMessage)m).ToList(),
            Model = _model,
            Temperature = _temperature
        });
    }
}
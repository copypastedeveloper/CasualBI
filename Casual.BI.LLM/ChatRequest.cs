using Casual.BI.LLM.ContextManagers;
using OpenAI;
using OpenAI.Interfaces;
using OpenAI.Managers;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels.ResponseModels;

namespace Casual.BI.LLM;

public class ChatRequest : IChatRequest
{
    readonly float _temperature;
    readonly ContextManager _contextManager;
    readonly IOpenAIService _openAiService;

    public List<Message> Messages => _contextManager.Messages;
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
    
    ChatRequest(float temperature, ContextManager contextManager)
    {
        if (LLMConfiguration.Settings == null) throw new Exception("LLM not initialized");

        _openAiService = new OpenAIService(new OpenAiOptions {ApiKey = LLMConfiguration.Settings.Key});
        
        _temperature = temperature;
        _contextManager = contextManager;
    }
    
    public static IChatRequest Create(string model = "gpt-3.5-turbo", float temperature = 0.7f)
    {
        return Create(new DefaultContextManagementStrategy(model),temperature);
    }
    
    public static IChatRequest Create(ContextManager contextManager,float temperature = 0.7f)
    {
        return new ChatRequest(temperature, contextManager);
    }

    IChatRequest IChatRequest.WithPersona(string persona)
    {
        _contextManager.AddMessage(new(Role.System, persona,"persona"));
        return this;
    }

    IChatRequest IChatRequest.WithPrompt(string prompt)
    {
        _contextManager.AddMessage(new(Role.User, prompt,"prompt"));
        return this;
    }

    IChatRequest IChatRequest.FromMessages(List<Message> chatMessages)
    {
        chatMessages.ForEach(_contextManager.AddMessage);
        return this;
    }

    IChatRequest IChatRequest.WithMessage(string role, string message, string purpose = "context")
    {
        if (!Role.Validate(role)) throw new ArgumentException($"{nameof(role)} must be valid", nameof(role));

        _contextManager.AddMessage(new(role, message, purpose));
        return this;
    }

    async Task<string> Send()
    {
        foreach (var messageSet in _contextManager)
        {
            var completionResult = await _openAiService.ChatCompletion.CreateCompletion(new()
            {
                Messages = messageSet.Select(m => (ChatMessage) m).ToList(),
                Model = _contextManager.Model,
                Temperature = _temperature,
                MaxTokens = 50
            });
            
            var response = completionResult.Choices.First().Message.Content;
            
            _contextManager.AddMessage(new Message(Role.Assistant,response,"response"));
        }
        
        return _contextManager.Response;
    }

    async Task<string> IChatRequest.Send()
    {
        var response = await Send();
        return response;
    }
    
    async Task<T> IChatRequest.Send<T>(Func<string,T> parser)
    {
        var response = await Send();
        return parser(response);
    }

    async Task<string> IChatRequest.Send(Func<string,string> extractor)
    {
        var response = await Send();
        var extractedResponse = extractor.Invoke(response);
        return extractedResponse;
    }
    
    IAsyncEnumerable<ChatCompletionCreateResponse> IChatRequest.SendAsStream()
    {
        //not sure how to do this yet
        return _openAiService.ChatCompletion.CreateCompletionAsStream(new()
        {
            Messages = _contextManager.SelectMany(m => m.Select(x => (ChatMessage)x)).ToList(),
            Model = _contextManager.Model,
            Temperature = _temperature
        });
    }
}
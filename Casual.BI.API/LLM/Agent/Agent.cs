using System.Text.Json;
using Casual.BI.API.LLM.Parsers;
using Casual.BI.API.LLM.Prompts;
using Casual.BI.API.LLM.Tools;

namespace Casual.BI.API.LLM.Agent;

public class Agent
{
    public List<Step> CompletedSteps { get; } = new ();
    public Queue<Step> RemainingSteps;
    public Step? CurrentStep => RemainingSteps.Count > 0 ? RemainingSteps.Peek() : null;
    public List<Message> History { get; init; } = new();

    public static async Task<Agent> FromPrompt(AgentPrompt prompt)
    {
        
        //gpt-4 provides significantly better steps than 3.5-turbo :(
        var chatRequest = ChatRequest.Create("gpt-4").FromMessages(prompt.AsChatMessages());

        //LastOrDefault here because we are only interested in the final block, not the ones before that were not condensed.
        var initialSteps = await chatRequest.Send();

        chatRequest.WithMessage(ChatRequest.Role.Assistant,initialSteps)
            .WithMessage(ChatRequest.Role.User, "Look at the steps one final time, determine if any can be condensed.  " +
            "If it looks correct as is return the existing json in a code block.  If any can be condensed, do so.");
        var stepsJson = await chatRequest.Send(r => r.AsCodeBlocks().LastOrDefault(r));

        IEnumerable<Step> steps = JsonSerializer.Deserialize<List<Step>>(stepsJson,new JsonSerializerOptions {PropertyNameCaseInsensitive = true }) 
                                  ?? new List<Step>();
        
        //i hate this
        foreach (var step in steps)
        {
            step.Tool = prompt.AvailableTools.Find(t => t.Type == step.ActionType);
        }
        
        var agent = new Agent(steps){ History = chatRequest.Messages };
        
        return agent;
    }

    Agent(IEnumerable<Step> steps)
    {
        RemainingSteps = new Queue<Step>(steps);
    }

    public async Task Execute(Func<string,Task> outputWriter)
    {
        if (CurrentStep is null) throw new Exception("There are no remaining steps");
        
        if (CurrentStep.Tool is ServerTool serverTool)
        {
            CurrentStep.Result = await serverTool.Execute(CurrentStep.Task, History);
            History.Add(new (ChatRequest.Role.Assistant,CurrentStep.Result));
        }
        
        await outputWriter(JsonSerializer.Serialize(CurrentStep));

        Advance();
    }
    
    public void Advance()
    {
        if (RemainingSteps.TryDequeue(out var step))
        {
            CompletedSteps.Add(step);
        }
    }
}
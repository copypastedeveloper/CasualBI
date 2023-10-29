using System.Collections;
using Casual.BI.LLM.Models;

namespace Casual.BI.LLM.ContextManagers;

public abstract class ContextManager : IContextManager
{
    protected ContextManager(Model model)
    {
        Model = model;
    }
    
    public List<Message> Messages { get; protected set; } = new();
    public string Response => Messages.First(x => x.Purpose == "response").Content;

    public abstract IEnumerator<List<Message>> GetEnumerator();
    public Model Model { get; private set; }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    
    public virtual void AddMessage(Message message) => Messages.Add(message);

    protected (List<Message> systemMessages, List<Message> promptMessage) ExtractSystemAndPrompt()
    {
        Func<Message,bool> systemFilter = x => x.Role == ChatRequest.Role.System;
        Func<Message, bool> promptFilter = x => x.Purpose == "prompt";

        //split out system, prompt and context
        var system = Messages.Where(systemFilter).ToList();
        var prompt = Messages.Where(promptFilter).ToList();
        Messages = Messages.Except(system.Concat(prompt)).ToList();

        return (system, prompt);
    }
}
using Casual.BI.LLM.Models;

namespace Casual.BI.LLM.ContextManagers;

public class SummaryContextManagementStrategy : ContextManager
{
    readonly int _responseLengthInTokens;

    public SummaryContextManagementStrategy(Model model, int responseLengthInTokens) : base(model)
    {
        _responseLengthInTokens = responseLengthInTokens;
    }
    
    public override IEnumerator<List<Message>> GetEnumerator()
    {

        
        if (Messages.Sum(x => x.TokenCount) > Model.ContextTokenLimit - _responseLengthInTokens)
        {
            var (system, prompt) = ExtractSystemAndPrompt();

            var summaryPrompt = $"Summarize the above as tersely as possible. Keep all details you think are " +
                                $"relevant to answer the following question: \n {prompt.First().Content}";
            
            var summary = ChatRequest.Create(new LoopingContextManagementStrategy(Model,"Add to the existing summary any new important information.", 100))
                .WithPersona("You are an AI that assists with summarizing content.")
                .FromMessages(Messages)
                .WithMessage("user",summaryPrompt,"prompt")
                .Send().Result;
            
            Console.WriteLine($"Summary intermediate response: {summary}");

            
            Messages.Clear();
            Messages.Add(new Message("user",summary));
            Messages.InsertRange(0,system);
            Messages.AddRange(prompt);
            
            yield return Messages;
        }
        else
        {
            yield return Messages;
        }
    }
}
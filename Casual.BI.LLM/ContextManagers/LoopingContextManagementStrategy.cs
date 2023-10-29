using Casual.BI.LLM.Models;

namespace Casual.BI.LLM.ContextManagers;

/// <summary>
/// Will loop over portions of the messages, always supplying messages marked as system messages and prompt messages
/// </summary>
public class LoopingContextManagementStrategy : ContextManager
{
    readonly string _loopIntro;
    readonly int _responseLengthInTokens;

    /// <summary>
    /// Create an instance of a looping context manager.  This will group messages into appropriately context sized
    /// sets.
    /// </summary>
    /// <param name="model">Model that should be used.</param>
    /// <param name="loopIntro">a message that will accompany each loop for instruction on how to handle the looping.</param>
    /// <param name="responseLengthInTokens">How many tokens should be reserved for the response</param>
    public LoopingContextManagementStrategy(Model model, string loopIntro, int responseLengthInTokens) : base(model)
    {
        _loopIntro = loopIntro;
        _responseLengthInTokens = responseLengthInTokens;
    }
    
    /// <summary>
    /// Allows for iteration over sets of messages such that each set and a response can fit in the model's context
    /// </summary>
    /// <returns></returns>
    public override IEnumerator<List<Message>> GetEnumerator()
    {
        var textSplitter = new TextSplitter();
        Func<Message,bool> responseFilter = x => x.Purpose == "response";

        var (system, prompt) = ExtractSystemAndPrompt();
        
        while (Messages.Any(m => m.Purpose != "response"))
        {
            var eachLoopMessages = system.ToList();
            var previousResponse = Messages.FirstOrDefault(responseFilter);
            
            //if its not the first time through we have to add the instructions for what to do with the previous response
            if (previousResponse is not null)
            {
                eachLoopMessages.Add(new Message(ChatRequest.Role.User, _loopIntro));
                Messages.Remove(previousResponse);
            }
            
            //some tokens need to be held out because of math
            var reservedTokens = eachLoopMessages.Sum(x => x.TokenCount) 
                                 + prompt.Sum(x => x.TokenCount) 
                                 + (previousResponse?.TokenCount ?? 0)
                                 + _responseLengthInTokens;
            
            var availableTokensPerLoop = Model.ContextTokenLimit - reservedTokens;

            //how to chunk when necessary
            textSplitter.TargetTokens = availableTokensPerLoop;
            textSplitter.TokenOverlap = 10;
            
            var messageLoop = new List<Message>();

            while (messageLoop.Sum(x => x.TokenCount) < availableTokensPerLoop &&  Messages.Any())
            {
                var message = Messages.First();
                var tokenCount = message.TokenCount;

                var splitContextForSingleMessage = textSplitter.Split(message.Content, Environment.NewLine, Model).ToList();
                if (splitContextForSingleMessage.Count > 1)
                {
                    var splitMessages = splitContextForSingleMessage.Select(x => new Message(message.Role, x, message.Purpose));
                    Messages.InsertRange(Messages.IndexOf(message),splitMessages);
                    Messages.Remove(message);

                    //start the loop again with the split messages
                    continue;
                }
                
                if (messageLoop.Sum(x => x.TokenCount) + tokenCount > availableTokensPerLoop)
                    break;

                messageLoop.Add(message);
                Messages.Remove(message);
            }
            
            messageLoop.InsertRange(0,eachLoopMessages);
            messageLoop.AddRange(prompt);
            
            if (previousResponse is not null)
            {
                Console.WriteLine($"Looping intermediate response: {previousResponse.Content}");
                messageLoop.Add(new Message("user",$"This was your previous response: \n {previousResponse.Content}"));
            }
            
            yield return messageLoop;
        }
    }
}
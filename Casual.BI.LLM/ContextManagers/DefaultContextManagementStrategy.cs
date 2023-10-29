using Casual.BI.LLM.Models;

namespace Casual.BI.LLM.ContextManagers;

/// <summary>
/// will not  prevent any kind of overflow, this could cause exceptions
/// </summary>
public class DefaultContextManagementStrategy : ContextManager
{
    public DefaultContextManagementStrategy(Model model) : base(model)
    {}
    
    public override IEnumerator<List<Message>> GetEnumerator()
    {
        yield return Messages;
    }
}
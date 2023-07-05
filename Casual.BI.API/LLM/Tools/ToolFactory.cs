namespace Casual.BI.API.LLM.Tools;

public class ToolFactory
{
    readonly Dictionary<string, ITool> _toolTypes;

    public ToolFactory(IEnumerable<ITool> allTools)
    {
        _toolTypes = allTools.ToDictionary(x => x.Type, x => x);
    }

    public ITool Get(string tool)
    {
        if (_toolTypes.TryGetValue(tool, out var type)) return type;

        throw new ArgumentException($"{tool} is not an available tool");
    }
}
using Casual.BI.LLM.Tools;

namespace Casual.BI.API.Tools;

public sealed class Graph : UITool
{
    public override string Type => "Graph";
    public override string Description => "A graphing component capable of displaying a Vega-Lite interactive graph";
}
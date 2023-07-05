namespace Casual.BI.API.LLM.Tools;

public sealed class Graph : BrowserTool
{
    public override string Type => "Graph";
    public override string Description => "A graphing component capable of displaying a Vega-Lite interactive graph";
}
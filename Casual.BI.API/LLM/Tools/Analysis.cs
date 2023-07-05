namespace Casual.BI.API.LLM.Tools;

public sealed class Analysis: BrowserTool
{
    public override string Type => "Analysis";
    public override string Description => "A markdown compatible view to display freeform data analysis";
}
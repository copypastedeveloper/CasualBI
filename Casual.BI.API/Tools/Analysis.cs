using Casual.BI.LLM.Tools;

namespace Casual.BI.API.Tools;

public sealed class Analysis: UITool
{
    public override string Type => "Analysis";
    public override string Description => "A markdown compatible view to display freeform data analysis";
}
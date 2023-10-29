using Casual.BI.LLM.Agent;
using Casual.BI.LLM.Tools;
using DuckDB.NET.Data;

namespace Casual.BI.API.Tools;

public sealed class GridView : UITool
{
    public override string Type => "Grid View";
    public override string Description => "A table capable of displaying tabular data";

    public async Task<string> Execute(string currentStepTask, List<Step> history)
    {
        // var query =  history.Last(x => x.Tool is QueryEngine).Result;
        // await using var db = new DuckDBConnection("DataSource=:memory:");
        // db.Open();
        //
        // command.CommandText = "INSTALL 'httpfs'; Load 'httpfs';";
        //
        // db.
        return string.Empty;
    }
        
}
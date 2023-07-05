namespace Casual.BI.API.Commands;

public class WriteQueryCommand
{
    public string? TableContext { get; set; }
    public string? Question { get; set; }
}

public class InitiateAgentCommand
{
    public string? TableContext { get; set; }
    public string? Question { get; set; }
    public IEnumerable<string> Tools { get; set; } = new List<string>();
}
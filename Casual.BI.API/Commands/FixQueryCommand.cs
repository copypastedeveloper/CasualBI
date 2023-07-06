using Casual.BI.LLM;

namespace Casual.BI.API.Commands;

public class FixQueryCommand
{
    public string? Error { get; set; }
    public List<Message> ChatHistory { get; set; } = new();
}
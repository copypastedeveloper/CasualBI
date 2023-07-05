using System.Text.RegularExpressions;

namespace Casual.BI.API.LLM.Parsers;

public static class CodeParser
{
    static readonly Regex Pattern = new("```.*?\n(?<code>(.|\n)*?)```", RegexOptions.Multiline);

    /// <summary>
    /// Parses codeblocks out of a llm response
    /// </summary>
    /// <param name="response"></param>
    /// <returns>an enumerable of all code blocks in the response</returns>
    public static IEnumerable<string> AsCodeBlocks(this string response)
    {
        return Pattern.Matches(response).Select(m => m.Groups["code"].Value).Where(v =>!string.IsNullOrEmpty(v));
    }
}
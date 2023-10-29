using System.Collections.Concurrent;
using Microsoft.DeepDev;

namespace Casual.BI.LLM.Models;

public class TextSplitter
{
    static readonly ConcurrentDictionary<string, ITokenizer> TokenizerCache = new();

    public int TokenOverlap { get; set; } = 200;
    public int TargetTokens { get; set; } = 1500;

    public IEnumerable<string> Split(string input, string separator, string model)
    {
        var splits = input.Split(separator);
        return MergeStrings(splits, separator,model);
    }
    
    static string Rejoin(IEnumerable<string> docs, string separator)
    {
        return string.Join(separator, docs).Trim();
    }

    IEnumerable<string> MergeStrings(IEnumerable<string> splits, string separator, string model)
    {
        var docs = new List<string>();
        var currentDoc = new List<string>();
        var total = 0;

        var splitsQueue = new Queue<string>(splits);
        var overlapQueue = new Queue<string>();

        while (splitsQueue.Any())
        {
            if (!TokenizerCache.ContainsKey(model))
            {
                TokenizerCache.TryAdd(model,TokenizerBuilder.CreateByModelNameAsync(model).Result);
            }

            var tokenizer = TokenizerCache[model];
            var tokens = tokenizer.Encode(splitsQueue.Peek(), new HashSet<string>()).Count;

            if (tokens + total <= TargetTokens - TokenOverlap)
            {
                currentDoc.Add(splitsQueue.Dequeue());
                total += tokens;
            }
            else if (tokens + total <= TargetTokens)
            {
                var split = splitsQueue.Dequeue();
                currentDoc.Add(split);
                overlapQueue.Enqueue(split);
                total += tokens;
            }
            else
            {
                docs.Add(Rejoin(currentDoc, separator));
                currentDoc.Clear();
                total = 0;
                while (overlapQueue.Any())
                {
                    var overlap = overlapQueue.Dequeue();
                    var overlapTokens = tokenizer.Encode(overlap,new HashSet<string>()).Count;
                    currentDoc.Add(overlap);
                    total += overlapTokens;
                }
            }
        }
        if (currentDoc.Any()) docs.Add(Rejoin(currentDoc, separator));
        
        return docs;
    }
}
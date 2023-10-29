namespace Casual.BI.LLM.Models;

public class Model
{
    
    public string Name { get; }
    public int ContextTokenLimit { get; }
    
    static Dictionary<string, int> _availableModels = new()
    {
        {"gpt-3.5-turbo", 4000},
        {"gpt-3.5-turbo-16k", 16000},
        {"gpt-4", 8000},
        {"gpt-4-32k", 32000},
    };
    
    Model(string name, int contextTokenLimit)
    {
        Name = name;
        ContextTokenLimit = contextTokenLimit;
    }

    public static implicit operator Model(string model)
    {
        if (!_availableModels.ContainsKey(model))
            throw new ArgumentOutOfRangeException(nameof(model), model, "Must be a supported model");

        return new Model(model, _availableModels[model]);
    }
    
    public static implicit operator string(Model model) => model.Name;
}
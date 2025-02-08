using System.Text;

namespace coaches.Modules.Shared.Application.Common.Helpers;

public class PromptBuilder
{
    private readonly List<string> _messages = [];
    private readonly Dictionary<string, string> _variables = new();
    private Type? _responseType;

    public PromptBuilder WithVariable(string key, string value)
    {
        _variables[key] = value;
        return this;
    }

    public PromptBuilder User(string content)
    {
        _messages.Add(content);
        return this;
    }

    public PromptBuilder FormatAs<T>()
    {
        _responseType = typeof(T);
        return this;
    }

    public string Build()
    {
        StringBuilder prompt = new();

        foreach (string message in _messages)
        {
            string processedMessage = _variables
                .Aggregate(message, (current, variable) => current.Replace($"{{{variable.Key}}}", variable.Value));

            prompt.AppendLine(processedMessage);
        }

        if (_responseType is not null)
        {
            IEnumerable<string> properties = _responseType.GetProperties()
                .Select(p => $"\"{p.Name}\": \"\"");
            string jsonStructure = $"{{ {string.Join(", ", properties)} }}";
            prompt.AppendLine($"Return JSON {jsonStructure}, add no extra formatting for markdown.");
        }

        return prompt.ToString().TrimEnd();
    }
}

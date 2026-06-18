using System.Text.Json.Serialization;

namespace playwrightbook.Model
{
    internal class Playbook
    {
        [JsonPropertyName("step")]
        public string? Step { get; set; }

        [JsonRequired]
        [JsonPropertyName("action")]
        public string Action { get; set; } = string.Empty;

        [JsonPropertyName("args")]
        public PlaybookArgs? Args { get; set; }
    }

    internal class PlaybookArgs
    {
        [JsonPropertyName("selector")]
        public string? Selector { get; set; }

        [JsonPropertyName("value")]
        public string? Value { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("delay")]
        public int? Delay { get; set; }

        [JsonPropertyName("timeout")]
        public int? Timeout { get; set; }
    }
}

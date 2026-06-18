using playwrightbook.Model;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace playwrightbook.Runtime
{
    internal static class PlaybookLoader
    {
        private static readonly HashSet<string> ValidActions = new(StringComparer.OrdinalIgnoreCase)
        {
            "check", "click", "delay", "doubleclick", "download",
            "elementexists", "fill", "focus", "goback", "goto",
            "hover", "press", "select", "scrollto",
            "type", "uploadfile"
        };

        public static async Task<List<Playbook>> LoadAsync(string filePath)
        {
            string json;
            try
            {
                json = await File.ReadAllTextAsync(filePath, System.Text.Encoding.UTF8);
            }
            catch (IOException ex)
            {
                throw new PlaybookException($"讀取 playbook 失敗：{ex.Message}", 30);
            }

            List<Playbook>? items;
            try
            {
                items = JsonSerializer.Deserialize<List<Playbook>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (JsonException ex)
            {
                throw new PlaybookException($"Playbook JSON 格式錯誤：{ex.Message}", 20);
            }

            if (items is null || items.Count == 0)
                throw new PlaybookException("Playbook 是空的或無法解析。", 20);

            Validate(items);
            SerializeData(items);
            return items;
        }

        private static void Validate(List<Playbook> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                var loc = item.Step is not null
                    ? $"第 {i + 1} 個步驟 (step: {item.Step})"
                    : $"第 {i + 1} 個步驟";

                if (string.IsNullOrWhiteSpace(item.Action))
                    throw new PlaybookException($"{loc}：action 不能為空。", 20);

                if (!ValidActions.Contains(item.Action))
                    throw new PlaybookException($"{loc}：未知的 action \"{item.Action}\"。", 20);

                ValidateArgs(item, loc);
            }
        }

        private static void ValidateArgs(Playbook item, string loc)
        {
            var a = item.Action.ToLowerInvariant();
            var args = item.Args;

            // actions that require selector
            bool needsSelector = a is "check" or "click" or "doubleclick" or "download"
                or "elementexists" or "fill" or "focus" or "hover" or "press"
                or "scrollto" or "select" or "type" or "uploadfile";

            if (needsSelector)
            {
                if (args is null || string.IsNullOrEmpty(args.Selector))
                    throw new PlaybookException($"{loc} ({a})：缺少必填的 args.selector。", 20);
            }

            // actions that require value (excluding press/type which allow null/empty at runtime)
            if (a is "fill" or "select" or "uploadfile")
            {
                if (args is null || args.Value is null)
                    throw new PlaybookException($"{loc} ({a})：缺少必填的 args.value。", 20);
            }

            // goto requires url
            if (a is "goto")
            {
                if (args is null || string.IsNullOrEmpty(args.Url))
                    throw new PlaybookException($"{loc} ({a})：缺少必填的 args.url。", 20);
            }

            // delay requires delay (number)
            if (a is "delay")
            {
                if (args is null || args.Delay is null)
                    throw new PlaybookException($"{loc} ({a})：缺少必填的 args.delay（毫秒數值）。", 20);
            }
        }

        private static void SerializeData(List<Playbook> items)
        {
            var reg = new Regex(@"[\\/:*?""<>|]");
            for (int i = 0; i < items.Count; i++)
            {
                items[i].Step = reg.Replace(items[i].Step,"");
            }
        }
    }
}

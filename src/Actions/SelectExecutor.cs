using Microsoft.Playwright;
using playwrightbook.Model;

namespace playwrightbook.Actions
{
    internal class SelectExecutor : IActionExecutor
    {
        public async Task<string> ExecuteAsync(PlaybookArgs? args, ActionContext context)
        {
            var timeout = args?.Timeout ?? 30000;
            var locator = context.Page.Locator(args!.Selector!);
            var values = args.Value!.Split(',', StringSplitOptions.TrimEntries);
            await locator.SelectOptionAsync(values, new LocatorSelectOptionOptions { Timeout = timeout });
            return $"Selected [{string.Join(", ", values)}] in {args.Selector}";
        }
    }
}

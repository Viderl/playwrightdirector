using Microsoft.Playwright;
using playwrightbook.Model;

namespace playwrightbook.Actions
{
    internal class CheckExecutor : IActionExecutor
    {
        public async Task<string> ExecuteAsync(PlaybookArgs? args, ActionContext context)
        {
            var timeout = args?.Timeout ?? 30000;
            var locator = context.Page.Locator(args!.Selector!);
            await locator.CheckAsync(new LocatorCheckOptions { Timeout = timeout });
            return $"Checked {args.Selector}";
        }
    }
}

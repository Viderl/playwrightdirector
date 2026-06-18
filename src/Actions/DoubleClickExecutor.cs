using Microsoft.Playwright;
using playwrightbook.Model;

namespace playwrightbook.Actions
{
    internal class DoubleClickExecutor : IActionExecutor
    {
        public async Task<string> ExecuteAsync(PlaybookArgs? args, ActionContext context)
        {
            var timeout = args?.Timeout ?? 30000;
            var locator = context.Page.Locator(args!.Selector!);
            await locator.DblClickAsync(new LocatorDblClickOptions
            {
                Delay = (float?)args.Delay,
                Timeout = timeout
            });
            return $"Double-clicked {args.Selector}";
        }
    }
}

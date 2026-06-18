using Microsoft.Playwright;
using playwrightbook.Model;

namespace playwrightbook.Actions
{
    internal class ClickExecutor : IActionExecutor
    {
        public async Task<string> ExecuteAsync(PlaybookArgs? args, ActionContext context)
        {
            var timeout = args?.Timeout ?? 30000;
            var locator = context.Page.Locator(args!.Selector!);
            await locator.ClickAsync(new LocatorClickOptions
            {
                Delay = (float?)args.Delay,
                Timeout = timeout,
            });
            return $"Clicked {args.Selector}";
        }
    }
}

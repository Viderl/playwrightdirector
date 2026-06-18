using Microsoft.Playwright;
using playwrightbook.Model;

namespace playwrightbook.Actions
{
    internal class ScrollToExecutor : IActionExecutor
    {
        public async Task<string> ExecuteAsync(PlaybookArgs? args, ActionContext context)
        {
            var timeout = args?.Timeout ?? 30000;
            var locator = context.Page.Locator(args!.Selector!);
            await locator.ScrollIntoViewIfNeededAsync(
                new LocatorScrollIntoViewIfNeededOptions { Timeout = timeout });
            return $"Scrolled to {args.Selector}";
        }
    }
}

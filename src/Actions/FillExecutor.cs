using Microsoft.Playwright;
using playwrightbook.Model;

namespace playwrightbook.Actions
{
    internal class FillExecutor : IActionExecutor
    {
        public async Task<string> ExecuteAsync(PlaybookArgs? args, ActionContext context)
        {
            var timeout = args?.Timeout ?? 30000;
            var locator = context.Page.Locator(args!.Selector!);
            await locator.FillAsync(args.Value!, new LocatorFillOptions { Timeout = timeout });
            return $"Filled {args.Selector} with \"{args.Value}\"";
        }
    }
}

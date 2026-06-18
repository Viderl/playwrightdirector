using Microsoft.Playwright;
using playwrightbook.Model;

namespace playwrightbook.Actions
{
    internal class PressExecutor : IActionExecutor
    {
        public async Task<string> ExecuteAsync(PlaybookArgs? args, ActionContext context)
        {
            if (string.IsNullOrEmpty(args?.Value))
                return "Skipped (value 為空)";

            var timeout = args.Timeout ?? 30000;
            var locator = context.Page.Locator(args.Selector!);
            await locator.PressAsync(args.Value, new LocatorPressOptions
            {
                Delay = (float?)args.Delay,
                Timeout = timeout
            });
            return $"key \"{args.Value}\" pressed on {args.Selector}";
        }
    }
}

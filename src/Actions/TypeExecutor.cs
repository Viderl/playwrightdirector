using Microsoft.Playwright;
using playwrightbook.Model;

namespace playwrightbook.Actions
{
    internal class TypeExecutor : IActionExecutor
    {
#pragma warning disable CS0612 // Locator.TypeAsync is deprecated but required by spec
        public async Task<string> ExecuteAsync(PlaybookArgs? args, ActionContext context)
        {
            if (string.IsNullOrEmpty(args?.Value))
                return "Skipped (value 為空)";

            var timeout = args.Timeout ?? 30000;
            var locator = context.Page.Locator(args.Selector!);
            await locator.TypeAsync(args.Value, new LocatorTypeOptions
            {
                Delay = (float?)args.Delay,
                Timeout = timeout
            });
            return $"typed \"{args.Value}\" into {args.Selector}";
        }
#pragma warning restore CS0612
    }
}

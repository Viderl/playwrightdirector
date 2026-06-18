using Microsoft.Playwright;
using playwrightbook.Model;

namespace playwrightbook.Actions
{
    internal class ElementExistsExecutor : IActionExecutor
    {
        public async Task<string> ExecuteAsync(PlaybookArgs? args, ActionContext context)
        {
            var timeout = args?.Timeout ?? 30000;
            var locator = context.Page.Locator(args!.Selector!);

            var stateStr = args.Value ?? "Visible";
            var state = stateStr.ToLowerInvariant() switch
            {
                "attached" => WaitForSelectorState.Attached,
                _          => WaitForSelectorState.Visible
            };

            await locator.WaitForAsync(new LocatorWaitForOptions
            {
                State = state,
                Timeout = timeout
            });

            return $"Element {args.Selector} is {stateStr}";
        }
    }
}

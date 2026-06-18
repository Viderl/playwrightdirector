using Microsoft.Playwright;
using playwrightbook.Model;

namespace playwrightbook.Actions
{
    internal class GotoExecutor : IActionExecutor
    {
        public async Task<string> ExecuteAsync(PlaybookArgs? args, ActionContext context)
        {
            var timeout = args?.Timeout ?? 30000;
            await context.Page.GotoAsync(args!.Url!, new PageGotoOptions { Timeout = timeout });
            return $"Navigated to {args.Url}";
        }
    }
}

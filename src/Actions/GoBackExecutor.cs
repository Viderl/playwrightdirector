using Microsoft.Playwright;
using playwrightbook.Model;

namespace playwrightbook.Actions
{
    internal class GoBackExecutor : IActionExecutor
    {
        public async Task<string> ExecuteAsync(PlaybookArgs? args, ActionContext context)
        {
            var timeout = args?.Timeout ?? 30000;
            await context.Page.GoBackAsync(new PageGoBackOptions { Timeout = timeout });
            return "Navigated back";
        }
    }
}

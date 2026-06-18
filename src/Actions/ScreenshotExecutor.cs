using playwrightbook.Model;

namespace playwrightbook.Actions
{
    internal class ScreenshotExecutor : IActionExecutor
    {
        public Task<string> ExecuteAsync(PlaybookArgs? args, ActionContext context)
        {
            return Task.FromResult("screenshot taken");
        }
    }
}

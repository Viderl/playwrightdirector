using playwrightbook.Model;

namespace playwrightbook.Actions
{
    internal class DelayExecutor : IActionExecutor
    {
        public async Task<string> ExecuteAsync(PlaybookArgs? args, ActionContext context)
        {
            await Task.Delay(args!.Delay!.Value);
            return $"Delayed {args.Delay}ms";
        }
    }
}

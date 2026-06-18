using Microsoft.Playwright;
using playwrightbook.Model;

namespace playwrightbook.Actions
{
    internal class UploadFileExecutor : IActionExecutor
    {
        public async Task<string> ExecuteAsync(PlaybookArgs? args, ActionContext context)
        {
            var timeout = args?.Timeout ?? 30000;
            var filePath = Path.IsPathRooted(args!.Value!)
                ? args.Value!
                : Path.Combine(Environment.CurrentDirectory, args.Value!);

            var locator = context.Page.Locator(args.Selector!);
            await locator.SetInputFilesAsync(filePath,
                new LocatorSetInputFilesOptions { Timeout = timeout });
            return $"Uploaded {filePath} to {args.Selector}";
        }
    }
}

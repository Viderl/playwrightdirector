using Microsoft.Playwright;
using playwrightbook.Model;

namespace playwrightbook.Actions
{
    internal class DownloadExecutor : IActionExecutor
    {
        public async Task<string> ExecuteAsync(PlaybookArgs? args, ActionContext context)
        {
            var timeout = args?.Timeout ?? 30000;
            var locator = context.Page.Locator(args!.Selector!);
            var browserContext = context.Page.Context;

            // 同時等待：當前頁下載 或 新分頁出現
            var currentPageDownloadTask = context.Page.WaitForDownloadAsync(new PageWaitForDownloadOptions { Timeout = timeout });
            var newPageTask = browserContext.WaitForPageAsync(new BrowserContextWaitForPageOptions { Timeout = timeout });

            await locator.ClickAsync(new LocatorClickOptions { Timeout = timeout });

            var completed = await Task.WhenAny(currentPageDownloadTask, newPageTask);

            IDownload download;
            IPage? newPage = null;

            if (completed == currentPageDownloadTask)
            {
                download = await currentPageDownloadTask;
            }
            else
            {
                newPage = await newPageTask;
                download = await newPage.WaitForDownloadAsync(new PageWaitForDownloadOptions { Timeout = timeout });
            }

            var fileName = download.SuggestedFilename;
            var savePath = context.OutputPaths.UniquePath(fileName, false);
            await download.SaveAsAsync(savePath);

            if (newPage is not null)
                await newPage.CloseAsync();

            return $"Downloaded {Path.GetFileName(savePath)}";
        }
    }
}

using Microsoft.Playwright;
using playwrightbook.Runtime;

namespace playwrightbook.Actions
{
    internal record ActionContext(
        IPage Page,
        LoggerAdapter Logger,
        ScreenshotService Screenshots,
        OutputPaths OutputPaths,
        string Step,
        string ActionName
    );
}

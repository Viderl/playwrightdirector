using Microsoft.Playwright;
using playwrightbook.Actions;
using playwrightbook.Model;
using playwrightbook.Runtime;

namespace playwrightbook.Runtime
{
    internal class PlaywrightRunner
    {
        private readonly OutputPaths _outputPaths;
        private readonly LoggerAdapter _logger;

        public PlaywrightRunner(OutputPaths outputPaths, LoggerAdapter logger)
        {
            _outputPaths = outputPaths;
            _logger = logger;
        }

        public async Task<int> RunAsync(string playbookFile)
        {
            // ── Load & validate playbook ──────────────────────────────
            List<Playbook> items;
            try
            {
                items = await PlaybookLoader.LoadAsync(playbookFile);
            }
            catch (PlaybookException ex) when (ex.ExitCode == 30)
            {
                _logger.Error("", "load", "", ex.Message);
                return ex.ExitCode;
            }
            catch (PlaybookException ex)
            {
                _logger.Error("", "load", "", ex.Message);
                return ex.ExitCode;
            }

            // ── Launch browser ────────────────────────────────────────
            var screenshotService = new ScreenshotService(_outputPaths);
            var dispatcher = new ActionDispatcher(_logger, screenshotService);

            var playwright = await Playwright.CreateAsync();
            IBrowser? browser = null;
            try
            {
                browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Channel = "msedge",
                    Headless = false,
                    SlowMo = 25
                });

                var page = await browser.NewPageAsync();
                // ── Execute actions ───────────────────────────────────
                foreach (var item in items)
                {
                    var ctx = new ActionContext(
                        Page: page,
                        Logger: _logger,
                        Screenshots: screenshotService,
                        OutputPaths: _outputPaths,
                        Step: item.Step ?? "",
                        ActionName: item.Action.ToLowerInvariant()
                    );

                    int? errorCode = await dispatcher.DispatchAsync(item, ctx);
                    if (errorCode.HasValue)
                        return errorCode.Value;
                }

                return 0;
            }
            finally
            {
                if (browser is not null)
                    await browser.CloseAsync();
                playwright.Dispose();
            }
        }
    }
}

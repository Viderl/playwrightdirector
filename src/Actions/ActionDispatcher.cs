using playwrightbook.Model;
using playwrightbook.Runtime;

namespace playwrightbook.Actions
{
    internal class ActionDispatcher
    {
        private readonly LoggerAdapter _logger;
        private readonly ScreenshotService _screenshots;
        private readonly ErrorPolicy _errorPolicy = new();
        private readonly Dictionary<string, IActionExecutor> _executors;

        public ActionDispatcher(LoggerAdapter logger, ScreenshotService screenshots)
        {
            _logger = logger;
            _screenshots = screenshots;
            _executors = new Dictionary<string, IActionExecutor>(StringComparer.OrdinalIgnoreCase)
            {
                ["check"]         = new CheckExecutor(),
                ["click"]         = new ClickExecutor(),
                ["delay"]         = new DelayExecutor(),
                ["doubleclick"]   = new DoubleClickExecutor(),
                ["download"]      = new DownloadExecutor(),
                ["elementexists"] = new ElementExistsExecutor(),
                ["fill"]          = new FillExecutor(),
                ["focus"]         = new FocusExecutor(),
                ["goback"]        = new GoBackExecutor(),
                ["goto"]          = new GotoExecutor(),
                ["hover"]         = new HoverExecutor(),
                ["press"]         = new PressExecutor(),
                ["select"]        = new SelectExecutor(),
                ["scrollto"]      = new ScrollToExecutor(),
                ["type"]          = new TypeExecutor(),
                ["uploadfile"]    = new UploadFileExecutor(),
            };
        }

        /// <summary>
        /// Dispatches the playbook item to the correct executor.
        /// Returns null to continue, or an exit code to stop.
        /// </summary>
        public async Task<int?> DispatchAsync(Playbook item, ActionContext context)
        {
            var executor = _executors[item.Action.ToLowerInvariant()];
            try
            {
                var message = await executor.ExecuteAsync(item.Args, context);
                var screenshotPath = await context.Screenshots.TakeAsync(context.Page, context.Step, context.ActionName, isError: false);
                var img = screenshotPath is not null ? Path.GetFileName(screenshotPath) : "";
                _logger.Info(context.Step, context.ActionName, img, message);
                return null;
            }
            catch (Exception ex)
            {
                var screenshotPath = await context.Screenshots.TakeAsync(context.Page, context.Step, context.ActionName, isError: true);
                var img = screenshotPath is not null ? Path.GetFileName(screenshotPath) : "";
                _logger.Error(context.Step, context.ActionName, img, ex.Message);

                int? exitCode = _errorPolicy.Classify(ex);
                return exitCode;
            }
        }
    }
}

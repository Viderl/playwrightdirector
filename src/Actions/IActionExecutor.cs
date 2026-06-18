using playwrightbook.Model;

namespace playwrightbook.Actions
{
    internal interface IActionExecutor
    {
        /// <summary>
        /// Executes the action. Returns the success message. Throws on error.
        /// </summary>
        Task<string> ExecuteAsync(PlaybookArgs? args, ActionContext context);
    }
}

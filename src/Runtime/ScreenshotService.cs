using Microsoft.Playwright;

namespace playwrightbook.Runtime
{
    internal class ScreenshotService
    {
        private readonly OutputPaths _outputPaths;

        public ScreenshotService(OutputPaths outputPaths)
        {
            _outputPaths = outputPaths;
        }

        /// <summary>
        /// 截圖並儲存，使用台北時間戳記。
        /// 一般截圖：{timestamp}_{step}_{action}.png
        /// 錯誤截圖：{timestamp}_{action}_error.png
        /// </summary>
        public async Task<string?> TakeAsync(IPage page, string step, string action, bool isError)
        {
            try
            {
                var ts = DateTime.Now.ToString(OutputPaths.SCREENSHOT_TIME_FORMAT);
                string baseName;
                
                if (isError)
                {
                    // 錯誤截圖不包含 step，格式：{timestamp}_{action}_error.png
                    baseName = string.IsNullOrEmpty(step)
                        ? $"{ts}_{action}_error.png"
                        : $"{ts}_{step}_{action}_error.png";
                }
                else
                {
                    baseName = string.IsNullOrEmpty(step) 
                        ? $"{ts}_{action}.png" 
                        : $"{ts}_{step}_{action}.png";
                }

                var path = _outputPaths.UniquePath(baseName,true);
                await page.ScreenshotAsync(new PageScreenshotOptions { Path = path });
                return path;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[WARNING] 截圖失敗 ({action}): {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 向後相容：通用截圖方法（已淘汰，改用 TakeAsync）
        /// </summary>
        public async Task<string?> TakeInternalAsync(IPage page, string actionName, bool isError)
        {
            return await TakeAsync(page, "", actionName, isError);
        }
    }
}

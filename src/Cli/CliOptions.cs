using System.CommandLine;

namespace playwrightbook.Cli
{
    internal class CliOptions
    {
        public string? PlaybookPath { get; private set; }
        public string? OutputPath { get; private set; }
        public bool ShowHelp { get; private set; }
        public bool ShowVersion { get; private set; }
        public bool HasError { get; private set; }
        public string? ErrorMessage { get; private set; }

        private CliOptions() { }

        public static CliOptions Parse(string[] args)
        {
            var opts = new CliOptions();

            // ── 建立 Option 對象 ────────────────────────────────────
            var inputOption = new Option<string?>("-i")
            {
                Description = "執行劇本的 UTF-8 JSON 檔案路徑（絕對或相對）"
            };

            var outputOption = new Option<string?>("-o")
            {
                Description = "指定 output 目錄（絕對或相對），預設 ./output"
            };

            var helpOption = new Option<bool>("-h")
            {
                Description = "顯示說明"
            };

            var versionOption = new Option<bool>("-v")
            {
                Description = "顯示版本資訊"
            };

            // ── 建立 RootCommand ────────────────────────────────────
            var rootCommand = new RootCommand("playwrightbook — Playwright 自動化測試執行器")
            {
                inputOption,
                outputOption,
                helpOption,
                versionOption
            };

            // ── 解析參數 ──────────────────────────────────────────
            var parseResult = rootCommand.Parse(args);

            // ── 檢查解析結果中是否有錯誤 ────────────────────────────
            if (parseResult.Errors.Count > 0)
            {
                opts.HasError = true;
                opts.ErrorMessage = string.Join("; ", parseResult.Errors.Select(e => e.Message));
                return opts;
            }

            // ── 提取參數值 ────────────────────────────────────────
            opts.PlaybookPath = parseResult.GetValue(inputOption);
            opts.OutputPath = parseResult.GetValue(outputOption);
            opts.ShowHelp = parseResult.GetValue(helpOption);
            opts.ShowVersion = parseResult.GetValue(versionOption);

            return opts;
        }

        public static void PrintHelp()
        {
            Console.WriteLine("""
playwrightbook — Playwright 自動化測試執行器

用法：
  playwrightbook [-i <playbook>] [-o <output_dir>] [-h] [-v]

參數：
  -i <file>   執行劇本的 UTF-8 JSON 檔案路徑（絕對或相對）。
              未指定時自動尋找目前目錄的 PlaybookTemplate.json。
              找不到 playbook 時輸出說明並結束（exit 10）。
  -o <dir>    指定 output 根目錄（絕對或相對）。
              預設：./output
  -h          顯示本說明。
  -v          顯示版本資訊。

Exit Code：
  0   程式正常結束
  10  輸入參數錯誤（包含找不到 playbook）
  20  Playbook JSON 格式錯誤
  30  System.IO 錯誤
  40  Playwright 相關錯誤累計超過 3 次
""");
        }

        public static void PrintVersion()
        {
            Console.WriteLine($"playwrightbook v{Program.VERSION}");
        }
    }
}

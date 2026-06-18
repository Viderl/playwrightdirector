# `playwrightbook` v0.0.2 開發計畫

根據 [spec.md](spec.md) 版本 0.0.2，制定本計畫。

## 版本概要

**版本：0.0.2**  
**發行日期：2026-06-17**

基於 v0.0.1 基礎建設完成，v0.0.2 著重於補齊全部 17 個 Action 、完善錯誤處理與日誌規格、實作下載與檔案上傳功能。

## 現況評估

| 模組 | 狀態 | 備註 |
|------|------|------|
| CLI 參數解析 | ✅ | `-p / -s / -t / -h / -v` 已支援 |
| Playbook 載入 & 驗證 | ✅ | Model 含 `step / action / args` |
| 基礎 Action | ⚠️ | `goto / click / fill / delay / doubleclick` 已實作；其餘 12 個待補齊 |
| 截圖與命名 | ⚠️ | 一般截圖實作，錯誤截圖與序號需完善 |
| Logger | ⚠️ | 基礎格式已有，需調整為結構化日誌 |
| 錯誤處理 | ⚠️ | IO 立停 / Playwright 累計已框架化；需補齊邊界情況 |
| 下載功能 | ❌ | `download` action 待實作 |
| 檔案上傳 | ❌ | `uploadfile` action 待實作 |
| 版本命令 | ❌ | `-v` 應輸出 `0.0.2` |

## 開發里程碑

### M1 — 補齊核心 Action（優先順序高）

**預期工期：3 天**

#### 1.1 檢查類 Action

- **`elementexists`**
  - 檢查指定元素是否存在
  - 呼叫 `Locator.WaitForAsync(state: "Attached"|"Visible")`
  - 支援 `timeout` 參數（預設 30000ms）
  - Log 成功訊息格式：`element {selector} state:{value} confirmed`

- **`check`**
  - 勾選指定 checkbox
  - 呼叫 `Locator.CheckAsync()`
  - Log 成功：`checkbox {selector} checked`

#### 1.2 滑鼠互動類 Action

- **`doubleclick`**
  - 雙擊指定元素
  - 呼叫 `Locator.DblClickAsync()`
  - 支援 `delay` 參數（mouse down / up 間延遲）
  - Log 成功：`element {selector} double-clicked`

- **`hover`**
  - 滑鼠移動到指定元素
  - 呼叫 `Locator.HoverAsync()`
  - Log 成功：`element {selector} hovered`

- **`focus`**
  - 將焦點移到指定元素
  - 呼叫 `Locator.FocusAsync()`
  - Log 成功：`element {selector} focused`

#### 1.3 輸入與選擇類 Action

- **`type`**
  - 模擬逐字輸入（與 `fill` 差異：fill 是一次性輸入）
  - 呼叫 `Locator.TypeAsync(value)`
  - 支援 `delay` 參數（keypress 間延遲）
  - `value` 為空字串/null 時不觸發輸入，不寫 error log（視為跳過）
  - Log 成功：`typed "{value}" into {selector}`

- **`press`**
  - 模擬按鍵操作（不支援組合鍵）
  - 呼叫 `Locator.PressAsync(value)`
  - `value` 為空字串/null 時不觸發，不寫 error log
  - Log 成功：`key "{value}" pressed on {selector}`

- **`select`**
  - 在下拉選單選擇選項
  - 呼叫 `Locator.SelectOptionAsync()`
  - `value` 支援逗號分隔多選：`"opt1,opt2"`
  - Log 成功：`selected "{value}" in {selector}`

#### 1.4 頁面導航類 Action

- **`goback`**
  - 返回上一頁
  - 呼叫 `Page.GoBackAsync(options)`
  - Log 成功：`navigated back`

- **`scrollto`**
  - 捲動到指定元素
  - 呼叫 `Locator.ScrollIntoViewIfNeededAsync()`
  - Log 成功：`scrolled to {selector}`

### M2 — 檔案操作 Action

**預期工期：2 天**

#### 2.1 `download` Action

- **功能**：下載指定元素的檔案
- **流程**：
  ```csharp
  var waitTask = page.WaitForDownloadAsync();
  await Locator.ClickAsync();
  var download = await waitTask;
  // 目錄：output/{yyyyMMddHHmm}/download/
  await download.SaveAsAsync(path);
  ```
- **檔案名稱衝突處理**：
  - 若 `{下載檔案名稱}.ext` 已存在，改為 `{下載檔案名稱}_1.ext`、`_2.ext` 等（序號不補零，從 1 開始）
- **Log 成功**：`downloaded {filename} to {full-path}`
- **異常處理**：
  - 下載逾時 → error log + error screenshot
  - 保存失敗（IO 錯誤）→ 立即 exit 30

#### 2.2 `uploadfile` Action

- **功能**：上傳檔案至指定表單元素（單檔上傳）
- **行為**：呼叫 `Locator.SetInputFilesAsync()`
- **路徑解析**：
  - 先當絕對路徑，再當相對 `Environment.CurrentDirectory`
  - 檔案不存在 → error log + error screenshot + IO 錯誤 exit 30
- **Log 成功**：`uploaded file {filepath} to {selector}`

### M3 — 日誌與截圖完善

**預期工期：2 天**

#### 3.1 日誌格式確認與實作

- **格式**：`timestamp | step | action | level | message`
- **timestamp**：`yyyy-MM-dd HH:mm:ss.fff`（台北時間 UTC+8，無時區標記）
- **level**：`info` 或 `error`
- **Console 輸出**：
  - `info` 級別 → 預設 Console.Out
  - `error` 級別 → `Console.Error` 且以紅色字體顯示（`ConsoleColor.Red`）
- **檔案寫入**：同步 append 至 log 檔，路徑 `output/{yyyyMMddHHmm}/{yyyyMMddHHmm}.log`

#### 3.2 截圖規則完善

- **一般截圖**：`{timestamp}_{step}_{action}.png`
- **錯誤截圖**：`{timestamp}_{action}_error.png`
- **timestamp 格式**：`yyyyMMddHHmmssfff`（台北時間，精確到毫秒）
- **檔案名稱衝突處理**：
  - 若檔案已存在，自動加序號：`{timestamp}_{action}_error_1.png`、`_2.png` 等（序號不補零，從 1 開始）
  - 避免覆蓋現有截圖
- **錯誤截圖觸發時機**：
  - 任何 action 執行失敗（Playwright / IO / Timeout 例外）
  - 截圖本身失敗時不丟出異常，以 fallback log 紀錄

### M4 — 錯誤處理強化

**預期工期：2 天**

#### 4.1 Exit Code 映對

| Code | 條件 | 處理流程 |
|------|------|---------|
| 0 | 程式正常結束 | 執行完所有 action，browser 正常關閉 |
| 10 | 輸入參數或 playbook 路徑錯誤 | 印說明，exit 10 |
| 20 | Playbook JSON 格式錯誤 / 驗證失敗 | error log + JSON 解析/驗證錯誤訊息，exit 20 |
| 30 | System.IO 錯誤 | error log + error screenshot，立即 exit 30 |
| 40 | Playwright 相關錯誤累計 > 3 次 | error log + error screenshot（第 4 次觸發），exit 40 |

#### 4.2 Playwright 錯誤計數

- **計數對象**：`PlaywrightException`、`TimeoutException` 及其衍生例外
- **計數機制**：
  - 每次捕獲 Playwright 相關例外，`playwrightErrorCount++`
  - `playwrightErrorCount <= 3` 時，寫 error log + error screenshot，繼續執行下一個 action
  - `playwrightErrorCount > 3` 時，exit 40
- **邊界情況**：
  - 連續多個 action 都失敗：各自計次
  - 某個 action 內部拋多次異常：僅計 1 次（外層 catch 一次）

#### 4.3 IO 錯誤分類

以下例外立即 exit 30，不計 Playwright 錯誤次數：
- `IOException` 及其衍生（`DirectoryNotFoundException`、`FileNotFoundException` 等）
- 截圖失敗
- log 檔寫入失敗
- 下載檔案保存失敗

### M5 — 版本與說明命令

**預期工期：1 天**

#### 5.1 `-v` 命令

- **輸出**：`playwrightbook v0.0.2`（或類似格式，版本來自 csproj 或常數定義）
- **Exit Code**：0

#### 5.2 `-h` 命令

- **內容**：
  - 參數列表表格（`-p / -s / -t / -h / -v`）
  - 各參數說明與預設值
  - Exit Code 表格
- **Exit Code**：0

#### 5.3 版本常數管理

- 在 `Program.cs` 或專用 `VersionInfo.cs` 中定義版本常數 `VERSION = "0.0.2"`
- 保持與 `.csproj` 一致

### M6 — 整合測試與除錯

**預期工期：2 天**

#### 6.1 測試用例

| 測試項目 | 預期行為 | 驗收條件 |
|---------|---------|--------|
| 完整 playbook 執行 | 所有 17 個 action 順序執行 | 截圖數 = step 數，log 記錄完整 |
| Playbook JSON 格式錯誤 | exit 20，log 含格式錯誤訊息 | exit code 正確 |
| 缺少必填 args | exit 20，log 含缺失欄位訊息 | exit code 正確 |
| 下載檔案衝突 | 第 2 次同名下載為 `_1`、`_2` 等 | 檔案數正確，序號遞增 |
| Playwright 連續失敗 4 次 | 第 4 次失敗後 exit 40 | exit code 40 |
| 截圖路徑不存在 | 自動建立目錄，截圖成功 | 目錄與檔案均存在 |
| log 檔大小 | 支援大量 action（百筆以上）| log 檔無截斷，記錄完整 |
| 台北時間驗證 | timestamp 反映台北時區 | 與系統台北時間誤差 < 1秒 |

#### 6.2 邊界情況驗證

- Playbook 空陣列 → 程式正常結束（exit 0）
- 元素不存在的 action → Playwright 例外，計次
- 多次快速重複執行 → output 目錄名稱含分鐘時間，避免覆蓋前次結果
- URL 無效 → Playwright 例外，計次

## 檔案與模組更新清單

### 新增/修改檔案

| 檔案 | 狀態 | 用途 |
|------|------|------|
| `Actions/DownloadExecutor.cs` | 🆕 | download action 實作 |
| `Actions/UploadFileExecutor.cs` | 🆕 | uploadfile action 實作 |
| `Actions/CheckExecutor.cs` | 🆕 | check action 實作 |
| `Actions/ElementExistsExecutor.cs` | 🆕 | elementexists action 實作 |
| `Actions/HoverExecutor.cs` | 🆕 | hover action 實作 |
| `Actions/FocusExecutor.cs` | 🆕 | focus action 實作 |
| `Actions/TypeExecutor.cs` | 🆕 | type action 實作 |
| `Actions/PressExecutor.cs` | 🆕 | press action 實作 |
| `Actions/SelectExecutor.cs` | 🆕 | select action 實作 |
| `Actions/GoBackExecutor.cs` | 🆕 | goback action 實作 |
| `Actions/ScrollToExecutor.cs` | 🆕 | scrollto action 實作 |
| `Runtime/Logger.cs` | ✏️ | 確認日誌格式、Console.Error 紅色輸出 |
| `Runtime/ErrorPolicy.cs` | ✏️ | IO / Playwright 計數邊界完善 |
| `Runtime/ScreenshotService.cs` | ✏️ | 錯誤截圖、序號衝突處理 |
| `Cli/CliOptions.cs` | ✏️ | `-v` 命令實作 |
| `Program.cs` | ✏️ | 版本常數、exit code 收斂 |

### 版本標記

- [ ] `playwrightbook.csproj` 版本更新至 `0.0.2`
- [ ] `doc/spec.md` 版本標記確認為 `0.0.2`
- [ ] Tag 發行版本：`git tag v0.0.2`

## 風險與緩解

| 風險 | 影響 | 緩解方案 |
|------|------|---------|
| Playwright 非同步操作複雜 | 某些 action 實作延期 | 優先實作基礎 action，複雜交互類後置 |
| 檔案系統路徑相容性（Windows） | 路徑分隔符、長路徑問題 | 使用 `Path.Combine()`，提前測試 >260 字元路徑 |
| log 檔鎖定與 append 效能 | 高頻日誌寫入阻塞 | 採用 `StreamWriter` 帶 buffer，定期 flush |
| 截圖檔案巨大 | 磁碟空間耗盡 | 提醒用戶定期清理 output，future 可加壓縮選項 |
| 下載檔案逾時 | 網路不穩定時卡住 | `timeout` 參數可配置，預設 30 秒 |

## 驗收準則

v0.0.2 發行前需滿足：

- ✅ 全 17 個 action 均可執行且 log 記錄完整
- ✅ 日誌格式符合規格，console 顯示正確
- ✅ 截圖規則（命名、衝突、錯誤截圖）完整實作
- ✅ Exit code（0 / 10 / 20 / 30 / 40）依規格返回
- ✅ 下載、上傳功能完整測試通過
- ✅ 台北時間、output 路徑生成邏輯驗證無誤
- ✅ 單機測試、多次執行結果一致性確認
- ✅ doc 更新：README、CHANGELOG、使用示例

## 參考

- [spec.md](spec.md) — v0.0.2 功能規格
- [PlaybookTemplate.json](../src/PlaybookTemplate.json) — playbook 範例
- [Program.cs](../src/Program.cs) — 主程式入點
- Playwright .NET 文檔：https://playwright.dev/dotnet/

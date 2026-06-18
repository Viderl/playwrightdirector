# Playwright測試程式
## 版本資訊
- 版本：0.0.3
## 系統概述
本系統為一套以 C# + Playwright .NET 10 實作之測試執行器，負責：

- 讀取 JSON 腳本（Playbook），依序執行其中定義的動作（Action）。
- 逐步執行測試腳本，每做一個步驟截圖一次。截圖檔名命名方式參考[截圖規則](#截圖規則)  
- 如果執行過程中發生System.IO錯誤則立即停止。
- 如果執行過程中發生Playwright相關錯誤累計超過3次則立即停止。
- 輸出 log到文字檔和console
- 程式結束關閉瀏覽器
## 執行環境
- 作業系統：Windows 11
## 名詞定義
- Playbook：一個 JSON 格式的測試腳本，包含一系列要執行的動作（action）和對應的參數（args）。
- Action：Playbook 中定義的每個動作，代表一個具體的操作，例如點擊、輸入文字、截圖等。
- Args：每個 Action 所需的參數，包含選擇器、輸入值、網址、延遲時間等。
- 相對路徑：相對於程式執行目錄（Environment.CurrentDirectory）的路徑。
- 時區：程式使用的時區為台北時間（UTC+8）。輸出log不須加上時區資訊，但timestamp需為台北時間。
- output目錄：程式執行後產生的輸出檔案（如截圖、下載檔案、log檔）存放的根目錄。定義Path.Combine(Environment.CurrentDirectory,"output",DateTime.Now.ToString("yyyyMMddHHmm"))。yyyyMMddHHmm為程式開始執行的年月日時分。
## 預設瀏覽器設定
使用Edge以外瀏覽器需要執行powershell安裝，不建議使用。
```
var playwright = await Playwright.CreateAsync();
var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
{
    Channel = "msedge",
    Headless = false,
    SlowMo = 25
});
```
## CLI 參數規格
### 參數列表
| 參數 | 必填 | 說明 | 預設 |
|------|------|------|------|
| -i | ❌ | 執行劇本的UTF-8 json檔。先當作絕對路徑找，沒有從相對路徑找。當-p都沒有提供時，到Environment.CurrentDirectory找PlaybookTemplate.json，最終沒有找到json檔，則輸出"說明"並結束程式，Exit Code=10 | ./PlaybookTemplate.json |
| -o | ❌ | 指定output目錄可接受相對路徑。沒給預設放到Path.Combine(Environment.CurrentDirectory,"output")這個目錄，輸出格式參考[目錄原則](#目錄原則) | ./output |
| -h | ❌ | 顯示說明。解釋各個參數的用途 | 無 |
| -v | ❌ | 顯示版本資訊 | 無 |
### Exit Code
| Code | 說明 |
|------|------|
| 0 | 程式正常結束 |
| 10 | 輸入參數錯誤 |
| 20 | Playbook JSON 格式錯誤 |
| 30 | System.IO錯誤 |
| 40 | Playwright相關錯誤累計超過3次 |

## Playbook JSON 格式
### 基本結構
```
[
  {
    "action": "string",
    "args": {}
  }
]
..
```
### Schema 定義
```
{
  "step": "string (optional)，執行步驟編號或名稱，僅供識別用，執行器不會對step做任何處理",
  "action": "string (required)",
  "args": {
    "selector": "string (optional)",
    "value": "string (optional)",
    "url": "string (optional)",
    "delay": "number (optional)，delay的等待時間，單位毫秒",
    "timeout": "number (optional)，預設30秒鐘，如果action需要使用到timeout參數但沒有提供則使用預設值"
  }
}
```
| 參數 | 必填 | 說明 | 範例 |
|------|------|------|------|
| action | ✅ | 指定要執行的動作，完整列表請參考[Action 定義](#action-定義) | goto |
| args.selector | ❌ | 指定元素的選擇器 | #accountID |
| args.value | ❌ | 指定要輸入的值 | 910301 |
| args.url | ❌ | 指定要前往的網址 | https://ebptest.tmnewa.com.tw/ |
| args.delay | ❌ | 指定延遲時間（毫秒）| 1000 |
| args.timeout | ❌ | 指定操作的超時時間（毫秒） | 30000 |
每個action需要的args都不同，請參考[Action 定義](#action-定義)中各個action的args說明。
### 範例
```
[
  {
    "step": "login 1",
    "action": "goto",
    "args": {
      "url": "https://ebptest.tmnewa.com.tw/"
    }
  },
  {
    "step": "login 2",
    "action": "click",
    "args": {
      "selector": "a[href=\"/Account/LogOff\"]"
    }
  },
  {
    "step": "login fill account",
    "action": "fill",
    "args": {
      "selector": "#accountID",
      "value": "910301"
    }
  }
]
```
## 檔案存放
### 目錄原則
- 每次執行都在output 目錄下建立子目錄，子目錄格式是yyyyMMddHHmm，程式開始執行的年月日時分，例如output/202406171630。
- 截圖存放目錄: Path.Combine(output目錄, {yyyyMMddHHmm}, "screenshots")
- 下載檔案存放目錄: Path.Combine(output目錄, {yyyyMMddHHmm}, "download")
- log檔案路徑: Path.Combine(output目錄, {yyyyMMddHHmm}, "log.csv")
### 截圖命名規則
- 使用 Page.ScreenshotAsync()時一般截圖命名為{timestamp}\_{step}\_{action}.png 
- 發生錯誤時的截圖命名為{timestamp}\_{step}\_{action}_error.png
- timestamp 格式為 yyyyMMddHHmmssfff，如果重複則自動加上序號避免覆蓋，序號從1開始，不補零。例如{timestamp}\_{step}\_{action}_error_1.png、{timestamp}\_{step}\_{action}_error_2.png等
### 下載檔案命名規則
- 下載檔案名稱如果重複則自動加上序號避免覆蓋，序號從1開始，不補零。例如{下載檔案名稱}_1.ext、{下載檔案名稱}_2.ext等

## Log 規格
### 格式
```
timestamp | step| action | level | message
```
- timestamp: 格式為 yyyy-MM-dd HH:mm:ss.fff，不指定時區
- step: 執行步驟編號或名稱，僅供識別用
- action: 執行的動作名稱
- level: log 等級，分為 info 和 error
- img: 截圖檔名
- message: log 訊息
### 範例
```
2024-06-01 12:00:00.000 | login 1 | goto | info | 20260617164355243.png | https://ebptest.tmnewa.com.tw/
2024-06-01 12:00:05.000 | quotation a | click | error | 20260617162155243_error.png | Element a[href="/Account/LogOff"] does not contain text "登出"
```
### Console 輸出
Console 輸出格式同 log 檔案，並且在 error 狀態時以紅色字體顯示。

## Action 定義
### 目錄
- [check](#check)
- [click](#click)
- [delay](#delay)
- [doubleclick](#doubleclick)
- [download](#download)
- [elementexists](#elementexists)
- [fill](#fill)
- [focus](#focus)
- [goback](#goback)
- [goto](#goto)
- [hover](#hover)
- [press](#press)
- [select](#select)
- [scrollto](#scrollto)
- [switchpage](#switchpage)
- [type](#type)
- [uploadfile](#uploadfile)
### Action 詳細說明
- 每個action在執行前都要確認必要的args是否存在，如果缺少必要的args，則在讀入Playbook時做JsonSerializer.Deserialize就要解析出來，並中斷執行
- 執行每個action正常結束，則寫一筆log，message內容為該action的成功訊息
- 執行每個action發生錯誤，則寫一筆log，message內容為該action的錯誤訊息，並且截取error screenshot
- 當action不存在時，在一開始讀入Playbook，做JsonSerializer.Deserialize就要解析出來，並中斷執行
- 以下action說明有列出的都是需要的參數，除非特別說明，否則都是必填
#### check
- 勾選指定的checkbox  
- 行為： Locator.CheckAsync()  
```
{
  "selector": "string",
  "timeout": "number (optional)"
}
```
#### click
- 點擊指定元素  
- 行為： Locator.ClickAsync()  
```
{
  "selector": "string",
  "delay": "mouse down和mouse up之間的延遲時間，單位毫秒，選填",
  "timeout": "number (optional)"
}
```
#### delay
- 延遲時間，單位毫秒  
- 行為： Task.Delay(delay)  
```
{
  "delay": "延遲時間，單位毫秒"
}
```
#### doubleclick
- 雙擊指定元素  
- 行為： Locator.DblClickAsync()  
```
{
  "selector": "string",
  "delay": "mouse down和mouse up之間的延遲時間，單位毫秒，選填",
  "timeout": "number (optional)"
}
```
#### download
- 下載指定元素的檔案  
- 行為：
```
var waitTask = page.WaitForDownloadAsync();
await Locator.ClickAsync();
var download = await waitTask;
await download.SaveAsAsync(path);
```
- 下載後儲存到"output目錄"，存放方式參考[檔案存放](#檔案存放)，並將下載檔案檔名寫入log。
```
{
  "selector": "string",
  "timeout": "number (optional)"
}
```
#### elementexists
- 檢查指定元素是否存在
- 行為： Locator.WaitForAsync(state: Attached/Visible)  
```
{
  "selector": "string",
  "value": "Attached或Visible，選填，預設為Visible",
  "timeout": "number (optional)"
}
```
#### fill
- 在指定元素清空後輸入文字  
- 行為： Locator.FillAsync()  
```
{
  "selector": "string",
  "value": "輸入文字",
  "timeout": "number (optional)"
}
```
#### focus
- 將焦點移到指定元素
- 行為： Locator.FocusAsync()  
```
{
  "selector": "string",
  "timeout": "number (optional)"
}
```
#### goback
- 返回上一頁  
- 行為：Page.GoBackAsync(options);
```
{
  "timeout": "timeout毫秒，選填，預設30000"
}
```
#### goto
- 前往指定網址  
- 行為： page.GotoAsync(url)  
```
{
  "url": "string",
  "timeout": "timeout毫秒，選填，預設30000"
}
```
#### hover
- 滑鼠移動到指定元素上  
- 行為： Locator.HoverAsync()  
```
{
  "selector": "string",
  "timeout": "number (optional)"
}
```
#### press
- 在指定元素上模擬按鍵操作，不支援組合鍵  
- 當value是空字串或null時，則不會輸入任何文字，且不觸發type的行為
- 行為： Locator.PressAsync()  
```
{
  "selector": "string",
  "value": "按鍵值，如 Enter、Tab、ArrowDown 等",
  "delay": "mouse down和mouse up之間的延遲時間，單位毫秒，選填",
  "timeout": "number (optional)"
}
```
#### select
- 在指定元素選擇下拉選單的值
- 行為： Locator.SelectOptionAsync()  
```
{
  "selector": "string",
  "value": "選項值，選項的value值，多選時以逗號分隔(,)",
  "timeout": "number (optional)"
}
```
#### scrollto
- 滾動到指定元素
- 行為： Locator.ScrollIntoViewIfNeededAsync()  
```
{
  "selector": "string",
  "timeout": "number (optional)"
}
```
### switchpage
- 切換到指定頁籤

var pages = context.Pages;

var secondPage = pages[1];

await secondPage.BringToFrontAsync();

#### type
- 在指定元素輸入文字，與fill的差異在於type會模擬人打字效果，逐字輸入並且每個字之間有短暫延遲
- 當value是空字串或null時，則不會輸入任何文字，且不觸發type的行為
- 行為： Locator.TypeAsync()
```
{
  "selector": "string",
  "value": "輸入文字",
  "delay": "keypress之間的延遲時間，單位毫秒，選填",
  "timeout": "number (optional)"
}
```
#### uploadfile
- 上傳檔案，不支援多檔上傳  
- 行為： Locator.SetInputFilesAsync()  
- 檔案路徑是絕對路徑或相對路徑，相對路徑定義參考[名詞定義](#名詞定義)
```
{
  "selector": "string",
  "value": "檔案路徑",
  "timeout": "number (optional)"
}
```
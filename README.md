#### 1️⃣让CefSharp在项目二级目录运行

****

1. 在**程序运行目录**新建一个文件夹存放CefSharp文件，例如  `"resources\chromium"`
```CefSharp类库文件获取可以使用Nuget安装，安装成功后按F5运行一次程序就会在程序运行目录生成相关库文件了。```

2. 添加CefSharp引用
   [upl-image-preview url=https://cdn.ncii.cn/bbs/assets/files/2022-04-30/1651307014-765224-image.png]
3. 设置引用`"复制本地"`为False
   [upl-image-preview url=https://cdn.ncii.cn/bbs/assets/files/2022-04-30/1651307060-280620-image.png]
4. 在`"App.config"`中添加以下 runtime，其中 `"privatePath"` 为CefSharp的存放路径

```config
<runtime>
	<assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
		<probing privatePath="resources/chromium"/>
	</assemblyBinding>
</runtime>
```

5. 在程序入口处添加初始化Cef函数

```cs
  /// <summary>
        ///     初始化Cef配置
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void InitializeCef()
        {
            var settings = new CefSettings
            {
                BrowserSubprocessPath = browser,
                LocalesDirPath = locales,
                ResourcesDirPath = res
            };
            Cef.Initialize(settings);
        }
```

6. 初始化Cefsharp
lib、browser、locals、res的路径填你存放的路径，这里我存放在```"resources\chromium\"```
```csharp
 private static void Main()
        {
            lib = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"resources\chromium\libcef.dll");
            browser = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"resources\chromium\CefSharp.BrowserSubprocess.exe");
            locales = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"resources\chromium\locales\");
            res = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"resources\chromium\");

            var libraryLoader = new CefLibraryHandle(lib);
            var isValid = !libraryLoader.IsInvalid;
            if (isValid)
            {
                InitializeCef();
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
```

7. 在调用CefSharp的窗口类实例化ChromiumWebBrowser类即可

```csharp
            String url = "http://map.baidu.com/";

            // Initialize cef with the provided settings
            // Create a browser component
            browser = new ChromiumWebBrowser(url);

            // Add it to the form and fill it to the form window.
            this.Controls.Add(browser);
            browser.Dock = DockStyle.Fill;

            // Allow the use of local resources in the browser
            BrowserSettings browserSettings = new BrowserSettings();
            browser.BrowserSettings = browserSettings;
```

[upl-image-preview url=https://cdn.ncii.cn/bbs/assets/files/2022-04-30/1651308473-401583-rmmv3tjn0bxb5xe88g6.png]


#### 2️⃣ JavaScript交互

****

1. C# 调用JS函数不获取返回值
```csharp
browser.ExecuteScriptAsync("alert('This message from JavaScript');");
```
[upl-image-preview url=https://cdn.ncii.cn/bbs/assets/files/2022-04-30/1651309939-883253-08zt3hriyui-at-etxi-5r.png]

2. C# 调用JS函数并获取返回值
建议是最好用await，或者你可以把代码写在ContinueWith里面。
```
        async private void button1_Click(object sender, EventArgs e)
        {
            var javaScript = @"(function () {
                return document.getElementsByTagName('title')[0].innerText;
            })();";
            var result = await browser.GetMainFrame().EvaluateScriptAsync(javaScript)
            .ContinueWith(t =>
            {
                var _result = t.Result;
                return _result.Result;
            });
            MessageBox.Show(result.ToString());
        }
```
[upl-image-preview url=https://cdn.ncii.cn/bbs/assets/files/2022-04-30/1651310025-639497-svskio0u8knbz2kzlij.png]

3. JavaScript调用C#函数 传参+返回
首先在C#新写一个类给JS调用
```csharp
public class CSharpObject
        {
            public string getMessage(string msg)
            {
                string ret = $"This message return from C#: {msg}";
                return ret;
            }
        }
```
然后把写好的类实例化注册给Browser，如果是CefSharp的版本比较老的话可能要用注释掉的那一行来注册对象。
```csharp
browser.JavascriptObjectRepository.Settings.LegacyBindingEnabled = true;
//browser.RegisterAsyncJsObject("CSharpObject", new CSharpObject());
browser.JavascriptObjectRepository.Register("CSharpObject", new CSharpObject(), isAsync: false, options: BindingOptions.DefaultBinder);
```
JS调用C#函数
方便写我直接在C#调用JS来调用C#的函数了（有点拗口哈哈哈）
```csharp
browser.ExecuteScriptAsync("alert(CSharpObject.getMessage('Test Call'));");
```
[upl-image-preview url=https://cdn.ncii.cn/bbs/assets/files/2022-04-30/1651310275-715953-5-r4w92ckoumyho2.png]


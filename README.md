#### 1️⃣让CefSharp在项目二级目录运行

****

1. 在**程序运行目录**新建一个文件夹存放CefSharp文件，例如  `"resources\chromium"`    
```CefSharp类库文件获取可以使用Nuget安装，安装成功后按F5运行一次程序就会在程序运行目录生成相关库文件了。```

2. 添加CefSharp引用  
   ![image](https://user-images.githubusercontent.com/38845682/166101319-8a6927e3-e1d4-47cf-adce-63cf1829022c.png)

3. 设置引用`"复制本地"`为False  
   ![image](https://user-images.githubusercontent.com/38845682/166101325-f81fc596-1a82-42f9-bf97-3902d15c733a.png)

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

![image](https://user-images.githubusercontent.com/38845682/166101333-f33885f1-cba0-445e-889c-bd873ac13aea.png)



#### 2️⃣ JavaScript交互

****

1. C# 调用JS函数不获取返回值
```csharp
browser.ExecuteScriptAsync("alert('This message from JavaScript');");
```
![image](https://user-images.githubusercontent.com/38845682/166101337-4aca7bd1-d0fa-4f27-be0b-c7839d26fc47.png)

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
![image](https://user-images.githubusercontent.com/38845682/166101340-a71f66ce-0b42-4885-b973-3e91d234e99a.png)

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
![image](https://user-images.githubusercontent.com/38845682/166101344-0d6d4201-fc68-4e0a-9cc0-ac604f1774f5.png)


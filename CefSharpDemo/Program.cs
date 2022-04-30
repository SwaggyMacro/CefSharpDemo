using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CefSharpDemo
{
    static class Program
    {
        private static string lib, browser, locales, res;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            lib = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"resources\chromium\libcef.dll");
            browser = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"resources\chromium\CefSharp.BrowserSubprocess.exe");
            locales = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"resources\chromium\locales\");
            res = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"resources\chromium\");
            Console.WriteLine(lib);
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
    }
}

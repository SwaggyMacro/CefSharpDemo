using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CefSharpDemo
{
    public partial class Form1 : Form
    {
        public class CSharpObject
        {
            public string getMessage(string msg)
            {
                string ret = $"This message return from C#: {msg}";
                return ret;
            }
        }

        private ChromiumWebBrowser browser;

        [Obsolete]
        public Form1()
        {
            InitializeComponent();
            String url = "http://map.baidu.com/";
            browser = new ChromiumWebBrowser(url);
            this.Controls.Add(browser);
            browser.Dock = DockStyle.Fill;
            BrowserSettings browserSettings = new BrowserSettings();
            browser.BrowserSettings = browserSettings;
            browser.JavascriptObjectRepository.Settings.LegacyBindingEnabled = true;
            //browser.RegisterAsyncJsObject("CSharpObject", new CSharpObject());
            browser.JavascriptObjectRepository.Register("CSharpObject", new CSharpObject(), isAsync: false, options: BindingOptions.DefaultBinder);
        }

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

        private void button3_Click(object sender, EventArgs e)
        {
            browser.ExecuteScriptAsync("alert('This message from JavaScript');");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            browser.ExecuteScriptAsync("alert(CSharpObject.getMessage('Test Call'));");
        }
    }
}

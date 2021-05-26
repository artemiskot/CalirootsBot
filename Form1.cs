using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using CefSharp.WinForms;

namespace WindowsFormsApp4
{
    public partial class Form1 : Form
    {
        object _lockAction = new object();
        bool _isCompleted = false;
        bool size_check = false;
        bool in_cart = false;
        bool hidden = false;
        string cart = "https://caliroots.com/cart/view";
        String value = "US 9";
        //ChromiumWebBrowser chrome;



        public Form1()
        {
            InitializeComponent();
            webBrowser1.ProgressChanged += new WebBrowserProgressChangedEventHandler(webBrowser1_ProgressChanged);
            webBrowser1.ScriptErrorsSuppressed = true;
            //this.webBrowser1.Controls.Add(chrome);
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(DocumentCompleted);

            // Установка уровня режима совместимости WebBrowser на 11
            SetWebBrowserCompatiblityLevel();
        }

        async private void DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //if (in_cart = false)
            //{
            timer_check();
            HtmlElementCollection elems = webBrowser1.Document.GetElementsByTagName("select");
                foreach (HtmlElement el in elems)
                {
                    HtmlElementCollection dropdownItems = el.Children;
                    foreach (HtmlElement option in dropdownItems)
                    {
                        value = option.InnerText.Trim();
                        string selected =  comboBox1.Items[comboBox1.SelectedIndex].ToString();
                        if (value == /* "US " + textBox2.Text */selected)
                        {
                            option.SetAttribute("selected", "selected");
                            await Task.Delay(1000);
                            add_click();
                            size_check = true;
                            in_cart = true;
                            break;
                        }
                    }
                }
            if (in_cart == true)
            {
                //MyMethod(3000);
                //webBrowser1.Navigate(cart);
            }
            if (checkBox1.Checked && size_check == false && in_cart==false)
                {
                    //int delay = Convert.ToInt32(textBox3.Text); 
                    await MyMethod(3000);
                }
        }


        #region ИЗМЕНЕНИЕ РЕЖИМА СОВМЕСТИМОСТИ WebBrowser
        // MAGIC
        //http://www.cyberforum.ru/windows-forms/thread1637948.html#post8616074
        //https://blogs.msdn.microsoft.com/patricka/2015/01/12/controlling-webbrowser-control-compatibility/
        //Элемент управления WebBrowser по умолчанию работает в режиме совместимости с IE7. Чтобы переключить его на более новые версии необходимо создать ключ в реестре по пути
        //HKEY_LOCAL_MACHINE\Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION
        // или HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION

        private static void SetWebBrowserCompatiblityLevel()
        {
            string appName = Path.GetFileNameWithoutExtension(Application.ExecutablePath);
            int lvl = 1000 * GetBrowserVersion();
            bool fixVShost = File.Exists(Path.ChangeExtension(Application.ExecutablePath, ".vshost.exe"));

            WriteCompatiblityLevel("HKEY_LOCAL_MACHINE", appName + ".exe", lvl);
            if (fixVShost) WriteCompatiblityLevel("HKEY_LOCAL_MACHINE", appName + ".vshost.exe", lvl);

            WriteCompatiblityLevel("HKEY_CURRENT_USER", appName + ".exe", lvl);
            if (fixVShost) WriteCompatiblityLevel("HKEY_CURRENT_USER", appName + ".vshost.exe", lvl);
        }

        private static void WriteCompatiblityLevel(string root, string appName, int lvl)
        {
            try
            {
                Microsoft.Win32.Registry.SetValue(root + @"\Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", appName, lvl);
            }
            catch (Exception)
            {
            }
        }

        public static int GetBrowserVersion()
        {
            string strKeyPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Internet Explorer";
            string[] ls = new string[] { "svcVersion", "svcUpdateVersion", "Version", "W2kVersion" };

            int maxVer = 0;
            for (int i = 0; i < ls.Length; ++i)
            {
                object objVal = Microsoft.Win32.Registry.GetValue(strKeyPath, ls[i], "0");
                string strVal = Convert.ToString(objVal);
                if (strVal != null)
                {
                    int iPos = strVal.IndexOf('.');
                    if (iPos > 0)
                        strVal = strVal.Substring(0, iPos);

                    int res = 0;
                    if (int.TryParse(strVal, out res))
                        maxVer = Math.Max(maxVer, res);
                }
            }

            return maxVer;
        }
        #endregion

        #region ОБРАБОТКА ЭЛЕМЕНТОВ ФОРМЫ
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string url = @textBox1.Text;
                _navigate(url);
            }
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void Timer1_Tick(object sender, EventArgs e)
        {

        }
        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            string siteUrl = @textBox1.Text;
            _navigate(siteUrl);
        }
        private void TextBox2_TextChanged(object sender, EventArgs e)
        {
        }


        private void Form1_Load(object sender, EventArgs e)
        {
        }
        private void PictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Label3_Click(object sender, EventArgs e)
        {

        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void TextBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void Timer1_Tick_1(object sender, EventArgs e)
        {

        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if(hidden == false)
            { 
                rightPanel.Hide();
                button2.Text = "Открыть";
                hidden = true;
            }
            else
            {
                rightPanel.Show();
                button2.Text = "Скрыть";
                hidden = false;
            }
        }
        #endregion

        #region PRIVATE VOIDS

        private void webBrowser1_ProgressChanged(object sender, WebBrowserProgressChangedEventArgs e)
        {
            if (webBrowser1.ReadyState == WebBrowserReadyState.Complete)
                _isCompleted = true;
        }

        private void _navigate(string url)
        {
            lock (_lockAction)
            {
                webBrowser1.Navigate(url);

                _isCompleted = false;
                while (_isCompleted == false) Application.DoEvents();
            }
        }

        private void add_click()
        {
            foreach (HtmlElement el in webBrowser1.Document.GetElementsByTagName("button"))
            {
                if (el.GetAttribute("className") == "add-to-cart-form-submit  disabled ")
                {
                    el.InvokeMember("Click");
                }
            }
        }
        private void timer_check()
        {
            foreach (HtmlElement el in webBrowser1.Document.GetElementsByTagName("span"))
            {
                if (el.GetAttribute("className") == "date")
                {
                    label4.Show();
                    label5.Text = el.InnerText;
                }
            }
        }
        private async Task MyMethod(int a)
        {
            await Task.Delay(a);
            //webBrowser1.Refresh();
        }

        private void GoToCheckOut()
        {
            foreach (HtmlElement el in webBrowser1.Document.GetElementsByTagName("a"))
            {
                if (el.GetAttribute("className") == "row")
                {
                    el.InvokeMember("click");
                }
            }
        }




        #endregion

        private void ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Button3_Click(object sender, EventArgs e)
        {

        }

        private void Label8_Click(object sender, EventArgs e)
        {

        }

        private void CheckBox2_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}









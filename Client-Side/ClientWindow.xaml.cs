using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IronPython;
using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

namespace Client_Side
{
    public partial class ClientWindow : Window
    {
        private ClientCode cc;
        public ClientWindow()
        {
            InitializeComponent();
            cc = new ClientCode();
            this.DataContext = cc;
        }

        private void bSwitchClientState_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                cc.SwitchClientState();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void bSend_Click(object sender, RoutedEventArgs e)
        {
            cc.SendMessageTo(tbTargetUsername.Text, tbMessage.Text);
        }
        //Sends upon button click by creating the dos python file before and executing it
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string statements;
            ScriptEngine engine;
            generatePythonFile(out statements, out engine);
            // Execute the python source
            dynamic pyScope = engine.CreateScope();
            engine.Execute(statements.ToString(), pyScope);
        }

        private void generatePythonFile(out string statements, out ScriptEngine engine)
        {
            dos dos = new dos();
            statements = dos.createPythonFile(cc.IpAddress, cc.Port, tbTargetUsername.Text);
            // Convert the text into python source code
            engine = Python.CreateEngine();
            engine.CreateScriptSourceFromString(statements, SourceCodeKind.Statements);
        }

        public class user
        {
            public static string _username;
            public static string username { get { return _username; } set { _username = value; } }
            public static string _data;
            public static string data { get { return _data; } set { _data = value; } }
            public static Mutex _mutex;
            public static Mutex mutex { get { return _mutex; } set { _mutex = value; } }
            public static int _total;
            public static int total { get { return _total; } set { _total = value; } }
        }
        // Use the DataObject.Pasting Handler 
        private void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void PortBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
    }
}

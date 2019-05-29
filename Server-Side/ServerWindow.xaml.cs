using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Server_Side
{ 
    public partial class ServerWindow : Window
    {  
        ServerCode cs;
        public ServerWindow()
        {
            InitializeComponent();

            lbActiveClients.SelectionChanged += (_s, _e) =>
            {
                if (lbActiveClients.SelectedValue == null)
                    return;
                if (lbActiveClients.SelectedValue is Client)
                    tbTargetUsername.Text = (lbActiveClients.SelectedValue as Client).Username;
            };


            this.DataContext = cs = new ServerCode();
        }

        private void bSwitchServerState_Click(object sender, RoutedEventArgs e)
        {
            cs.SwitchServerState();
        }

        private void bSend_Click(object sender, RoutedEventArgs e)
        {
            cs.SendMessage(tbTargetUsername.Text, tbMessage.Text);
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

        private void LstChatHistory_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(lstChatHistory, 0);
            scrollViewer.ScrollToEnd();
        }
    }
}

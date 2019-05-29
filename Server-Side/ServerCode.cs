using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Server_Side
{
    public class ServerCode : INotifyPropertyChanged
    {
        private Dispatcher _dispatcher { get; set; }

        private int _clientIdCounter;
        private Thread _thread;
        private Socket _socket;

        private IPAddress _ipAddress;
        private ushort _port;
        public ushort Port
        {
            get  { return _port; }
            set
            {
                if (this.IsServerActive)
                    throw new Exception("Can't change this property when server is active");
                this._port = value;
            }
        }

        private IPEndPoint _ipEndPoint => new IPEndPoint(_ipAddress, _port);

        public string IpAddress
        {
            get {  return _ipAddress.ToString();  }
            set
            {
                if (this.IsServerActive)
                    throw new Exception("Can't change this property when server is active");
                _ipAddress = IPAddress.Parse(value);
            }
        }

        private string _username;
        public string Username
        {
            get {  return _username;  }
            set
            {
                this._username = value;
                if (this.IsServerActive) this.lstClients[0].Username = value; 
            }
        }

        public BindingList<Client> lstClients { get; set; }
        public BindingList<ChatObject> lstChat { get; set; }
         
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propName) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName)); 

        private bool _isServerActive;
        public bool IsServerActive
        {
            get  { return _isServerActive; }
            private set
            {
                this._isServerActive = value;

                this.NotifyPropertyChanged("IsServerActive");
                this.NotifyPropertyChanged("IsServerStopped");
            }
        }
        public bool IsServerStopped => !this.IsServerActive;
        public int ActiveClients => lstClients.Count;

        public ServerCode()
        {
            this._dispatcher = Dispatcher.CurrentDispatcher;
            this.lstChat = new BindingList<ChatObject>();
            this.lstClients = new BindingList<Client>();
            this.lstClients.ListChanged += (_sender, _e) =>
            {
                this.NotifyPropertyChanged("ActiveClients");
            };

            this._clientIdCounter = 0;
            this.IpAddress = "127.0.0.1";
            this.Port = 5960;
            this.Username = "Server";
        }
        public void StartServer()
        {
            if (this.IsServerActive) return;

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Bind(_ipEndPoint);
            _socket.Listen(5);
            _thread = new Thread(new ThreadStart(WaitForConnections));
            _thread.Start();

            lstClients.Add(new Client() { ID = 0, Username = this.Username });

            this.IsServerActive = true;
        }
        public void StopServer()
        {
            if (!this.IsServerActive) return; 
            lstChat.Clear(); 
            while (lstClients.Count != 0)
            {
                Client c = lstClients[0]; 
                lstClients.Remove(c);
                c.Dispose();
            }

            _socket.Dispose();
            _socket = null;

            this.IsServerActive = false;
        }
        public void SwitchServerState()
        {
            if (!this.IsServerActive) this.StartServer();
            else this.StopServer();
        }
        public void SendMessage(string toUsername, string messageContent)  => this.SendMessage(this.lstClients[0], toUsername, messageContent);
        private void SendMessage(Client from, string toUsername, string messageContent)
        {
            string message = from.Username + " : " + toUsername +  " : " + messageContent; 
            bool isSent = false; 
            if (toUsername == this.Username)
            {
                this.lstChat.Add(new ChatObject { Sender = from.Username, Recepient = toUsername, Message = messageContent });
                isSent = true;
            } 
            foreach (Client c in lstClients)
            {
                if (c.Username == toUsername)
                {
                    c.SendMessage(message);
                    isSent = true;
                }
            } 
            if (!isSent) from.SendMessage("**Server**: Error! Username not found, unable to deliver your message"); // send an error to sender
 
        }

        void WaitForConnections()
        {
            while (true)
            {
                if (_socket == null) return;
                Client client = new Client();
                client.ID = this._clientIdCounter;
                client.Username = "NewUser";
                try
                {
                    client.Socket = _socket.Accept();
                    client.Thread = new Thread(() => ProcessMessages(client));

                    this._dispatcher.Invoke(new Action(() =>
                    {
                        lstClients.Add(client);
                    }), null);

                    client.Thread.Start();
                }
                catch { }
            }
        }
        void ProcessMessages(Client c)
        {
            while (true)
            {
                try
                {
                    if (!c.IsSocketConnected())
                    {
                        this._dispatcher.Invoke(new Action(() =>
                        {
                            lstClients.Remove(c);
                            c.Dispose();
                        }), null);

                        return;
                    }

                    byte[] inf = new byte[1024];
                    int x = c.Socket.Receive(inf);
                    if (x > 0)
                    {
                        string strMessage = Encoding.Unicode.GetString(inf);
                        if (strMessage.Substring(0, 8) == "/setname")
                        {
                            string newUsername = strMessage.Replace("/setname ", "").Trim('\0');
                            c.Username = newUsername;
                        }
                        else if (strMessage.Substring(0, 6) == "/msgto")
                        {
                            string data = strMessage.Replace("/msgto ", "").Trim('\0');
                            string targetUsername = data.Substring(0, data.IndexOf(':'));
                            string message = data.Substring(data.IndexOf(':') + 1);

                            this._dispatcher.Invoke(new Action(() =>
                            {
                                SendMessage(c, targetUsername, message);
                            }), null);
                        }
                        //Receiving messages from the clients with the specified begining tag
                        else
                        {
                            StringBuilder strMessage2 = new StringBuilder();
                            strMessage2.Append(Encoding.ASCII.GetString(inf, 0, x));
                            string data = strMessage2.ToString().Replace("/ddos* ", "").Trim('\0');
                            string targetUsername = data.Substring(0, data.IndexOf(':'));
                            string message = data.Substring(data.IndexOf(':') + 1);
                            c.Username = "DDOS User";
                            this._dispatcher.Invoke(new Action(() =>
                            {
                                SendMessage(c, targetUsername, message);
                            }), null);
                        }

                    }
                }
                catch (Exception)
                {
                    this._dispatcher.Invoke(new Action(() =>
                    {
                        lstClients.Remove(c);
                        c.Dispose();
                    }), null);
                    return;
                }
            }
        }
    }

}

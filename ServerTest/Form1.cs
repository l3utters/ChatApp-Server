using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ServerTest
{
    public partial class Form1 : Form
    {
        //Create a connection Socket for the Server that users connect to
        public Socket ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        //List of all connected users' sockets
        public List<Socket> ClientSockets = new List<Socket>();
        //List of all connected users' sockets paired with their username
        public List<User> ConnectedClients = new List<User>();
        //Default buffer for incomming byte stream
        public byte[] ReceiveBuffer = new byte[800000000];

        public string[] users = new string[10];

        public Form1()
        {
            //Start server
            InitializeComponent();
            ServerStartup();
        }

        public void ServerStartup()
        {
            //Instansiate the server socket with any local address and the specified port
            ServerSocket.Bind(new IPEndPoint(IPAddress.Any, 8888));
            //Start listening for client connection, with a buffer of 5 connections at any instance
            ServerSocket.Listen(5);
            //Start Accepting client connectios
            ServerSocket.BeginAccept(new AsyncCallback(AcceptCallBack), null);

        }

        private void AcceptCallBack(IAsyncResult AR)
        {
            //Create socket for new client connection
            Socket socket = ServerSocket.EndAccept(AR);

            //Save client socket in list
            ClientSockets.Add(socket);

            //Update UI with Invoke due to it running on a seperate thread
            OnlineCount.Invoke(new Action(() => OnlineCount.Text = Convert.ToString(ClientSockets.Count)));

            //Start waiting for data from the client
            socket.BeginReceive(ReceiveBuffer, 0, ReceiveBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), socket);

            //Restart the accepting for new clients
            ServerSocket.BeginAccept(new AsyncCallback(AcceptCallBack), null);
        }

        //Async function that gets called when any data stream is received
        private void ReceiveCallBack(IAsyncResult AR)
        {
            try
            {
                Socket socket = (Socket)AR.AsyncState;
                int received = socket.EndReceive(AR);
                byte[] tempBuffer = new byte[received];
                Array.Copy(ReceiveBuffer, tempBuffer, received);

                tempBuffer = Compression.DecompressBytes(tempBuffer);
                byte[] decrypted = Encryption.mydec(tempBuffer);


                string msg = Convert.ToString(decrypted);

                msg = Convert.ToBase64String(decrypted);
                msg = Base64Decode(msg);

                Console.WriteLine(msg);

                //Check is message is from newly connected client
                if (msg.Contains("OnlineConnected8888$$"))
                {
                    //string name = msg.Substring(21);
                    string name = msg.Replace("OnlineConnected8888$$", "");
                    UserList.Invoke(new Action(() => UserList.Items.Add(name)));
                    ConnectedClients.Add(new User(name, socket));
                    string sent = "Connected";

                    byte[] SendBuffer = Convert.FromBase64String(Base64Encode(sent));

                    SendBuffer = Encryption.encryptStream(SendBuffer);

                    SendBuffer = Compression.CompressBytes(SendBuffer);


                    socket.BeginSend(SendBuffer, 0, SendBuffer.Length, SocketFlags.None, new AsyncCallback(SendCallBack), socket);
                    //Send userlist to all connected clients
                    UpdateUsers();
                    //Sends message to all connected clients to show new connected client
                    SendToAll("CON$#$User " + name + " Connected");
                }
                //Check if message is from client disconnecting
                else if (msg.Contains("!Disconnect"))
                {
                    //Remove client socket from list
                    ClientSockets.Remove(socket);
                    //Remove client from client list
                    int index = -1;
                    for (int i = 0; i < ConnectedClients.Count; i++)
                    {
                        if (ConnectedClients[i].GetSocket(socket))
                            index = i;
                    }
                    ConnectedClients.RemoveAt(index);
                    socket.Close();

                    //Update UI
                    OnlineCount.Invoke(new Action(() => OnlineCount.Text = Convert.ToString(ClientSockets.Count)));
                    //string name = msg.Substring(11);
                    string name = msg.Replace("!Disconnect", "");
                    UserList.Invoke(new Action(() => UserList.Items.Remove(name)));
                    //Send new userlist to all connected clients
                    UpdateUsers();
                    Thread.Sleep(50);
                    SendToAll("User " + name + " Disconnected");

                }
                //CHECK IF A NEW PRIVATE CHAT IS OPENED
                else if (msg.Contains("NEWPRIVATECHAT&&$"))
                {
                    //string Receiver_Client = msg.Substring(17);
                    string Receiver_Client = msg.Replace("NEWPRIVATECHAT&&$", "");
                    string[] users = Receiver_Client.Split(':');
                    foreach (var item in ConnectedClients)
                    {
                        if (item.GetName(users[0]))
                        {
                            Socket Receiver = item.GetSocket();

                            byte[] SendBuffer = Convert.FromBase64String(Base64Encode(msg));

                            SendBuffer = Encryption.encryptStream(SendBuffer);
                            SendBuffer = Compression.CompressBytes(SendBuffer);

                            Receiver.BeginSend(SendBuffer, 0, SendBuffer.Length, SocketFlags.None, new AsyncCallback(SendCallBack), Receiver);

                        }
                    }

                }
                //CHECK IF A PRIVATE MESSAGE IS SENT
                else if (msg.Contains("@[PRIVATE]"))
                {
                    string[] message = msg.Split('@');
                    foreach (var item in ConnectedClients)
                    {
                        message[1] = message[1].Replace("[PRIVATE] ", "");

                        if (item.GetName(message[1]))
                        {
                            Socket Receiver = item.GetSocket();

                            string MsgSent = "[PRIVATE] " + message[0];

                            byte[] SendBuffer = Convert.FromBase64String(Base64Encode(MsgSent));
                            SendBuffer = Encryption.encryptStream(SendBuffer);

                            SendBuffer = Compression.CompressBytes(SendBuffer);

                            Receiver.BeginSend(SendBuffer, 0, SendBuffer.Length, SocketFlags.None, new AsyncCallback(SendCallBack), Receiver);

                        }
                    }
                }
                else if (msg.Contains("@VNT[PRIVATE]"))
                {
                    msg = msg.Replace("VNT[PRIVATE] ", "");

                    string[] message = msg.Split('@');

                    foreach (var item in ConnectedClients)
                    {
                        message[1] = message[1].Replace("[PRIVATE] ", "");

                        if (item.GetName(message[1]))
                        {
                            Socket Receiver = item.GetSocket();

                            string MsgSent = "[PRIVATE]VNT@" + message[0] + "@" + message[2];

                            Console.WriteLine(MsgSent);

                            byte[] SendBuffer = Convert.FromBase64String(Base64Encode(MsgSent));
                            SendBuffer = Encryption.encryptStream(SendBuffer);
                            SendBuffer = Compression.CompressBytes(SendBuffer);

                            Receiver.BeginSend(SendBuffer, 0, SendBuffer.Length, SocketFlags.None, new AsyncCallback(SendCallBack), Receiver);

                        }
                    }
                }

                else if (msg.Contains("@IMG[PRIVATE]"))
                {
                    msg = msg.Replace("IMG[PRIVATE] ", "");

                    string[] message = msg.Split('@');

                    foreach (var item in ConnectedClients)
                    {
                        message[1] = message[1].Replace("[PRIVATE] ", "");

                        if (item.GetName(message[1]))
                        {
                            Socket Receiver = item.GetSocket();

                            string MsgSent = "[PRIVATE]IMG@" + message[0] + "@" + message[2];

                            Console.WriteLine(MsgSent);

                            byte[] SendBuffer = Convert.FromBase64String(Base64Encode(MsgSent));
                            SendBuffer = Encryption.encryptStream(SendBuffer);
                            SendBuffer = Compression.CompressBytes(SendBuffer);

                            Receiver.BeginSend(SendBuffer, 0, SendBuffer.Length, SocketFlags.None, new AsyncCallback(SendCallBack), Receiver);

                        }
                    }

                }
                else if (msg.Contains("@VID[PRIVATE]"))
                {
                    msg = msg.Replace("VID[PRIVATE] ", "");

                    string[] message = msg.Split('@');

                    foreach (var item in ConnectedClients)
                    {
                        message[1] = message[1].Replace("[PRIVATE] ", "");

                        if (item.GetName(message[1]))
                        {
                            Socket Receiver = item.GetSocket();

                            string MsgSent = "[PRIVATE]VID@" + message[0] + "@" + message[2];

                            Console.WriteLine(MsgSent);

                            byte[] SendBuffer = Convert.FromBase64String(Base64Encode(MsgSent));
                            SendBuffer = Encryption.encryptStream(SendBuffer);
                            SendBuffer = Compression.CompressBytes(SendBuffer);

                            Receiver.BeginSend(SendBuffer, 0, SendBuffer.Length, SocketFlags.None, new AsyncCallback(SendCallBack), Receiver);

                        }
                    }

                }
                else if (msg.Contains("@AUD[PRIVATE]"))
                {
                    msg = msg.Replace("AUD[PRIVATE] ", "");

                    string[] message = msg.Split('@');

                    foreach (var item in ConnectedClients)
                    {
                        message[1] = message[1].Replace("[PRIVATE] ", "");

                        if (item.GetName(message[1]))
                        {
                            Socket Receiver = item.GetSocket();

                            string MsgSent = "[PRIVATE]AUD@" + message[0] + "@" + message[2];

                            Console.WriteLine(MsgSent);

                            byte[] SendBuffer = Convert.FromBase64String(Base64Encode(MsgSent));
                            SendBuffer = Encryption.encryptStream(SendBuffer);
                            SendBuffer = Compression.CompressBytes(SendBuffer);

                            Receiver.BeginSend(SendBuffer, 0, SendBuffer.Length, SocketFlags.None, new AsyncCallback(SendCallBack), Receiver);

                        }
                    }

                }
                //ANYTHING ELSE IS SIMPLY PASSED ON
                else
                {

                    byte[] SendBuffer = Convert.FromBase64String(Base64Encode(msg));

                    SendBuffer = Encryption.encryptStream(SendBuffer);
                    SendBuffer = Compression.CompressBytes(SendBuffer);


                    foreach (var item in ConnectedClients)
                    {
                        Socket temp = item.GetSocket();
                        if (temp == socket)
                            ;
                        else
                            temp.BeginSend(SendBuffer, 0, SendBuffer.Length, SocketFlags.None, new AsyncCallback(SendCallBack), temp);
                    }
                }

                if (ClientSockets.Contains(socket))
                    socket.BeginReceive(ReceiveBuffer, 0, ReceiveBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), socket);
                else
                    ;
            }
            catch (Exception s)
            {
                Console.WriteLine(s.ToString());

                SendToAll("DISC&&$ALL&&$USERS");

                //UserList.Invoke(new Action(() => UserList.Items.Clear()));

                //foreach(User userclient in ConnectedClients)
                //{
                //    UserList.Invoke(new Action(() => UserList.Items.Add(userclient.Name)));
                //}
                //OnlineCount.Invoke(new Action(() => OnlineCount.Text = UserList.Items.Count.ToString()));
                //UpdateUsers();

            }
        }

        private void SendCallBack(IAsyncResult AR)
        {
            try
            {
                Socket socket = (Socket)AR.AsyncState;
                socket.EndSend(AR);
            }
            catch (Exception p)
            {
                Console.WriteLine(p.ToString());
            }

        }

        private void UpdateUsers()
        {
            try
            {
                string[] usersArray = UserList.Items.OfType<string>().ToArray();
                string users = "CurrentOnline$$" + string.Join(";", usersArray);
                SendToAll(users);
            }
            catch (Exception s)
            {
                Console.WriteLine(s.ToString());
            }

        }

        private void SendToAll(string message)
        {
            try
            {
                byte[] SendBuffer = Convert.FromBase64String(Base64Encode(message));

                SendBuffer = Encryption.encryptStream(SendBuffer);
                SendBuffer = Compression.CompressBytes(SendBuffer);


                foreach (var item in ConnectedClients)
                {
                    Socket temp = item.GetSocket();
                    temp.BeginSend(SendBuffer, 0, SendBuffer.Length, SocketFlags.None, new AsyncCallback(SendCallBack), temp);

                }
            }
            catch (Exception s)
            {
                Console.WriteLine(s.ToString());
            }

        }

        public string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
        public string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            SendToAll("DISC&&$ALL&&$USERS");
            Thread.Sleep(1500);
            //ServerSocket.Disconnect(false);
        }
    }
}
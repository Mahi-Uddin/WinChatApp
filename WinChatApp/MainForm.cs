using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace WinChatApp
{
    public partial class MainForm : Form
    {
        Socket sck;
        EndPoint epLocal, epRemote;
        byte[] buffer;

        public MainForm()
        {
            InitializeComponent();
        }

        private void ShowNameForm()
        {
            NameForm nameForm = new NameForm();
            nameForm.Show();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.yourName == "You")
            {
                ShowNameForm();
            }
            //Set up Socket
            sck = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            sck.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            //Get User IP
            tbLocalIP.Text = GetLocalIP();
            tbRemoteIP.Text = GetLocalIP();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            int localPort;
            int remotePort;
            localPort = Convert.ToInt32(tbLocalPort.Text);
            localPort = int.Parse(tbLocalPort.Text);
            remotePort = Convert.ToInt32(tbRemotePort.Text);
            remotePort = int.Parse(tbRemotePort.Text);
            if (localPort == remotePort)
            {
                MessageBox.Show("Please do not enter same ports.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                ConnectionPart();
            }
        }

        private void ConnectionPart()
        {
            try
            {
                Connection();
            }
            catch (Exception)
            {
                DialogResult result = MessageBox.Show("Connection unsuccessful", "Connection Failed", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                if(result == DialogResult.OK)
                {
                    
                }
                else if (result == DialogResult.Cancel)
                {
                    Environment.Exit(0);
                }
           }
        }

        private void Connection()
        {
            int localPort;
            int remotePort;
            localPort = Convert.ToInt32(tbLocalPort.Text);
            localPort = int.Parse(tbLocalPort.Text);
            remotePort = Convert.ToInt32(tbRemotePort.Text);
            remotePort = int.Parse(tbRemotePort.Text);
            //Bind Socket
            epLocal = new IPEndPoint(IPAddress.Parse(tbLocalIP.Text), localPort);
            sck.Bind(epLocal);

            //Connecting to remote IP
            epRemote = new IPEndPoint(IPAddress.Parse(tbRemoteIP.Text), remotePort);
            sck.Connect(epRemote);

            //Listening the specific port
            buffer = new byte[5000];
            sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);

            //Shows that connection is succesfull
            var result = MessageBox.Show("Connection successfull.", "Connection Succeed", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MessageCallBack(IAsyncResult ar)
        {
            try
            {
                byte[] receivedName = new byte[2000];
                receivedName = (byte[])ar.AsyncState;

                byte[] receivedData = new byte[5000];
                receivedData = (byte[])ar.AsyncState;

                //Converting byte[] to string
                ASCIIEncoding aEncoding = new ASCIIEncoding();
                string receivedMessage = aEncoding.GetString(receivedData);

                //Adding this Message to ListBox
                ASCIIEncoding nEncoding = new ASCIIEncoding();
                lbMessages.Items.Add(nEncoding.GetString(receivedName) + " : " + nEncoding.GetString(receivedData));

                buffer = new byte[5000];
                sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            }

        private void btnSend_Click(object sender, EventArgs e)
        {
            //Convert string myName to byte[]
            string myName = Properties.Settings.Default.yourName;
            ASCIIEncoding nameEncoding = new ASCIIEncoding();
            byte[] sendingName = new byte[2000];
            sendingName = nameEncoding.GetBytes(myName);

            //Convert string Message to byte[]
            ASCIIEncoding aEncoding = new ASCIIEncoding();
            byte[] sendingMessage = new byte[5000];
            sendingMessage = aEncoding.GetBytes(tbMessage.Text);

            //Sending the encoded Message
            sck.Send(sendingName);
            sck.Send(sendingMessage);

            //Adding this Message to ListBox
            //lbMessages.Items.Add(aEncoding.GetString(sendingName) + ": " + aEncoding.GetString(sendingMessage));
        }

        private void btnChangeName_Click(object sender, EventArgs e)
        {
            ShowNameForm();
        }

        private string GetLocalIP()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach(IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();
            }
            return "127.0.0.1";
        }
    }
}

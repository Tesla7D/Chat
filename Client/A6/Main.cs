/*
 * FILE         |   Main.cs
 * DATE         |   4/11/2014
 * AUTHOR       |   Greg Kozyrev (6850549) & Denys Solomonov (6849809)
 * PROJECT      |   Windows Programming Assignmet #6
 * PURPOSE      |   This is Main Form for Chat Client. It allows user to send messages, receive messages.
 *              |   Client keeps updating log for different errors.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace A6
{
    public partial class MainForm : Form
    {
        private int port = 01337;       //Connection will be done on this port

        String username;        //Name of user
        String IPAddress;       //IP Address to connect to
        Boolean connected;      //Connection flag

        Socket sender;          //Socket for communication between client and server

        Listener listener;      //Listener object
        Thread t;               //Thread for listener object

        List<String> messageQuery = new List<String>();     //Query of incomming messages

        public MainForm()
        {
            InitializeComponent();
            username = "noname";
            IPAddress = "127.0.0.1";

            connected = false;
            connectionLabel.Text = "Disconnected";

            //Button is disabled by default, since we are not connected by default
            sendButton.Enabled = false;                 
            sendButton.BackColor = Color.LightGray;

            getSettings();
            connect();      //Try to connect
        }

        private void informationMenuItem_Click(object sender, EventArgs e)
        {
            getSettings();
        }

        private void getSettings()
        {
            Settings settingWindow = new Settings(username, IPAddress);     //Initialize Settinga form.
            settingWindow.ShowDialog();                                     //Show form
            //Take data from settings form
            username = settingWindow.GetName();                             
            IPAddress = settingWindow.GetAddress();
        }

        private void connect()
        {
            try
            {
                System.IO.File.AppendAllText("log.txt", "");        //Open log file

                //Initialize socket
                sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //Try to connect
                sender.Connect(IPAddress, port);
                sendMsg(username + " connected");

                //Create and lauch thread with listener object (listens to incomming messages)
                listener = new Listener(sender, messageQuery);
                t = new Thread(new ThreadStart(listener.Run));
                t.Start();

                //Start timer, which parses message query
                MainTimer.Start();

                //If by this point there were no errors we set different flags and lables
                connected = true;

                sendButton.Enabled = true;
                sendButton.BackColor = Color.LightGreen;

                connectionLabel.Text = "Connected to " + sender.RemoteEndPoint.ToString();
                errorLabel.Text = "";
            }
            catch (SocketException se)
            {
                errorLabel.Text = "Socket Error";
                using (System.IO.StreamWriter dataFile = new System.IO.StreamWriter("log.txt", true))
                {
                    dataFile.WriteLine(DateTime.Now + " | " + se.ToString() + '\n');
                }
            }
            catch (Exception e)
            {
                errorLabel.Text = "Unexcpected Error";
                using (System.IO.StreamWriter dataFile = new System.IO.StreamWriter("log.txt", true))
                {
                    dataFile.WriteLine(DateTime.Now + " | " + e.ToString() + '\n');
                }
            }
        }

        private void disconnect()
        {
            try
            {
                //Trying to shutdown a socket connection
                sendMsg(username + " disconnected");
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();

                //Stop message parsing timer if everything is good
                MainTimer.Stop();

                //Set flags and labels
                connected = false;
                sendButton.Enabled = false;
                sendButton.BackColor = Color.LightGray;
                connectionLabel.Text = "Disconnected";
                errorLabel.Text = "";
            }
            catch (SocketException se)
            {
                errorLabel.Text = "Socket Error";
                using (System.IO.StreamWriter dataFile = new System.IO.StreamWriter("log.txt", true))
                {
                    dataFile.WriteLine(DateTime.Now + " | " + se.ToString() + '\n');
                }
            }
            catch (Exception e)
            {
                errorLabel.Text = "Unexcpected Error";
                using (System.IO.StreamWriter dataFile = new System.IO.StreamWriter("log.txt", true))
                {
                    dataFile.WriteLine(DateTime.Now + " | " + e.ToString() + '\n');
                }
            }
        }

        private void sendMsg(String data)
        {
            //Create array of bytes. Encode user message, and add username and EOL symbol
            byte[] msg = Encoding.ASCII.GetBytes(data);

            //Send mesage to server
            int bytesSent = sender.Send(msg);
        }

        private void connectMenuItem_Click(object sender, EventArgs e)
        {
            if (connected == false)
            {
                connect();      //Try to connect only if we are not connected
            }
        }

        private void disconnectMenuItem_Click(object sender, EventArgs e)
        {
            if (connected == true)
            {
                disconnect();   //Try to disconnect only if we are connected
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (connected == true)
            {
                disconnect();   //Disconnect on closing, if we are connected
            }
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            sendMsg(username + " : " + inputBox.Text + "\n");              //Send message
            inputBox.Text = "";     //Clear input field
        }

        private void MainTimer_Tick(object sender, EventArgs e)
        {
            if (messageQuery.Count > 0)     //If message Query is not empty
            {
                //Add new item to message ListBox
                messageList.Items.Add(messageQuery[0] + '\n');
                //Remove item from the query
                messageQuery.RemoveAt(0);
            }

            if (connected == true)
            {
                //If listener found, that connection is gone (probably server exit)
                if (listener.Connected == false)
                {
                    //Also disconnect
                    disconnect();
                }
            }
        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();     //Exit will automaticly go to FormClosing event
        }

        private void aboutMenuItem_Click(object sender, EventArgs e)
        {
            //Create and show about box
            AboutBox about = new AboutBox();
            about.ShowDialog();
        }
    }
}

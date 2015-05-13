/*
 * FILE         |   Settings.cs
 * DATE         |   4/11/2014
 * AUTHOR       |   Greg Kozyrev (6850549)
 * PROJECT      |   Windows Programming Assignmet #6
 * PURPOSE      |   This is Settings Form for Chat Client. It allows user set username and
 *              |   server's IP Address to connect to
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace A6
{
    public partial class Settings : Form
    {
        String username;        //Username
        String IP_address;      //IP Address

        public Settings()
        {
            InitializeComponent();
        }

        public Settings(String name, String address)
        {
            InitializeComponent();
            inputName.Text = username = name;           //Set input field and username to entered value
            inputAddress.Text = IP_address = address;   //Set input field and IP_address to entered value
        }

        //Returns username
        public String GetName()
        {
            return username;
        }

        //Return IP_address
        public String GetAddress()
        {
            return IP_address;
        }

        //Save entered values on OK pressed and close the form
        private void buttonOK_Click(object sender, EventArgs e)
        {
            username = inputName.Text;
            IP_address = inputAddress.Text;
            this.Close();
        }

        //Just close the form
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

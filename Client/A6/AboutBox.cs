/*
 * FILE         |   AboutBox.cs
 * DATE         |   4/11/2014
 * AUTHOR       |   Greg Kozyrev (6850549)
 * PROJECT      |   Windows Programming Assignmet #6
 * PURPOSE      |   This is About box for Chat Client. It provides some program information to user
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
    public partial class AboutBox : Form
    {
        public AboutBox()
        {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.Close();   //Just close form on click
        }
    }
}

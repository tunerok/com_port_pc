﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using System.IO.Ports;



namespace Com_port

{
    public partial class Inp_f : Form
    {

        String ID_mk;

        public Inp_f()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ID_mk = textBox1.Text;
            try
            {

                PortEr.Ini();
                PortEr.Find_port(ID_mk);
                textBox1.Text = PortEr.strFromPort;

            }
            catch { }

            if (PortEr.MkPortFound == false)
            {
                // textBox1.Text = "Не найдено";
                
                return;
            }
            else
            {
                this.Hide();
                Form1 frm1 = new Form1();
                frm1.ShowDialog();
                
               // Application.Run(new Form1());
                
            }

        }
       
        private void Window_Closing(object sender, EventArgs e)
        {
          
        }

    }
}

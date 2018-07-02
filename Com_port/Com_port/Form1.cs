using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using System.Threading;

namespace Com_port
{
    public partial  class Form1 : Form
    {
        private delegate void InvokeDelegate();
        System.Timers.Timer aTimer;
        String S = ""; 

        public Form1()
        {
            InitializeComponent();
           
        }

        private void tbAux_SelectionChanged(object sender, EventArgs e)
        {
            BeginInvoke(new InvokeDelegate(updateImageBox));
        }




        void Main()
        {
            aTimer = new System.Timers.Timer(1000);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;

        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            S = PortEr.strFromPort;
            throw new NotImplementedException();
        }

        private void updateImageBox( )
        {
            textBox1.Text = S;
        }
    }
}

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
        double Max_voltage = 1.4;
        double current_inp = 0;
        Graphics g;
        Bitmap b;
        int conn_counter = 0;
        bool connect;
        bool paused = false;
        public Form1()
        {
            label2.Text = PortEr._port_finded;//выводим номер порта
            InitializeComponent();
            connect = PortEr.Check_connection();
            b = new Bitmap(pictureBox1.Width, pictureBox1.Height);//сразу объявим картинку как графику ,чтобы упростить с ней взаимодействие
            g = Graphics.FromImage(b);

        }

        private void tbAux_SelectionChanged(object sender, EventArgs e)//метод для объединения потоков(пока хз насколько в таком виде это заработает, но я думаю все будет более-менее)
        {
            BeginInvoke(new InvokeDelegate(updateImageBox));
            Main();
        }

        void Main()
        {
            aTimer = new System.Timers.Timer(1000);//тут у нас второй таймер, достаточно медленный(1 сек), чтобы не задушить порт. Опять же хз насколько быстро нужно считывать с буфера FIFO порта
            aTimer.Elapsed += OnTimedEvent;//вызываем событие по "прерыванию"
            aTimer.AutoReset = true;//авторестарт
            aTimer.Enabled = true;

        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)//Само событие просто считывает значение переменной, которая была забита из класса управления портом - синглтоном
        {

            conn_counter = conn_counter + 1;
            if (conn_counter > 300)
            {
                connect = PortEr.Check_connection();
                conn_counter = 0;
            }

            if (connect)
            {
                S = PortEr.strFromPort;
                button4.Enabled = false;
                label6.Text = PortEr._port_finded;
            }
            else
            {
                label6.Text = "Не найдено";
                button4.Enabled = true;

            }
            


            throw new NotImplementedException();
        }

        private void updateImageBox( )
        {
            try
            {
                current_inp = Convert.ToDouble(S);
                Drawer_my(current_inp, Max_voltage); 
            }
            catch { }

            textBox1.Text = S;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Max_voltage = Convert.ToDouble(textBox1.Text);//считываем опорное напряжение
        }


        private void Drawer_ini()
        {

        }

        //
        //Функция отрисовки. каждую итерацию смещает все изображение на один писель влево и записывает новое значение  
        //в правую часть. Принимает на вход две переменных - полученное значение с АЦП МК и заданное пользователем или дефолтное опорное наряжение
        private void Drawer_my(double r, double v)
        {
            double t_inp;


            //приводим к текушей канве
            t_inp = r / v;
            t_inp *= b.Height;
            t_inp = Math.Round(t_inp);

            PointF [] points = new PointF[150]; 
            //сюда запилисть фор


            g.DrawCurve();

            pictureBox1.Image = (Image)b;

        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                string ID_mk = textBox2.Text;
                PortEr.Ini();
                PortEr.Find_port(ID_mk);

            }
            catch { }

            if (PortEr.MkPortFound == false)
            {
                label6.Text = "Не найдено";
                return;
            }
            else
            {

            }
    }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!paused)
            {
                aTimer.Enabled = false;
                button2.Text = "Продолжить";
            }
            else
            {
                aTimer.Enabled = true;
                button2.Text = "Пауза";
            }
        }
    }

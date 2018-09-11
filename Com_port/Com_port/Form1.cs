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
    public partial class Form1 : Form
    {
        private delegate void InvokeDelegate();
        System.Timers.Timer aTimer;
        PointF[] points = new PointF[614];
        String S = "";
        double Max_voltage;
        double current_inp = 0;
        Graphics g;
        Bitmap b;
        int conn_counter = 0;
        bool connect = true;
        bool paused = false;
        Pen pen = new Pen(Color.Green);
        SolidBrush fig = new SolidBrush(Color.White);
        public Form1()
        {
            InitializeComponent();
            label2.Text = PortEr._port_finded;//выводим номер порта
            
            PortEr.Run_port();
            b = new Bitmap(pictureBox1.Width, pictureBox1.Height);//сразу объявим картинку как графику ,чтобы упростить с ней взаимодействие
            g = Graphics.FromImage(b);
            //g = pictureBox1.CreateGraphics();
            for (int i = 0; i < 614; i++)
            {//инициализируем нужные нам точки 
                points[i].X = i;
                points[i].Y = 0;
            }
            g.FillRectangle(fig, 0, 0, pictureBox1.Width, pictureBox1.Height);
            Max_voltage = Convert.ToDouble(textBox1.Text);
            Main();
        }

        private void tbAux_SelectionChanged(object sender, EventArgs e)//метод для объединения потоков(пока хз насколько в таком виде это заработает, но я думаю все будет более-менее)
        {
            
            Main();
        }

        void Main()
        {
           
                aTimer = new System.Timers.Timer(50);//тут у нас второй таймер, достаточно медленный(1 сек), чтобы не задушить порт. Опять же хз насколько быстро нужно считывать с буфера FIFO порта
                aTimer.Elapsed += OnTimedEvent;//вызываем событие по "прерыванию"
                aTimer.AutoReset = true;//авторестарт
                aTimer.Enabled = true;
            
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)//Само событие просто считывает значение переменной, которая была забита из класса управления портом - синглтоном
        {

            conn_counter = conn_counter + 1;
            if (conn_counter > 300)
            {

               // connect = PortEr.Check_connection();
              //  System.Threading.Thread.Sleep(2000);
                conn_counter = 0;
            }

            if (connect)
            {
                BeginInvoke(new InvokeDelegate(updateImageBox));
                S = PortEr.strFromPort;
                button4.BeginInvoke((Action)delegate () { button4.Enabled = false; });
                ThreadHelperClass.SetText(this, label6, S); //PortEr._port_finded);
            }
            else
            {
                ThreadHelperClass.SetText(this, label6, "Не найдено");
                button4.BeginInvoke((Action)delegate () { button4.Enabled = true; }); 

            }



            //throw new NotImplementedException();
        }

        private void updateImageBox()
        {
            try
            {
                current_inp = Convert.ToDouble(S);
                Drawer_my(current_inp, Max_voltage);
            }
            catch { }

            //textBox1.Text = S;
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
            g.Clear(Color.White);
            double t_inp;
            int i = 0;

            //приводим к текушей канве
            t_inp = r / v;
            t_inp *= b.Height;

            t_inp = b.Height - t_inp + 1;

            if (t_inp > b.Height - 1)
                t_inp = b.Height - 1;


            t_inp = Math.Round(t_inp);



            for (; i < 613; i++)//перебираем все элементы, кроме самого последнего 
                points[i].Y = points[i + 1].Y; // оперируем только  с элементами оординат, т.к. абсцисса "виртуальная и не едет"

            points[613].Y = (float)t_inp;


            //g.DrawCurve(pen, points);


           // Pen redPen = new Pen(Color.Red, 3);
           // Pen greenPen = new Pen(Color.Green, 3);

            // Create points that define curve.
           // Point point1 = new Point(0, 0);
           // Point point2 = new Point(100, 250);
          //  Point point3 = new Point(200, 5);
          //  Point point4 = new Point(250, 50);
          //  Point point5 = new Point(300, 100);
         //   Point point6 = new Point(350, 200);
          //  Point point7 = new Point(250, 250);
          //  Point[] curvePoints = { point1, point2};

            // Draw lines between original points to screen.
         //   g.DrawLines(pen, points);
            g.DrawCurve(pen, points);










            //pictureBox1.Image = (Image)b;
            //pictureBox1.Refresh();
            pictureBox1.Image = b;//выбрать между функциями
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
                paused = !paused;
                aTimer.Enabled = false;
                button2.Text = "Продолжить";
            }
            else
            {
                aTimer.Enabled = true;
                button2.Text = "Пауза";
                paused = !paused;
            }
        }
    }
}

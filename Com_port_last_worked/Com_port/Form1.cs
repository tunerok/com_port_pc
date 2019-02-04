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
        public delegate void DisplayHandler();

        System.Timers.Timer aTimer;
        PointF[] points = new PointF[614];
        String S = "0";
        double Max_voltage;
        double current_inp = 0;
        Graphics g;
        Bitmap b;
        int conn_counter = 0;
        bool connect = true;
        bool paused = false;
        Pen pen = new Pen(Color.Black);
        Pen pen_lvl = new Pen(Color.Red);
        SolidBrush fig = new SolidBrush(Color.White);
        float graph_maxy, graph_miny;
        DisplayHandler handler;
        int writer_counter = 1000;
        int cycle_counter = 0;

        public Form1()
        {

            InitializeComponent();
            textBox2.Text = PortEr._ID_mk;
            label2.Text = PortEr._port_finded;//выводим номер порта
            
           // PortEr.Run_port();
            b = new Bitmap(pictureBox1.Width, pictureBox1.Height);//сразу объявим картинку как графику ,чтобы упростить с ней взаимодействие
            g = Graphics.FromImage(b);
            Max_voltage = Convert.ToDouble(textBox1.Text);
            for (int i = 0; i < 614; i++)
            {//инициализируем нужные нам точки 
                points[i].X = i;
                points[i].Y = (float)Max_voltage/2;
            }
            g.FillRectangle(fig, 0, 0, pictureBox1.Width, pictureBox1.Height);
            
            handler = new DisplayHandler(updateImageBox);
            My_txt_Writer.Ini_file_writer();
            Main();
        }

        private void tbAux_SelectionChanged(object sender, EventArgs e)//метод для объединения потоков(пока хз насколько в таком виде это заработает, но я думаю все будет более-менее)
        {
            
            Main();
        }

        void Main()
        {
           
                aTimer = new System.Timers.Timer(50);//тут у нас второй таймер, для отрисовки и чека порта
                aTimer.Elapsed += OnTimedEvent;//вызываем событие по "прерыванию"
                aTimer.AutoReset = true;//авторестарт
                aTimer.Enabled = true;
            
            
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)//Само событие просто считывает значение переменной, которая была забита из класса управления портом
        {
            if (checkBox5.Checked)//inversion
            {
                pen.Color = Color.Green;
                //pen_lvl.Color = Color.Blue;
                fig.Color = Color.Black; 
            }
            else
            {
                pen.Color = Color.Black;
               //pen_lvl.Color = Color.Red;
                fig.Color = Color.White;
            }
            conn_counter = conn_counter + 1;
            if (conn_counter > 20)
            {

               // connect = PortEr.Check_connection();
              //  System.Threading.Thread.Sleep(2000);
                conn_counter = 0;
            }
            try
            {
                if (connect)
                {

                    //BeginInvoke(new InvokeDelegate(updateImageBox));
                    

                    
					
					
                    
					//if (PortEr.isChanged){
					//	PortEr.isChanged = false;
                        

                    S = PortEr.strFromPort;
                    if (S.Length > 0)
                    {
                        cycle_counter++;
                        if (cycle_counter > writer_counter)
                        {
                            My_txt_Writer.Close_file();
                            My_txt_Writer.Ini_file_writer();
                            cycle_counter = 0;
                        }
                        My_txt_Writer.Append_to_file(S);
                        //button4.BeginInvoke((Action)delegate () { button4.Enabled = false; });

                        ThreadHelperClass.SetText(this, label6, S); //PortEr._port_finded);
                        handler.Invoke();
                    }
					//}
                }
                else
                {
                    ThreadHelperClass.SetText(this, label6, "Не найдено");
                    //button4.BeginInvoke((Action)delegate () { button4.Enabled = true; });

                }
            }
            catch {
                //MessageBox.Show(ex.ToString());
            }

            

            //throw new NotImplementedException();
        }

        private void updateImageBox()
        {
            try
            {
                current_inp = double.Parse(S, System.Globalization.CultureInfo.InvariantCulture);
                graph_miny = (float)Max_voltage;
                graph_maxy = 1;
                Drawer_my(current_inp, Max_voltage);
            }
            catch 
            {
                
            }

            //textBox1.Text = S;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Max_voltage = Convert.ToDouble(textBox1.Text);//считываем опорное напряжение
        }


        private void speed_changed(object sender, EventArgs e)
        {
            aTimer.Interval = 50d / (double)numericUpDown1.Value;
        }

        private void Drawer_ini()
        {

        }

        //
        //Функция отрисовки. каждую итерацию смещает все изображение на один писель влево и записывает новое значение  
        //в правую часть. Принимает на вход две переменных - полученное значение с АЦП МК и заданное пользователем или дефолтное опорное наряжение
        private void Drawer_my(double r, double v)
        {
            try
            {
//MessageBox.Show("tick");
                g.Clear(fig.Color);
                double t_inp;
                int i = 0;

                //приводим к текушей канве
                t_inp = r / v;
                t_inp *= b.Height;

                t_inp = b.Height - t_inp + 1;

                if (t_inp > b.Height - 1)
                    t_inp = b.Height - 1;


                //t_inp = Math.Round(t_inp);



                for (; i < 613; i++)
                {//перебираем все элементы, кроме самого последнего 
                    points[i].Y = points[i + 1].Y; // оперируем только  с элементами оординат, т.к. абсцисса "виртуальная и не едет"
                    if (points[i].Y > graph_maxy)
                        graph_maxy = points[i].Y;
                    if (points[i].Y < graph_miny)
                        graph_miny = points[i].Y;
                }
                points[613].Y = (float)t_inp;

                g.DrawLines(pen, points);
                //g.DrawCurve(pen, points);

                if (checkBox3.Checked)//min
                    g.DrawLine(pen_lvl, 1, graph_maxy, 613, graph_maxy);

                if (checkBox1.Checked)//max
                    g.DrawLine(pen_lvl, 1, graph_miny, 613, graph_miny);
                pictureBox1.Image = b;//выбрать между функциями
                //MessageBox.Show("tack");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                string ID_mk = textBox2.Text;
                PortEr.Ini();
                PortEr.Find_port(ID_mk);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

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

        

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            aTimer.Stop();
            My_txt_Writer.Close_file();
            PortEr.Close_port();
            
            //Application.Exit();
            this.Close();
        }

        
    }
}

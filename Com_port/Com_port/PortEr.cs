using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.IO.Ports;

namespace Com_port
{
    public sealed class PortEr
    {
        private static volatile PortEr instance;
        private static object syncRoot = new Object();
        public static SerialPort _currentPort;
        public static String _ID_mk, _port_finded, strFromPort;
        public static bool MkPortFound;
        private static string[] ports;
        public static Timer aTimer;


        private PortEr() { }

        public static void Ini()
        {
            strFromPort = "";
            Get_ports();
        }



        public static void Find_port(string _ID_mk)
        {
            MkPortFound = false;
            try
            {
                foreach (string port in ports)
                {

                    _currentPort = new SerialPort(port, 9600);
                    if (MkDetected())
                    {
                        MkPortFound = true;
                        _port_finded = port;
                        System.Threading.Thread.Sleep(500);//ждем, не спешим
                        break;
                    }
                    else
                    {
                        MkPortFound = false;
                    }

                }
            }

            catch { }
        }


        public static void Run_port()
        {
            _currentPort.BaudRate = 9600;
            _currentPort.DtrEnable = true;
            _currentPort.ReadTimeout = 1000;

            try
            {
                _currentPort.Open();
            }
            catch { }


            aTimer = new System.Timers.Timer(1000);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;


        }

        private static void OnTimedEvent(object sender, ElapsedEventArgs e)
        {

            if (!_currentPort.IsOpen) return;
            try // так как после закрытия окна таймер еще может выполнится или предел ожидания может быть превышен
            {
                // удалим накопившееся в буфере
                _currentPort.DiscardInBuffer();
                // считаем последнее значение 
                strFromPort = _currentPort.ReadLine();
            }
            catch { }
        }

        

    



        private static void Get_ports()
        {
            ports = SerialPort.GetPortNames();
        }

        public static int Nums_ports()
        {
            //получаем все порты на данном пк
            int i = ports.Length;//для отображения статус бара

            return i;
        }

        private static bool MkDetected()
        {
            try
            {
                _currentPort.Open();
                System.Threading.Thread.Sleep(1000);
                // небольшая пауза, ведь SerialPort не терпит суеты

                string returnMessage = _currentPort.ReadLine();
                _currentPort.Close();

                // необходимо чтобы void loop() в скетче содержал код c ID;
                if (returnMessage.Contains(_ID_mk))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static PortEr getInstance(string name)
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                        instance = new PortEr();
                }
            }
            return instance;
        }

        public static string Get_port()
        {

            return _port_finded;
        }

        ~PortEr()
        {
            aTimer.Enabled = false;
            _currentPort.Close();
        }



    }

 }

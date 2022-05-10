using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace EscPos
{
    public class tcpPrinter : PrinterInterface
    {

        private IPEndPoint eP;
        private Socket s;
        private StreamReader sr;
        private Byte[] buffer = new Byte[256];
        private int bytesrec;

        private String printerHost;
        private Int32 printerPort;

        private Timer keepAliveTimer;

        /// <summary>
        /// TCP Printer Object
        /// </summary>
        /// <param name="pHost">IP address of printer</param>
        /// <param name="pPort">TCP Port</param>
        public tcpPrinter(String pHost, Int32 pPort)
        {
            printerPort = pPort;
            printerHost = pHost;
        }

        public void Close()
        {
            s.Close();
        }

        public bool Init()
        {
            keepAliveTimer = new Timer();
            keepAliveTimer.Interval = 30000;
            keepAliveTimer.Tick += new EventHandler(keepAliveTimer_Tick);
            keepAliveTimer.Enabled = true;
            try
            {
                eP = new IPEndPoint(IPAddress.Parse(printerHost), printerPort);
                s = new Socket(eP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                s.ReceiveTimeout = 100;


                s.Connect(eP);

                
                return true;

            } catch (Exception ex)
            {
                return false;
                //MessageBox.Show(ex.Message, "Error enabling printer");
                //Console.WriteLine(ex);
            }
            
        }

        private void keepAliveTimer_Tick(object sender, EventArgs e)
        {
            // Request paper status
            Write("\x1D" + "\x72" + "\x01");
            
        }

        public bool IsOpen()
        {
            return s.Connected;
        }

        public void Write(string line)
        {
            s.Send(Encoding.UTF8.GetBytes(line));
        }

        public void Write(List<string> lines)
        {
            foreach (string line in lines)
            {
                s.Send(Encoding.UTF8.GetBytes(line));
            }
        }

        public void Read()
        {            
            s.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), buffer);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            int i;
            bytesrec = s.EndReceive(ar);
            Console.Write(DateTime.Now.ToString("HH:mm:ss.fff") + ": ");
            for (i = 0; i < bytesrec; i++)
            {
                // print byte as hex
                
                Console.Write("{0:X2}", buffer[i]);
                Console.Write(" ");

            }
            Console.Write("\n");
            Read();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace PushPOS.printer
{
    public class tcpPrinter : PrinterInterface
    {

        private IPEndPoint eP;
        private Socket s;

        private String printerHost;
        private Int32 printerPort;

        public tcpPrinter(String pHost, Int32 pPort)
        {
            printerPort = pPort;
            printerHost = pHost;
        }

        public void Close()
        {
            s.Close();
        }

        public void Init()
        {
            try
            {
                eP = new IPEndPoint(IPAddress.Parse(printerHost), printerPort);
                s = new Socket(eP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                s.Connect(eP);

            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error enabling printer");
                Console.WriteLine(ex);
            }
            
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
    }
}

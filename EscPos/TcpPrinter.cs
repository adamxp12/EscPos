using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace EscPos
{
    public class TcpPrinter : PrinterInterface
    {
        TcpClient c = new TcpClient();
        NetworkStream stream;

        private String printerHost;
        private Int32 printerPort;

        public TcpPrinter(String pHost, Int32 pPort)
        {
            printerPort = pPort;
            printerHost = pHost;
        }

        public void Close()
        {
            stream.Close();
            c.Close();
        }

        public void Init()
        {
            try
            {
                c.SendTimeout = 3000;
                c.ReceiveTimeout = 3000;
                c.Connect(printerHost, printerPort);
                stream = c.GetStream();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error enabling printer");
                Console.WriteLine(ex);
            }

        }

        public bool IsOpen()
        {
            return c.Connected;
        }

        public void Write(string line)
        {
            stream.Write(Encoding.Unicode.GetBytes(line), 0, line.Length);
            stream.Flush();
        }

        public void Write(List<string> lines)
        {
            foreach (string line in lines)
            {
                stream.Write(Encoding.Unicode.GetBytes(lines.ToString()), 0, lines.ToString().Length);
            }
            stream.Flush();
        }
    }
}

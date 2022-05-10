using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EscPos
{
    public class SerialPrinter : PrinterInterface
    {
        private SerialPort sPrint = new SerialPort();
        private String printerPort;
        private Int32 baudRate;

        public SerialPrinter(String pPort, Int32 bRate)
        {
            printerPort = pPort;
            baudRate = bRate;
        }


        public bool Init()
        {
            try
            {
                sPrint.PortName = printerPort;
                sPrint.BaudRate = baudRate;
                sPrint.Open();
                sPrint.RtsEnable = true;
                return true;
            }
            catch (Exception ex)
            {
                return false;
                //MessageBox.Show(ex.Message, "Error enabling printer");
                //Console.WriteLine(ex);
            }
        }

        public Boolean IsOpen()
        {
            if (sPrint.IsOpen) return true;
            return false;
        }

        public void Close()
        {
            sPrint.Close();
        }


        public void Write(String line)
        {
            sPrint.Write(line);
        }
        public void Write(List<String> lines)
        {
            foreach (string line in lines)
            {
                sPrint.Write(line);
            }

        }

    }
}

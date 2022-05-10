using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EscPos
{
    public class ConsolePrinter : PrinterInterface
    {
        public void Close()
        {
            Console.WriteLine("Printer Closed");
        }

        public bool Init()
        {
            Console.WriteLine("Printer Init");
            return true;
        }

        public bool IsOpen()
        {
            return true;
        }

        public void Write(string line)
        {
            Console.WriteLine("Printer: " + line);
        }

        public void Write(List<string> lines)
        {
            foreach (String line in lines)
            {
                Console.WriteLine("Printer: " + line);
            }
        }
    }
}

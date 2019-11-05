using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace EscPos
{

    [StructLayout(LayoutKind.Sequential)]
    public struct DOCINFO
    {
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pDocName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pOutputFile;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pDataType;
    }


    public class WindowsPrinter : PrinterInterface
    {

        [DllImport("winspool.drv", CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern long OpenPrinter(string pPrinterName, ref IntPtr phPrinter, int pDefault);

        [DllImport("winspool.drv", CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern long StartDocPrinter(IntPtr hPrinter, int Level, ref DOCINFO pDocInfo);

        [DllImport("winspool.drv", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern long StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern long WritePrinter(IntPtr hPrinter, string data, int buf, ref int pcWritten);

        [DllImport("winspool.drv", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern long EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern long EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern long ClosePrinter(IntPtr hPrinter);


        PrintDocument pDoc;
        private String pName;
        private Font fo;
        private List<String> reader;
        IntPtr printer;

        public void Close()
        {

        }

        public WindowsPrinter(String classPName)
        {
            pName = classPName;
        }

        public void Init()
        {
            //fo = new Font("Lucida Console", 10);
            //pDoc = new PrintDocument();
            printer = new IntPtr();
            //pDoc.PrinterSettings.PrinterName = pName;
            //pDoc.PrintPage += new PrintPageEventHandler(this.PrintTextFileHandler);
        }

        public bool IsOpen()
        {
            return true;
        }

        public void Write(string line)
        {

        }

        public void Write(List<string> lines)
        {
            reader = lines;
            StringBuilder sB = new StringBuilder();
            foreach (string line in lines)
            {
                sB.Append(line);
            }

            Print(pName, sB.ToString(), "Reciept");
            //pDoc.Print();
        }

        private void Print(String printerAddress, String text, String documentName)
        {

            // A pointer to a value that receives the number of bytes of data that were written to the printer.
            int pcWritten = 0;

            DOCINFO docInfo = new DOCINFO
            {
                pDocName = documentName,
                pDataType = "RAW"
            };

            OpenPrinter(printerAddress, ref printer, 0);
            StartDocPrinter(printer, 1, ref docInfo);
            StartPagePrinter(printer);

            try
            {
                WritePrinter(printer, text, text.Length, ref pcWritten);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            EndPagePrinter(printer);
            EndDocPrinter(printer);
            ClosePrinter(printer);
        }

        private void PrintTextFileHandler(object sender, PrintPageEventArgs ppeArgs)
        {
            //Get the Graphics object  
            Graphics g = ppeArgs.Graphics;
            float linesPerPage = 0;
            float yPos = 0;
            int count = 0;
            //Read margins from PrintPageEventArgs  
            float leftMargin = ppeArgs.MarginBounds.Left;
            leftMargin = 0;
            float topMargin = ppeArgs.MarginBounds.Top;
            string line = null;
            //Calculate the lines per page on the basis of the height of the page and the height of the font  
            linesPerPage = ppeArgs.MarginBounds.Height / fo.GetHeight(g);
            //Now read lines one by one, using StreamReader  
            while (count < linesPerPage && reader.Count > count && ((line = reader.ElementAt(count)) != null))
            {
                Console.WriteLine(count);
                line = reader.ElementAt(count);
                if (line.Contains((char)'\x1B') || line.Contains((char)'\x1C') || line.Contains((char)'\x1D') || line.Contains((char)'\x1F'))
                {

                }
                else
                {
                    //Calculate the starting position  
                    yPos = topMargin + (count * fo.GetHeight(g));
                    //Draw text  
                    g.DrawString(line, fo, Brushes.Black, leftMargin, yPos, new StringFormat());
                    //Move to next line  
                }

                if (reader.Count > count)
                {
                    line = null;
                }
                count++;
            }
            //If PrintPageEventArgs has more pages to print  
            if (line != null)
            {
                ppeArgs.HasMorePages = true;
            }
            else
            {
                ppeArgs.HasMorePages = false;
            }
        }

    }
}

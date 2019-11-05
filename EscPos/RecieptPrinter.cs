using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace EscPos
{
    public class RecieptPrinter
    {
        const char ESC = '\x1B';
        const char GS = '\x1D';
        const char US = '\x1F';
        public enum JustifyType
        {
            left = 0,
            center = 1,
            right = 2
        }

        public enum ModeType
        {
            FontA = 0,
            FontB = 1,
            Emphasized = 8,
            DoubleHeight = 16,
            DoubleWdith = 32,
            Underline = 128
        }

        public enum FontType
        {
            FontA = 0,
            FontB = 1,
            FontC = 2,
            FontD = 3,
            FontE = 4,
            SpecialFontA = 97,
            SpecialFontB = 98
        }

        public enum DrawType
        {
            Pin2 = 0,
            Pin5 = 1
        }

        private PrinterInterface print;
        private Boolean inlinePole;
        private List<string> sb;

        /// <summary>
        /// Initilize a RecieptPrinter
        /// </summary>
        /// <param name="pT">a PrinterInterface object to use</param>
        /// <param name="iPole">Boolean if inline pole display is in use for Serial only</param>
        public void Init(PrinterInterface pT, Boolean iPole)
        {
            print = pT;
            print.Init();
            inlinePole = iPole;
            sb = new List<String>();
        }

        /// <summary>
        /// Is RecieptPrinter Open
        /// </summary>
        /// <returns></returns>
        public Boolean IsOpen()
        {
            if (print.IsOpen()) return true;
            return false;
        }

        /// <summary>
        /// Close RecieptPrinter
        /// </summary>
        public void Close()
        {
            print.Close();
        }

        /// <summary>
        /// Clear Pole Display
        /// </summary>
        public void ClearDisplay()
        {
            if (inlinePole) { print.Write(((char)'\x0c') + ""); }
        }

        /// <summary>
        ///  Set Pole Display Postistion
        /// </summary>
        /// <param name="pos">Which character posistion to use</param>
        /// <param name="line">Which line to use</param>
        public void SetPosDisplay(int pos, int line)
        {
            if (inlinePole)
            {
                pos = int.Parse(pos.ToString(), NumberStyles.HexNumber);
                line = int.Parse(line.ToString(), NumberStyles.HexNumber);
                Write(US + "$" + ((char)pos) + ((char)line));
            }
        }

        /// <summary>
        /// Reset the printer
        /// </summary>
        public void Reset()
        {
            print.Write(ESC + "@");
        }

        /// <summary>
        /// Write a line to the RecieptPrinter buffer
        /// </summary>
        /// <param name="line"></param>
        public void Write(String line)
        {
            sb.Add(line);
        }

        /// <summary>
        /// Write a line to the RecieptPrinter buffer
        /// </summary>
        /// <param name="line"></param>
        public void WriteLine(String line)
        {
            sb.Add(line + "\n");
        }

        /// <summary>
        /// Write directly to the printer if supported
        /// </summary>
        /// <param name="line"></param>
        public void WriteRaw(String line)
        {
            print.Write(line);
        }

        /// <summary>
        /// Write a line directly to printer if supported
        /// </summary>
        /// <param name="line"></param>
        public void WriteLineRaw(String line)
        {
            print.Write(line + "\n");
        }

        /// <summary>
        /// Flush the buffer out to the printer
        /// </summary>
        public void Flush()
        {
            print.Write(sb);
            sb = new List<String>();
        }

        /// <summary>
        /// Set text justification
        /// </summary>
        /// <param name="pos"></param>
        public void Justify(JustifyType pos)
        {
            switch (pos)
            {
                case JustifyType.left:
                    Write(ESC + "a0");
                    break;
                case JustifyType.center:
                    Write(ESC + "a1");
                    break;
                case JustifyType.right:
                    Write(ESC + "a2");
                    break;
                default:
                    Write(ESC + "a0");
                    break;
            }
        }

        /// <summary>
        /// Print a barcode
        /// </summary>
        /// <param name="data">barcode data</param>
        public void Barcode(String data)
        {
            Write(GS + "H2");
            Write(GS + "k" + ((char)'\x04') + data + ((char)'\x00'));
        }

        /// <summary>
        /// Feed lines
        /// </summary>
        /// <param name="n">Amount of lines to feed</param>
        public void Feed(int n)
        {
            int conv = int.Parse(n.ToString(), NumberStyles.HexNumber);
            Write(ESC + "d" + ((char)conv));
        }


        /// <summary>
        /// Set the font
        /// </summary>
        /// <param name="mT">Which mode to use</param>
        public void Mode(ModeType mT)
        {
            switch (mT)
            {
                case ModeType.FontA:
                    Write(ESC + "!" + ((char)(int)ModeType.FontA));
                    break;
                case ModeType.FontB:
                    Write(ESC + "!" + ((char)(int)ModeType.FontB));
                    break;
                case ModeType.Emphasized:
                    Write(ESC + "!" + ((char)(int)ModeType.Emphasized));
                    break;
                case ModeType.DoubleHeight:
                    Write(ESC + "!" + ((char)(int)ModeType.DoubleHeight));
                    break;
                case ModeType.DoubleWdith:
                    Write(ESC + "!" + ((char)(int)ModeType.DoubleWdith));
                    break;
                case ModeType.Underline:
                    Write(ESC + "!" + ((char)(int)ModeType.Underline));
                    break;
            }
        }

        /// <summary>
        /// Underline text
        /// </summary>
        /// <param name="b"></param>
        public void Underline(bool b)
        {
            if (b) Write(ESC + "-" + ((char)'\x01'));
            if (!b) Write(ESC + "-" + ((char)'\x00'));
        }

        /// <summary>
        /// Emphasize text
        /// </summary>
        /// <param name="b"></param>
        public void Emphasized(bool b)
        {
            if (b) Write(ESC + "E" + ((char)'\x01'));
            if (!b) Write(ESC + "E" + ((char)'\x00'));
        }

        /// <summary>
        /// Double Strike text
        /// </summary>
        /// <param name="b"></param>
        public void DoubleStrike(bool b)
        {
            if (b) Write(ESC + "G" + ((char)'\x01'));
            if (!b) Write(ESC + "G" + ((char)'\x00'));
        }

        /// <summary>
        /// Set the font
        /// </summary>
        /// <param name="fT"></param>
        public void SetFont(FontType fT)
        {
            switch (fT)
            {
                case FontType.FontA:
                    Write(ESC + "M" + ((char)(int)FontType.FontA));
                    break;
                case FontType.FontB:
                    Write(ESC + "M" + ((char)(int)FontType.FontB));
                    break;
                case FontType.FontC:
                    Write(ESC + "M" + ((char)(int)FontType.FontC));
                    break;
                case FontType.FontD:
                    Write(ESC + "M" + ((char)(int)FontType.FontD));
                    break;
                case FontType.FontE:
                    Write(ESC + "M" + ((char)(int)FontType.FontE));
                    break;
                case FontType.SpecialFontA:
                    Write(ESC + "M" + ((char)(int)FontType.SpecialFontA));
                    break;
                case FontType.SpecialFontB:
                    Write(ESC + "M" + ((char)(int)FontType.SpecialFontB));
                    break;
            }
        }

        /// <summary>
        /// Print in reverse color
        /// </summary>
        /// <param name="b"></param>
        public void ReversePrint(bool b)
        {
            if (b) Write(GS + "B" + ((char)'\x01'));
            if (!b) Write(GS + "B" + ((char)'\x00'));
        }

        /// <summary>
        /// Switch to a secondary color if supported
        /// </summary>
        /// <param name="b"></param>
        public void SetColor(bool b)
        {
            if (b) Write(ESC + "r" + ((char)'\x01'));
            if (!b) Write(ESC + "r" + ((char)'\x00'));
        }


        /// <summary>
        /// Open Cash Drawer
        /// </summary>
        /// <param name="dT">Which pin to activate draw on</param>
        public void OpenDraw(DrawType dT)
        {
            switch (dT)
            {
                case DrawType.Pin2:
                    Write(ESC + "p" + ((char)'\x00') + ((char)'\x50') + ((char)'\x50'));
                    break;
                case DrawType.Pin5:
                    Write(ESC + "p" + ((char)'\x01') + ((char)'\x50') + ((char)'\x50'));
                    break;
            }
        }

        /// <summary>
        /// Feed and cut the paper
        /// </summary>
        public void Cut()
        {
            Write(ESC + "d" + ((char)'\x05'));
            Write(GS + "V1");
        }

        /// <summary>
        /// Switch to inline pole display
        /// </summary>
        public void SwitchDisplay()
        {
            Write(ESC + "=" + ((char)'\x02'));
        }

        /// <summary>
        /// Switch to printer
        /// </summary>
        public void SwitchPrinter()
        {
            Write(ESC + "=" + ((char)'\x01'));
        }

        /// <summary>
        /// returns true if inlinePoleDisplay is enabled
        /// </summary>
        /// <returns></returns>
        public bool InlinePoleDisplay()
        {
            return inlinePole;
        }

        /// <summary>
        /// Print logo from NVRam
        /// </summary>
        public void PrintLogoNv()
        {
            Write(((char)'\x1c') + "p" + ((char)'\x01') + ((char)'\x00'));
        }

        /// <summary>
        /// A bad method to print a logo from file
        /// </summary>
        public void PrintLogo()
        {
            Write(ImageTools.GetLogo2());
        }
    }
}

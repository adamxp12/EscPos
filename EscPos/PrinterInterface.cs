using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EscPos
{
    public interface PrinterInterface
    {
        void Init();
        bool IsOpen();
        void Close();
        void Write(String line);
        void Write(List<String> lines);


    }
}

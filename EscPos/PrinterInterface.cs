using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EscPos
{
    public interface PrinterInterface
    {
        /// <summary>
        /// Init the printer
        /// </summary>
        /// <returns>True if connected succesfully</returns>
        bool Init();
        /// <summary>
        /// Check if the printer is connected
        /// </summary>
        /// <returns>True if connected</returns>
        bool IsOpen();
        /// <summary>
        /// Close the printer connection
        /// </summary>
        void Close();
        /// <summary>
        /// Print a string
        /// </summary>
        void Write(String line);
        /// <summary>
        /// Print a list of strings
        /// </summary>
        void Write(List<String> lines);


    }
}

# EscPos

This is a basic EscPos libary for .Net

It works with Serial, Ethernet/WIFI and LPT/USB via the Windows Printer drivers (not recomended)

Has many features but not 100% perfect. for example the is some code for BMP printing but it was just cobbled together from a blog and a stackoverflow post. the code is total junk though but no one seems to have anything better. Feel free to make a pull request if you manage to make BMP printing work correctly. I just get intermitent lines and sometimes a bunch of question marks. its also kinda slow but thats just sending image data over serial.

Most of the text functionality works great however. as does NV Logo printing and barcode printing. 

## To Be implemented
* QR Code printing
* Buzzer control
* Printer status


## Example code
This example code is for a Serial printer on COM5

```
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EscPos;

namespace TestPrint
{
    public partial class Form1 : Form
    {

        RecieptPrinter rp = new RecieptPrinter();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            rp.Init(new SerialPrinter("COM5", 19200), false);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if(rp.IsOpen())
            {
                rp.Write("Test print");
                rp.Cut();
                rp.flush();
            }
        }
    }
}
```

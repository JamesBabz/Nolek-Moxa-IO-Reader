using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace Nolek_Moxa_IO_Reader.Model
{
    public class IO
    {
        public List<DigitalInput> di { get; set; }
        public List<Relay> relay { get; set; }
    }
}